
using NLog;
using RabbitMQ.Client.Events;
using SiteBook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;


namespace ServerDistributionBook
{
    /// <summary>
    /// Класс управления сервером
    /// </summary>
    internal class ControlServerDB
    {
        /// <summary>
        /// Объект работы сo списком имитаций деятельности клиентов
        /// </summary>
        private GroupImitationClientActivity WorkClientGroup;
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Объект работы с MongoDB
        /// </summary>
        private MongoDbSDB WorkMongo;
        /// <summary>
        /// Объект работы с MySQL
        /// </summary>
        private MySqlSDB WorkMySQL;
        /// <summary>
        /// Объект работы с RabbitMQ
        /// </summary>
        public RabbitmqSDB WorkRabbit;
        /// <summary>
        /// Мьютекс выполнения одного метода
        /// </summary>
        private Mutex MutexOnlyMethod;
        /// <summary>
        /// Переменная секундного эквивалента дня
        /// </summary>
        private int daysSecond;
        /// <summary>
        /// Секундный эквивалент дня
        /// </summary>
        private int DaysSecond
        {
            get
            {
                return daysSecond;
            }
            set
            {
                if (value >= DaysSecondMin)
                {
                    daysSecond = value;
                }
                else
                {
                    daysSecond = DaysSecondDefault;
                }
            }
        }
        /// <summary>
        /// Коэффициент выравнивания загружености клиентов
        /// </summary>
        private double AlignmentCoefficient = 2;
        /// <summary>
        /// Значение секундного эквивалента дня по умолчанию
        /// </summary>
        private const int DaysSecondDefault = 24;
        /// <summary>
        /// Минимальное значение секундного эквивалента дня
        /// </summary>
        private const int DaysSecondMin = 1;

        /// <summary>
        /// Конструктор объекта управления сервером
        /// </summary>
        /// <param name="daysSecond">Секундный эквивалент дня</param>
        public ControlServerDB(int daysSecond)
        {
            WorkClientGroup = new GroupImitationClientActivity(this);
            WorkMongo = new MongoDbSDB(this);
            WorkMySQL = new MySqlSDB(this);
            MutexOnlyMethod = new Mutex();
            DaysSecond = daysSecond;
            WorkRabbit = new RabbitmqSDB(this);
        }

        /// <summary>
        /// Обработки данных клиента из RabbitMQ
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <param name="e">Сообщение из RabbitMQ</param>
        public void AddDeleteClientFromRabbitMQ(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                TryAddDeleteClientRequest(sender, e);
            }
            catch (Exception exception)
            {
                if (!CheckException(exception))
                {
                    Log.Error(exception.ToString);
                }
                StopServer();
            }
        }

        /// <summary>
        ///  Обработки данных клиента из RabbitMQ без обработки общих исключений
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <param name="e">Сообщение из RabbitMQ</param>
        private void TryAddDeleteClientRequest(object sender, BasicDeliverEventArgs e)
        {
            MutexOnlyMethod.WaitOne();
            string bodyType = e.BasicProperties.Type;
            if (bodyType == WorkRabbit.PropetyAdding)
            {
                ClientSDB client = WorkRabbit.ConvertMessageRabbit<ClientSDB>(e.Body);
                UnloadingQueuesClientsServer(client);
                AddClientServer(client);
                WorkClientGroup.StartImitationActivityClient(client.Address);
            }
            else if (bodyType == WorkRabbit.PropetyDeleted)
            {
                string address = WorkRabbit.ConvertMessageRabbit<string>(e.Body);
                WorkClientGroup.StopImitationActivityClient(address);
                DeleteClientServer(address);
            }
            WorkRabbit.BasicAckRabbit(sender, e);
            MutexOnlyMethod.ReleaseMutex();
        }

        /// <summary>
        /// Разгрузка очередей перегруженных клиентов
        /// </summary>
        /// <param name="client">Потенциальный клиент на разгруженные книги</param>
        private void UnloadingQueuesClientsServer(ClientSDB client)
        {
            if (!WorkClientGroup.ClientsAvailability())
            {
                return;
            }
            Dictionary<string, Queue<string>> unloadingBooksQueues = WorkClientGroup.UnloadingQueuesClients(client, AlignmentCoefficient);
            foreach (var dictionaryBookQueue in unloadingBooksQueues)
            {
                int countBook = dictionaryBookQueue.Value.Count;
                for (int i = 0; i < countBook; i++)
                {
                    WorkMongo.DeletionBookFromQueue(dictionaryBookQueue.Key);
                    string book = dictionaryBookQueue.Value.Peek();
                    try
                    {
                        WorkRabbit.PublishBookQueue(book);
                    }
                    catch (RabbitmqException)
                    {
                        WorkMongo.AddBookInQueue(dictionaryBookQueue.Key, book);
                        StopServer();
                    }
                    dictionaryBookQueue.Value.Dequeue();
                }
                WorkClientGroup.SetRecalculateTimeRead(dictionaryBookQueue.Key);
                WorkClientGroup.SetReleaseMutexJointOperation(dictionaryBookQueue.Key);
            }
        }

