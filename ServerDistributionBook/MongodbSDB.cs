using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using SiteBook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDistributionBook
{
    /// <summary>
    /// Класс работы с MongoDB
    /// </summary>
    internal class MongoDbSDB: IOperationSDBDataBase
    {
        /// <summary>
        /// Коллекция MongoDB
        /// </summary>
        private IMongoCollection<ClientServerDB> CollectionMongo;
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Объект управления сервером
        /// </summary>
        private ControlServerDB ServerDB;

        /// <summary>
        /// Конструктор объекта работы с MongoDB
        /// </summary>
        public MongoDbSDB(ControlServerDB serverDB)
        {
            ServerDB = serverDB;
            string connectionStringMongo = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString; ;
            string dataBase = "SystemDistributionBook";
            string collection = "SDBCollection";
            MongoClient ClientMongo = new MongoClient(connectionStringMongo);
            IMongoDatabase DatabaseMongo = ClientMongo.GetDatabase(dataBase);
            try
            {
                DatabaseMongo.RunCommand((Command<BsonDocument>)"{ping:1}");
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                ServerDB.StopServer();
            }
            CollectionMongo = DatabaseMongo.GetCollection<ClientServerDB>(collection);
        }

        /// <summary>
        /// Удаление первой книги из списка книг и изменение даты-времени чтения активной книги
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <param name="dateTime">Дата-время чтения активной книги</param>
        public void ImplementationBook(string address, DateTime dateTime)
        {
            SetTimeReadActive(address, dateTime);
            DeletionBookFromQueue(address);
        }

        /// <summary>
        /// Изменение даты-времени чтения активной книги
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <param name="dateTime">Дата-время чтения активной книги</param>
        public void SetTimeReadActive(string address, DateTime dateTime)
        {
            try
            {
                var filter = Builders<ClientServerDB>.Filter.Eq("Address", address);
                var updateTimeReadActive = Builders<ClientServerDB>.Update.Set(x => x.TimeReadActive, dateTime);
                CollectionMongo.UpdateOne(filter, updateTimeReadActive);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new MongoException();
            }
        }

        /// <summary>
        /// Удаление первой книги в очереди
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void DeletionBookFromQueue(string address)
        {
            try
            {
                var filter = Builders<ClientServerDB>.Filter.Eq("Address", address);
                var updateBook = Builders<ClientServerDB>.Update.PopFirst(x => x.QueueBook);
                CollectionMongo.UpdateOne(filter, updateBook);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new MongoException();
            }
        }

        /// <summary>
        /// Добавление клиента
        /// </summary>
        /// <param name="clientServer">Объект клиента</param>
        public void AddClient(ClientServerDB clientServer)
        {
            try
            {
                CollectionMongo.InsertOne(clientServer);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                ServerDB.StopServer();
            }
        }

        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void DeleteClient(string address)
        {
            try
            {
                CollectionMongo.DeleteOne(p => p.Address == address);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                ServerDB.StopServer();
            }
        }

        /// <summary>
        /// Добавление книги в конец очереди и изменение даты-времени освобождения клиента от чтения
        /// </summary>
        /// <param name="clientAddress">Электронный адресс клиента</param>
        /// <param name="bookJSON">Книга в JSON</param>
        /// <param name="timeRead">Даты-время освобождения клиента от чтения</param>
        public void AddBook(string clientAddress, string bookJSON, DateTime timeRead)
        {
            AddBookInQueue(clientAddress, bookJSON);
            SetTimeRead(clientAddress, timeRead);
        }

        /// <summary>
        /// Добавление книги в конец очереди
        /// </summary>
        /// <param name="clientAddress">Электронный адресс клиента</param>
        /// <param name="bookJSON">Книга в JSON</param>
        public void AddBookInQueue(string clientAddress, string bookJSON)
        {
            try
            {
                var filter = Builders<ClientServerDB>.Filter.Eq("Address", clientAddress);
                var updateBook = Builders<ClientServerDB>.Update.AddToSet(x => x.QueueBook, bookJSON);
                CollectionMongo.UpdateOne(filter, updateBook);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                ServerDB.StopServer();
            }
        }

        /// <summary>
        /// Изменение даты-времени освобождения клиента от чтения
        /// </summary>
        /// <param name="clientAddress">Электронный адресс клиента</param>
        /// <param name="timeRead">Даты-время освобождения клиента от чтения</param>
        public void SetTimeRead(string clientAddress, DateTime timeRead)
        {
            try
            {
                var filter = Builders<ClientServerDB>.Filter.Eq("Address", clientAddress);
                var updateTimeRead = Builders<ClientServerDB>.Update.Set(x => x.TimeRead, timeRead);
                CollectionMongo.UpdateOne(filter, updateTimeRead);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                ServerDB.StopServer();
            }
        }

        /// <summary>
        /// Проверка наличия клиентов
        /// </summary>
        /// <returns>Наличие клиентов</returns>
        public bool ClientsAvailability()
        {
            bool availability = false;
            try
            {
                availability = CollectionMongo.CountDocuments(new BsonDocument()) > 0;
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                ServerDB.StopServer();
            }
            return availability;
        }

        /// <summary>
        /// Возвращение объектов всех клиентов
        /// </summary>
        /// <returns>Список объектов клиентов</returns>
        public List<ClientServerDB> GetAllClients()
        {
            List<ClientServerDB> people = null;
            try
            {
                var filter = new BsonDocument();
                people = CollectionMongo.Find(filter).ToList();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                ServerDB.StopServer();
            }
            return people;
        }

        /// <summary>
        /// Удаление последней книги в очереди
        /// </summary>
        /// <param name="address"></param>
        public void DeleteLastBook(string address)
        {
            try
            {
                var filter = Builders<ClientServerDB>.Filter.Eq("Address", address);
                var updateBook = Builders<ClientServerDB>.Update.PopLast(x => x.QueueBook);
                CollectionMongo.UpdateOne(filter, updateBook);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                ServerDB.StopServer();
            }
        }
    }
}
