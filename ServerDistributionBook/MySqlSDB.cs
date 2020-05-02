using MySql.Data.MySqlClient;
using NLog;
using SiteBook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerDistributionBook
{
    /// <summary>
    /// Класс работы с MySQL
    /// </summary>
    internal class MySqlSDB: IClientAvailability, IOperationSDBDataBase
    {
        /// <summary>
        /// Соединение с MySQL
        /// </summary>
        private MySqlConnection ConnectionMySQL;
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Объект управления сервером
        /// </summary>
        private ControlServerDB ServerDB;

        /// <summary>
        /// Конструктор объекта работы с MySQL
        /// </summary>
        public MySqlSDB(ControlServerDB serverDB)
        {
            ServerDB = serverDB;
            string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
            ConnectionMySQL = new MySqlConnection(connectionString);
            try
            {
                ConnectionMySQL.Open();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
        }

        /// <summary>
        /// Проверка наличия клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <returns>Наличие клиента</returns>
        public bool ClientAvailability(string address)
        {
            string sqlCommand = "SELECT Address FROM clients WHERE Address = @Address";
            bool hasRows = false;
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand commandChek = new MySqlCommand(sqlCommand, ConnectionMySQL);
                commandChek.Parameters.AddWithValue("@Address", address);
                MySqlDataReader reader = commandChek.ExecuteReader();
                hasRows = reader.HasRows;
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
            return hasRows;
        }

        /// <summary>
        /// Добавление клиента
        /// </summary>
        /// <param name="clientServer">Объект клиента</param>
        public void AddClient(ClientServerDB clientServer)
        {
            AddClientInfo(clientServer);           
            AddLevelLanguages(clientServer);
        }

        /// <summary>
        /// Добавление информации о клиенте
        /// </summary>
        /// <param name="clientServer">Объект клиента</param>
        public void AddClientInfo(ClientServerDB clientServer)
        {
            string sqlCommand = "INSERT INTO clients (Address, Surname, Name, PagesPerDay, ReadingIntervalActive, " +
                                "ReadingIntervalPassive, DataRegistration, Subscription) " +
                                "VALUES (@Addres, @Surname, @Name, @PagesPerDay, @ReadingIntervalActive, " +
                                "@ReadingIntervalPassive, @DataRegistration, @Subscription)";
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand command = new MySqlCommand(sqlCommand, ConnectionMySQL);
                command.Parameters.AddWithValue("@Addres", clientServer.Address);
                command.Parameters.AddWithValue("@Surname", clientServer.Surname);
                command.Parameters.AddWithValue("@Name", clientServer.Name);
                command.Parameters.AddWithValue("@PagesPerDay", clientServer.PagesPerDay);
                command.Parameters.AddWithValue("@ReadingIntervalActive", clientServer.ReadingIntervalActive);
                command.Parameters.AddWithValue("@ReadingIntervalPassive", clientServer.ReadingIntervalPassive);
                command.Parameters.AddWithValue("@DataRegistration", clientServer.DataRegistration);
                command.Parameters.AddWithValue("@Subscription", "Подписан");
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new MySqlException();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
        }

        /// <summary>
        /// Добавление языков клиента и уровня их владения
        /// </summary>
        /// <param name="clientServer">Объект клиента</param>
        public void AddLevelLanguages(ClientServerDB clientServer)
        {
            foreach (LevelLanguageSDB ll in clientServer.LevelLanguages)
            {
                AddLevelLanguage(clientServer, ll);
            }
        }

        /// <summary>
        /// Добавление языка и уровня владения
        /// </summary>
        /// <param name="clientServer">Объект клиента</param>
        /// <param name="ll">Объект языка и уровня владения</param>
        public void AddLevelLanguage(ClientServerDB clientServer, LevelLanguageSDB ll)
        {
            string sqlCommand = "INSERT INTO levellanguages (AddressClient, Language, Level) " +
                "VALUES (@AddressClient, @Language, @Level)";
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand command = new MySqlCommand(sqlCommand, ConnectionMySQL);
                command.Parameters.AddWithValue("@AddressClient", clientServer.Address);
                command.Parameters.AddWithValue("@Language", (int)ll.Language);
                command.Parameters.AddWithValue("@Level", ll.Level);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new MySqlException();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
        }

        /// <summary>
        /// Изменение статуса подписки клиента на "Отписан"
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void DeleteClient(string address)
        {
            ChangeSubscription(address, false);
        }

        /// <summary>
        /// Изменение статуса подписки клиента на "Подписан"
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void SetSubscriptionOn(string address)
        {
            ChangeSubscription(address, true);
        }

        /// <summary>
        /// Изменение статуса подписки
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <param name="subscrition">true-"Подписан",false-"Отписан"</param>
        public void ChangeSubscription(string address, bool subscrition)
        {
            string subscriptionValue = subscrition ? "Подписан" : "Отписан";
            string sqlCommand = "UPDATE clients SET Subscription = @Subscription WHERE Address = @Address";
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand command = new MySqlCommand(sqlCommand, ConnectionMySQL);
                command.Parameters.AddWithValue("@Address", address);
                command.Parameters.AddWithValue("@Subscription", subscriptionValue);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new MySqlException();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
        }

        /// <summary>
        /// Добавление книги
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <param name="bookJSON">Книга в JSON</param>
        /// <param name="dataGetting">Дата-время получения книги</param>
        public void AddBook(string address, string bookJSON, DateTime dataGetting)
        {
            BookSDB bookObject = JsonSerializer.Deserialize<BookSDB>(bookJSON);
            string sqlCommand = "INSERT INTO books (AddressClient, Language, Name, Pages, DataGetting) " +
                "VALUES(@AddressClient, @Language, @Name, @Pages, @DataGetting); ";
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand command = new MySqlCommand(sqlCommand, ConnectionMySQL);
                command.Parameters.AddWithValue("@AddressClient", address);
                command.Parameters.AddWithValue("@Language", (int)bookObject.Language);
                command.Parameters.AddWithValue("@Name", bookObject.Name);
                command.Parameters.AddWithValue("@Pages", bookObject.Pages);
                command.Parameters.AddWithValue("@DataGetting", dataGetting);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
        }

        /// <summary>
        /// Запись даты-времени прочтения активной книги
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <param name="dateTime">Дата-время прочтения книги</param>
        public void ImplementationBook(string address, DateTime dateTime)
        {
            var idBook = GetIdUnreadBook(address);
            if (idBook != null)
            {
                AddDataReadingBook(idBook, dateTime);
            }
        }

        /// <summary>
        /// Запись даты-времени прочтения книги
        /// </summary>
        /// <param name="idBook">ID книги</param>
        /// <param name="dateTime">Дата-время прочтения книги</param>
        public void AddDataReadingBook(object idBook, DateTime dateTime)
        {
            string sqlCommand = "UPDATE books SET DataReading = @DataReading WHERE Id = @Id";
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand Command = new MySqlCommand(sqlCommand, ConnectionMySQL);
                Command.Parameters.AddWithValue("@DataReading", dateTime);
                Command.Parameters.AddWithValue("@Id", idBook);
                Command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
        }

        /// <summary>
        /// Удаление последней не прочитаной книги
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void DeleteLastBook(string address)
        {
            var idBook = GetIdUnreadBook(address);
            if (idBook != null)
            {
                DeleteBook(idBook);
            }           
        }
        /// <summary>
        /// Возвращение Id последней не прочитаной книги
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <returns>Id последней не прочитаной книги</returns>
        public object GetIdUnreadBook(string address)
        {
            string sqlCommand = "SELECT MAX(Id) FROM books WHERE AddressClient = @Address AND DataReading IS NULL";
            object idBook = null;
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand Command = new MySqlCommand(sqlCommand, ConnectionMySQL);
                Command.Parameters.AddWithValue("@Address", address);
                idBook = Command.ExecuteScalar();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
            return idBook;
        }

        /// <summary>
        /// Удаление книги
        /// </summary>
        /// <param name="idBook">Id книги</param>
        public void DeleteBook(object idBook)
        {
            string sqlCommand = "DELETE FROM books WHERE Id = @IdBook";
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand command = new MySqlCommand(sqlCommand, ConnectionMySQL);
                command.Parameters.AddWithValue("@IdBook", idBook);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
        }

        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        public void FullDeleteClient(string address)
        {
            string sqlCommand = "DELETE FROM clients WHERE Address = @Address";
            try
            {
                ConnectionMySQL.Open();
                MySqlCommand command = new MySqlCommand(sqlCommand, ConnectionMySQL);
                command.Parameters.AddWithValue("@Address", address);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
            finally
            {
                ConnectionMySQL.Close();
            }
        }

        /// <summary>
        /// Завершение приложения
        /// </summary>
        public void StopApplication()
        {
            ConnectionMySQL.Close();
            ServerDB.StopServer();
        }
    }
}