        /// <summary>
        /// Добавление клиента в системы сервера
        /// </summary>
        /// <param name="client">Клиент</param>
        private void AddClientServer(ClientSDB client)
        {
            if (!WorkClientGroup.ClientAvailability(client.Address) && !WorkMySQL.ClientAvailability(client.Address))
            {
                ClientServerDB clientServer = new ClientServerDB(client);

                WorkClientGroup.AddClient(clientServer);
                WorkMongo.AddClient(clientServer);
                try
                {
                    WorkMySQL.AddClient(clientServer);
                    WorkRabbit.RedistributionQueue(WorkRabbit.QueueUnallocated, WorkRabbit.ExchangeBook, WorkRabbit.RoutingBook);
                }
                catch (MySqlException)
                {
                    WorkMongo.DeleteClient(clientServer.Address);
                    StopServer();
                }
                catch (RabbitmqException)
                {
                    WorkMongo.DeleteClient(clientServer.Address);
                    WorkMySQL.FullDeleteClient(clientServer.Address);
                    StopServer();
                }
            }
        }

        /// <summary>
        /// Удаление клиента из систем сервера
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        private void DeleteClientServer(string address)
        {
            if (WorkClientGroup.ClientAvailability(address))
            {
                ClientServerDB clientServer = WorkClientGroup.CloneClient(address);
                WorkClientGroup.DeleteClient(address);
                WorkMongo.DeleteClient(address);
                try
                {
                    WorkMySQL.DeleteClient(address);
                    WorkRabbit.RedistributionQueue(clientServer.QueueBook, WorkRabbit.ExchangeBook, WorkRabbit.RoutingBook);
                }
                catch (MySqlException)
                {
                    WorkMongo.AddClient(clientServer);
                    StopServer();
                }
                catch (RabbitmqException)
                {
                    WorkMongo.AddClient(clientServer);
                    WorkMySQL.SetSubscriptionOn(address);
                    StopServer();
                }
            }
        }

