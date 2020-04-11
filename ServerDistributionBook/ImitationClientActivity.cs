using SiteBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServerDistributionBook
{
    /// <summary>
    /// Класс имитации клиентской деятельности
    /// </summary>
    internal class ImitationClientActivity
    {
        /// <summary>
        /// Объект управления сервером
        /// </summary>
        private ControlServerDB ServerDB;
        /// <summary>
        /// Объект клиента сервера
        /// </summary>
        private ClientServerDB ClientServer;
        /// <summary>
        /// Поток имитации деятельности клиента
        /// </summary>
        private Thread ImitationActivity;
        /// <summary>
        /// Мьютекс выполнения объединенных операций
        /// </summary>
        private Mutex MutexJointOperation;
        /// <summary>
        /// Объект работы с MySQL
        /// </summary>
        private MySQL_SDB MySQL_Connection;
        /// <summary>
        /// Объект работы с MongoDB
        /// </summary>
        private MongoDB_SDB Mongo_Connection;
        /// <summary>
        /// Объект блокировки потока до получения сигнала
        /// </summary>
        private AutoResetEvent WaitHandler;

        /// <summary>
        /// Конструктор объекта имитации деятельности клиента
        /// </summary>
        /// <param name="clientServer"></param>
        /// <param name="serverDB"></param>
        public ImitationClientActivity(ClientServerDB clientServer, ControlServerDB serverDB)
        {
            ServerDB = serverDB;
            ClientServer = clientServer;
            ImitationActivity = new Thread(new ThreadStart(GettingAndReadingBook));
            MutexJointOperation = new Mutex();
            MySQL_Connection = new MySQL_SDB();
            Mongo_Connection = new MongoDB_SDB();
            WaitHandler = new AutoResetEvent(false);
        }

        /// <summary>
        /// Получение и чтение книг клиентом
        /// </summary>
        private void GettingAndReadingBook()
        {
            try
            {
                TryGettingAndReadingBook();               
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                string executionResult = "Ошибка связи с MySQL";
                ServerDB.StopServer(executionResult);
            }
            catch (Exception exception)
            {
                ServerDB.StopServer(exception.Message);
            }
        }

        /// <summary>
        /// Получение и чтение книг клиентом без обработки исключений
        /// </summary>
        private void TryGettingAndReadingBook()
        {
            int MilisecondOfSecond = 1000;
            ReadingActiveBook();
            while (true)
            {
                if (ClientServer.QueueBook.Count == 0)
                {
                    WaitHandler.WaitOne();
                }

                MutexJointOperation.WaitOne();
                string bookJSON = ClientServer.QueueBook.Dequeue();
                BookSDB book = JsonSerializer.Deserialize<BookSDB>(bookJSON);
                double timeSleep = ServerDB.TimeReadingBook(ClientServer, book, DateTime.UtcNow.ToLocalTime());
                DateTime timeComleteReadActive = DateTime.UtcNow.ToLocalTime().AddSeconds(timeSleep);
                SendBookClient(bookJSON, timeComleteReadActive);
                MutexJointOperation.ReleaseMutex();

                Thread.Sleep((int)timeSleep * MilisecondOfSecond);
                MySQL_Connection.ImplementationBook(ClientServer.Address, DateTime.UtcNow.ToLocalTime());
            }
        }

        /// <summary>
        /// Чтение активной книги
        /// </summary>
        private void ReadingActiveBook()
        {
            object idBook = MySQL_Connection.GetIdUnreadBook(ClientServer.Address);
            if (idBook != null)
            {
                DateTime dataReading = ClientServer.TimeReadActive;
                if (dataReading > DateTime.UtcNow.ToLocalTime())
                {
                    TimeSpan timeSleepActive = dataReading - DateTime.UtcNow.ToLocalTime();
                    Thread.Sleep(timeSleepActive);
                    dataReading = DateTime.UtcNow.ToLocalTime();
                }
                MySQL_Connection.AddDataReadingBook(idBook, dataReading);
            }
        }

        /// <summary>
        /// Отправка книги 
        /// </summary>
        /// <param name="bookJSON">Книга в JSON</param>
        /// <param name="timeCompleteReadActive">Дата-время завершения чтения книги</param>
        private void SendBookClient(string bookJSON, DateTime timeCompleteReadActive)
        {
            ClientServer.TimeReadActive = timeCompleteReadActive;
            MySQL_Connection.AddBook(ClientServer.Address, bookJSON, DateTime.UtcNow.ToLocalTime());
            try
            {
                Mongo_Connection.ImplementationBook(ClientServer.Address, timeCompleteReadActive);
            }
            catch (MongoDB.Driver.MongoConnectionException)
            {
                MySQL_Connection.DeleteLastBook(ClientServer.Address);
                string executionResult = "Ошибка соединения с MongoDB";
                ServerDB.StopServer(executionResult);
            }
        }

        /// <summary>
        /// Остановка потока имитации деятельности клиента
        /// </summary>
        public void StopImitationActivity()
        {
            if (ImitationActivity.ThreadState != ThreadState.Unstarted)
            {
                MutexJointOperation.WaitOne();
                ImitationActivity.Abort();
                MutexJointOperation.ReleaseMutex();
            }
        }

        /// <summary>
        /// Запуск потока имитации деятельности клиента
        /// </summary>
        public void StartImitationActivity()
        {
            if (ImitationActivity.ThreadState == ThreadState.Unstarted)
            {
                ImitationActivity.Start();
            }
        }

        /// <summary>
        /// Возвращение объекта клиента
        /// </summary>
        /// <returns>Объект клиента</returns>
        public ClientServerDB GetClientServerDB()
        {
            return ClientServer;
        }

        /// <summary>
        /// Запуск спящего потока имитации деятельности клиента
        /// </summary>
        public void StartSleepThread()
        {
            if (ImitationActivity.ThreadState == ThreadState.WaitSleepJoin)
            {
                WaitHandler.Set();
            }
        }

        /// <summary>
        /// Взятие мьютекса выполнения объединенных операций
        /// </summary>
        public void SetWaitOneMutexJointOperation()
        {
            MutexJointOperation.WaitOne();
        }

        /// <summary>
        /// Освобождение мьютекса выполнения объединенных операций
        /// </summary>
        public void SetReleaseMutexJointOperation()
        {
            MutexJointOperation.ReleaseMutex();
        }
    }
}
