using SiteBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ServerDistributionBook
{
    /// <summary>
    /// Класс списка имитаций деятельности клиентов
    /// </summary>
    internal class GroupImitationClientActivity : IClientAvailability, IOperationSDB
    {
        /// <summary>
        /// Словарь:электроннный аддрес клиента - объект имитации клиентской активности
        /// </summary>
        private Dictionary<string, ImitationClientActivity> ClientDictionary;
        /// <summary>
        /// Объект управления сервером
        /// </summary>
        private ControlServerDB ServerDB;

        /// <summary>
        /// Конструктор списка имитаций деятельности клиентов
        /// </summary>
        /// <param name="serverDB">Объект управления сервером</param>
        public GroupImitationClientActivity (ControlServerDB serverDB)
        {
            ServerDB = serverDB;
            ClientDictionary = new Dictionary<string, ImitationClientActivity>();
        }

        /// <summary>
        /// Проверка наличия клиентов
        /// </summary>
        /// <returns>Наличие клиентов</returns>
        public bool ClientsAvailability()
        {
            bool availability = false;
            if (ClientDictionary.Count > 0)
            {
                availability = true;
            }
            return availability;
        }

        /// <summary>
        /// Проверка наличия клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <returns>Наличие клиента</returns>
        public bool ClientAvailability(string address)
        {
            bool availability = ClientDictionary.ContainsKey(address);
            return availability;
        }

        /// <summary>
        /// Добавление клиента
        /// </summary>
        /// <param name="clientServer">Объект клиента</param>
        public void AddClient(ClientServerDB clientServer)
        {
            ImitationClientActivity clientActivity = new ImitationClientActivity(clientServer, ServerDB);
            ClientDictionary.Add(clientServer.Address, clientActivity);
        }

        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void DeleteClient(string address)
        {
            ClientDictionary.Remove(address);
        }

        /// <summary>
        /// Запуск имитации деятельности клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void StartImitationActivityClient(string address)
        {
            if (ClientDictionary.ContainsKey(address))
            {
                ClientDictionary[address].StartImitationActivity();
            }
        }

        /// <summary>
        /// Остановка имитации деятельности клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void StopImitationActivityClient(string address)
        {
            if (ClientDictionary.ContainsKey(address))
            {
                ClientDictionary[address].StopImitationActivity();
            }
        }

        /// <summary>
        /// Предоставление очереди книг на чтение клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <returns>Очередь книг на чтение</returns>
        public Queue<string> GetQueueBook(string address)
        {
            return ClientDictionary[address].GetClientServerDB().QueueBook;
        }

        /// <summary>
        /// Поиск клиента, прочитающего книгу быстрее всех
        /// </summary>
        /// <param name="book">Книга</param>
        /// <param name="timeRead">Дата-время прочтения книги</param>
        /// <returns>Электронный адресс клиента</returns>
        public string SearchFastReader(BookSDB book, out DateTime timeRead)
        {
            string clientAddress = null;
            DateTime timeReadEnd = new DateTime();                      //промежуточное время окончания чтения книги
            DateTime timeReadNow = new DateTime();                      //время начала чтения книги
            timeRead = new DateTime();                                  //время окончания чтения книги
            bool first = true;

            var selectedClients = from client in ClientDictionary
                                  from lela in client.Value.GetClientServerDB().LevelLanguages
                                  where lela.Language == book.Language
                                  select client.Value.GetClientServerDB();

            foreach (var clientServer in selectedClients)
            {
                DateTime nowTime = DateTime.UtcNow.ToLocalTime();
                timeReadNow = clientServer.TimeRead > nowTime ?
                            clientServer.TimeRead : nowTime;

                timeReadEnd = timeReadNow.AddSeconds(ServerDB.TimeReadingBook(clientServer, book, timeReadNow));
                if (first || timeReadEnd < timeRead)
                {
                    if (first)
                    {
                        first = false;
                    }
                    clientAddress = clientServer.Address;
                    timeRead = timeReadEnd;
                }
            }
            return clientAddress;
        }

        /// <summary>
        /// Добавление книги и изменение даты-времени освобождения от чтения
        /// </summary>
        /// <param name="clientAddress">Электронный адресс клиента</param>
        /// <param name="bookJSON">Книга в JSON</param>
        /// <param name="timeRead">Дата-время освобождения от чтения клиента</param>
        public void AddBook(string clientAddress, string bookJSON, DateTime timeRead)
        {
            AddBookQueue(clientAddress, bookJSON);
            SetTimeRead(clientAddress, timeRead);
        }

        /// <summary>
        /// Добавление книги в очередь книг клиента
        /// </summary>
        /// <param name="clientAddress">Электронный адресс клиента</param>
        /// <param name="bookJSON">Книга в JSON</param>
        public void AddBookQueue(string clientAddress, string bookJSON)
        {
            if (ClientDictionary.ContainsKey(clientAddress))
            {
                GetQueueBook(clientAddress).Enqueue(bookJSON);
            }
        }

        /// <summary>
        /// Изменение даты-времени освобождения от чтения клиента
        /// </summary>
        /// <param name="clientAddress">Электронный адресс клиента</param>
        /// <param name="timeRead">Дата-время освобождения от чтения клиента</param>
        public void SetTimeRead(string clientAddress, DateTime timeRead)
        {
            if (ClientDictionary.ContainsKey(clientAddress))
            {
                ClientDictionary[clientAddress].GetClientServerDB().TimeRead = timeRead;
            }
        }

        /// <summary>
        /// Информирование об отправки книги
        /// </summary>
        /// <param name="clientAddress">Электронный адресс клиента</param>
        public void InformSendingBook(string clientAddress)
        {
            if (ClientDictionary.ContainsKey(clientAddress))
            {
                ClientDictionary[clientAddress].StartSleepThread();
            }
        }

        /// <summary>
        /// Возвращение поверхностной копии клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <returns>Поверхностной копия клиента</returns>
        public ClientServerDB CloneClient(string address)
        {
            ClientServerDB clientServer = null;
            if (ClientDictionary.ContainsKey(address))
            {
                clientServer = (ClientServerDB)ClientDictionary[address].GetClientServerDB().Clone();
            }
            return clientServer;
        }

        /// <summary>
        /// Разгрузка очередей перегруженных клиентов
        /// </summary>
        /// <param name="client">Потенциальный клиент на разгруженные книги</param>
        /// <param name="alignmentCoefficient">Коэффициент выравнивания загружености клиентов</param>
        /// <returns>Очередь разгруженых книг</returns>
        public Dictionary<string, Queue<string>> UnloadingQueuesClients(ClientSDB client, double alignmentCoefficient)
        {
            Dictionary<string, Queue<string>> unloadingBooksQueue = new Dictionary<string, Queue<string>>();
            DateTime reloadTimeRead = СalculationСriticalTimeRead(alignmentCoefficient);

            var selectedClient = from clientServer in ClientDictionary
                                 from lelaClientServer in clientServer.Value.GetClientServerDB().LevelLanguages
                                 from lelaClient in client.LevelLanguages
                                 where lelaClientServer.Language == lelaClient.Language 
                                     && clientServer.Value.GetClientServerDB().TimeRead > reloadTimeRead
                                 select clientServer.Value;
            foreach (var selectImitationActivity in selectedClient)
            {
                selectImitationActivity.SetWaitOneMutexJointOperation();
                ClientServerDB selectClientServer = selectImitationActivity.GetClientServerDB();
                unloadingBooksQueue.Add(selectClientServer.Address, new Queue<string>());
                while (selectClientServer.TimeRead > reloadTimeRead)
                {
                    if (selectClientServer.QueueBook.Count > 0)
                    {
                        string book = selectClientServer.QueueBook.Dequeue();
                        unloadingBooksQueue[selectClientServer.Address].Enqueue(book);
                        SetRecalculateTimeRead(selectClientServer.Address);
                    }
                }
            }
            return unloadingBooksQueue;
        }

        /// <summary>
        /// Расчет даты-времени перегрузки
        /// </summary>
        /// <param name="alignmentCoefficient">Коэффициент выравнивания загружености клиентов</param>
        /// <returns>Дата-время перегрузки</returns>
        private DateTime СalculationСriticalTimeRead(double alignmentCoefficient)
        {
            DateTime nowNime = DateTime.UtcNow.ToLocalTime();
            double averageTime = ClientDictionary.Average(GetTimeReading);
            double reloadTime = averageTime * alignmentCoefficient;
            DateTime reloadTimeRead = nowNime.AddSeconds(reloadTime);
            return reloadTimeRead;
        }

        /// <summary>
        /// Возвращение времени чтения книг
        /// </summary>
        /// <param name="dictionaryObject">Объект словаря</param>
        /// <returns>Время чтения книг</returns>
        private double GetTimeReading(KeyValuePair<string, ImitationClientActivity> dictionaryObject)
        {
            DateTime nowTime = DateTime.UtcNow.ToLocalTime();
            DateTime timeRead = dictionaryObject.Value.GetClientServerDB().TimeRead;
            TimeSpan intervalTime = timeRead > nowTime ? timeRead - nowTime : TimeSpan.Zero;
            return intervalTime.TotalSeconds;
        }

        /// <summary>
        /// Изменение даты-времени освобождения от чтения на перерасчитаное
        /// </summary>
        /// <param name="address"></param>
        public void SetRecalculateTimeRead(string address)
        {
            ClientServerDB clientServer = ClientDictionary[address].GetClientServerDB();
            clientServer.TimeRead = RecalculateTimeRead(address);
        }

        /// <summary>
        /// Перерасчет даты-времени освобождения клиента от чтения
        /// </summary>
        /// <param name="clientServer">Клиент сервера</param>
        /// <returns>Даты-время освобождения клиента от чтения</returns>
        public DateTime RecalculateTimeRead(string address)
        {
            ClientServerDB clientServer = ClientDictionary[address].GetClientServerDB();
            int queueBookCount = clientServer.QueueBook.Count;
            DateTime nowTime = DateTime.UtcNow.ToLocalTime();
            DateTime timeReadRecalculation = clientServer.TimeReadActive < nowTime ? nowTime : clientServer.TimeReadActive;
            for (int i = 0; i < queueBookCount; i++)
            {
                string bookJSON = clientServer.QueueBook.Dequeue();
                clientServer.QueueBook.Enqueue(bookJSON);
                BookSDB book = JsonSerializer.Deserialize<BookSDB>(bookJSON);
                double timeSleep = ServerDB.TimeReadingBook(clientServer, book, timeReadRecalculation);
                timeReadRecalculation = timeReadRecalculation.AddSeconds(timeSleep);
            }
            return timeReadRecalculation;
        }

        public void SetReleaseMutexJointOperation(string address)
        {
            if (ClientDictionary.ContainsKey(address))
            {
                ClientDictionary[address].SetReleaseMutexJointOperation();
            }
        }
    }
}