        /// <summary>
        /// Распределение книги из RabbitMQ
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <param name="e">Сообщение из RabbitMQ</param>
        public void DistributionBookRequest(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                TryDistributionBookRequest(sender, e);
            }
            catch (Exception exception)
            {
                if (!CheckException(exception))
                {
                    Log.Error(exception.ToString);
                }
                StopServer();
            }
        }

        /// <summary>
        /// Распределение книги из RabbitMQ без обработки общих исключений
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <param name="e">Сообщение из RabbitMQ</param>
        private void TryDistributionBookRequest(object sender, BasicDeliverEventArgs e)
        {
            MutexOnlyMethod.WaitOne();
            BookSDB book = WorkRabbit.ConvertMessageRabbit<BookSDB>(e.Body);
            DateTime timeRead;
            string clientAddress = WorkClientGroup.SearchFastReader(book, out timeRead);
            if (clientAddress == null)
            {
                WorkRabbit.PublishUnallocatedQueue(e);
                WorkRabbit.BasicAckRabbit(sender, e);
            }
            else
            {
                string bookJSON = WorkRabbit.ConvertMessageRabbit<string>(e.Body);
                AddBookServer(clientAddress, bookJSON, timeRead);
                try
                {
                    WorkRabbit.BasicAckRabbit(sender, e);
                }
                catch (RabbitmqException)
                {
                    WorkMongo.DeleteLastBook(clientAddress);
                    StopServer();
                }
                WorkClientGroup.InformSendingBook(clientAddress);
            }
            MutexOnlyMethod.ReleaseMutex();
        }

        /// <summary>
        /// Добавление книги в системы сервера
        /// </summary>
        /// <param name="clientAddress">"Электронный адресс клиента</param>
        /// <param name="bookJSON">Книга в JSON</param>
        /// <param name="timeRead">Дата-время освобождения клиента от чтения</param>
        private void AddBookServer(string clientAddress, string bookJSON, DateTime timeRead)
        {
            WorkClientGroup.AddBook(clientAddress, bookJSON, timeRead);
            WorkMongo.AddBook(clientAddress, bookJSON, timeRead);
        }

        /// <summary>
        /// Загрузка клиентов на сервер
        /// </summary>
        public void ObjectOverload()
        {
            List<ClientServerDB> people = WorkMongo.GetAllClients();

            foreach (ClientServerDB clientServer in people)
            {
                WorkClientGroup.AddClient(clientServer);
                DateTime timeReadRecalculation = WorkClientGroup.RecalculateTimeRead(clientServer.Address);
                WorkClientGroup.SetTimeRead(clientServer.Address, timeReadRecalculation);
                WorkMongo.SetTimeRead(clientServer.Address, timeReadRecalculation);
                WorkClientGroup.StartImitationActivityClient(clientServer.Address);
            }
        }

        /// <summary>
        /// Запуск прослушивания очередей RabbitMQ
        /// </summary>
        public void StartListeningQueuesRabbit()
        {
            WorkRabbit.StartListeningQueues();
        }

        /// <summary>
        /// Проверка наличия клиентов в MongoDB
        /// </summary>
        /// <returns>Наличие клиентов в MongoDB</returns>
        public bool MongoClientsAvailability()
        {
            return WorkMongo.ClientsAvailability();
        }

        /// <summary>
        /// Расчет времени чтения книги
        /// </summary>
        /// <param name="clientServer">Клиент сервера</param>
        /// <param name="book">Книги</param>
        /// <param name="dataTime">Дата-время, относительно которого производится расчет</param>
        /// <returns>Время чтения книги (в секундах)</returns>
        public double TimeReadingBook(ClientServerDB clientServer, BookSDB book, DateTime dataTime)
        {
            double timeReadingBook;                                                                         //время чтения книги
            int level = 0;                                                                                  //уровень владения языком
            foreach (LevelLanguageSDB lela in clientServer.LevelLanguages)
            {
                if (lela.Language == book.Language)
                {
                    level = lela.Level;
                    break;
                }
            }
            if (level == 0)
                throw new Exception($"Клиент не владеет языком {book.Language}");
            TimeSpan interval = dataTime - clientServer.DataRegistration;                                                   //интервал времени от данного до времени регистрации
            int readingIntervalActiveSecond = clientServer.ReadingIntervalActive * DaysSecond;                              //время активного чтения в цикле
            int readingIntervalPassiveSecond = clientServer.ReadingIntervalPassive * DaysSecond;                            //время пассивного чтения чтения в цикле
            int cycleReading = readingIntervalActiveSecond + readingIntervalPassiveSecond;                                  //время цикла чтения
            double levelPagesPerCycle = clientServer.ReadingIntervalActive * clientServer.PagesPerDay * (level / 10.0);     //количество читаемых страниц в цикл
            double balanseInterval = interval.TotalSeconds % (double)cycleReading;                                  //текущее время нового цикла
            double remainingActiveCycle = readingIntervalActiveSecond - balanseInterval;                                    //оставшееся время цикла активного чтения
            double timeReadingBookNCCycle = (double)(book.Pages % levelPagesPerCycle) / (double)levelPagesPerCycle          //время чтения неполного цикла
                * (double)readingIntervalActiveSecond;
            int TimeReadingBookFullCycle = (int)(book.Pages / levelPagesPerCycle) * cycleReading;                           //время чтения полных циклов

            if (balanseInterval >= readingIntervalActiveSecond)
            {
                timeReadingBook = TimeReadingBookFullCycle + timeReadingBookNCCycle + (cycleReading - balanseInterval);
            }
            else
            {
                if (remainingActiveCycle > timeReadingBookNCCycle)
                {
                    timeReadingBook = TimeReadingBookFullCycle + timeReadingBookNCCycle;
                }
                else
                {
                    timeReadingBook = TimeReadingBookFullCycle + timeReadingBookNCCycle + readingIntervalPassiveSecond;
                }
            }
            return timeReadingBook;
        }

        /// <summary>
        /// Остановка сервера
        /// </summary>
        /// <param name="executionResult">Результат выполнения</param>
        public void StopServer()
        {
            WorkRabbit.Dispose();
            Environment.Exit(0);
        }

        /// <summary>
        /// Изменение коэффициента
        /// выравнивания загружености клиентов
        /// </summary>
        /// <param name="alignmentCoefficient">Коэффициент выравнивания загружености клиентов</param>
        public void ChangeAlignmentCoefficient(int alignmentCoefficient)
        {
            if (alignmentCoefficient > 1)
            {
                AlignmentCoefficient = alignmentCoefficient;
            }
        }

        public static bool CheckException(Exception exception)
        {
            bool check = false;
            if (exception is MongoException || exception is MySqlException || exception is RabbitmqException)
            {
                check = true;
            }
            return check;
        }
    }
}
