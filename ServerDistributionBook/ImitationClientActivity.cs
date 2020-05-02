using NLog;
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
        private MySqlSDB MySqlConnection;
        /// <summary>
        /// Объект работы с MongoDB
        /// </summary>
        private MongoDbSDB MongoConnection;
        /// <summary>
        /// Объект блокировки потока до получения сигнала
        /// </summary>
        private AutoResetEvent WaitHandler;
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();

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
            MySqlConnection = new MySqlSDB(ServerDB);
            MongoConnection = new MongoDbSDB(ServerDB);
            WaitHandler = new AutoResetEvent(true);
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
            catch (Exception exception)
            {
                if (!ControlServerDB.CheckException(exception))
                {
                    Log.Error(exception.ToString);
                }
                ServerDB.StopServer();
            }
        }

        /// <summary>
        /// Получение и чтение книг клиентом без обработки общих исключений
        /// </summary>
        private void TryGettingAndReadingBook()
        {
            int MilisecondOfSecond = 1000;
            ReadingActiveBook();
            while (true)
            {
                if (ClientServer.QueueBook.Count == 0)
                {
                    WaitHandler.Reset();
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
                MySqlConnection.ImplementationBook(ClientServer.Address, DateTime.UtcNow.ToLocalTime());
            }
        }

        /// <summary>
        /// Чтение активной книги
        /// </summary>
        private void ReadingActiveBook()
        {
            object idBook = MySqlConnection.GetIdUnreadBook(ClientServer.Address);
            if (idBook != null)
            {
                DateTime dataReading = ClientServer.TimeReadActive;
                if (dataReading > DateTime.UtcNow.ToLocalTime())
                {
                    TimeSpan timeSleepActive = dataReading - DateTime.UtcNow.ToLocalTime();
                    Thread.Sleep(timeSleepActive);
                    dataReading = DateTime.UtcNow.ToLocalTime();
                }
                MySqlConnection.AddDataReadingBook(idBook, dataReading);
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
            MySqlConnection.AddBook(ClientServer.Address, bookJSON, DateTime.UtcNow.ToLocalTime());
            try
            {
                MongoConnection.ImplementationBook(ClientServer.Address, timeCompleteReadActive);
            }
            catch (MongoException)
            {
                MySqlConnection.DeleteLastBook(ClientServer.Address);
                ServerDB.StopServer();
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
