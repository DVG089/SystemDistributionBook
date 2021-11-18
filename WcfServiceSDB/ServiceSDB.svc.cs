using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using NLog;

namespace WcfServiceSDB
{
    public class ServiceSDB : IServiceSDB
    {
        /// <summary>
        /// Соединение с MySQL
        /// </summary>
        private MySqlConnection ConnectionMySQL { get; set; }
        /// <summary>
        /// Команда MySQL
        /// </summary>
        private MySqlCommand Command { get; set; }
        /// <summary>
        /// Объект фильтра
        /// </summary>
        private FilterSDB Filter { get; set; }
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Электронный адресс клиента
        /// </summary>
        private string Address { get; set; }
        /// <summary>
        /// Список используемых языков
        /// </summary>
        private List<string> listLanguage { get; set; }
        /// <summary>
        /// Статус чтения книги:Прочитана
        /// </summary>
        private string ReadEnd { get; set; }
        /// <summary>
        /// Статус чтения книги:Читается
        /// </summary>
        private string ReadNow { get; set; }
        /// <summary>
        /// Статус подписки:Подписан
        /// </summary>
        private string SubscriptionOn { get; set; }
        /// <summary>
        /// Статус подписки:Отписан
        /// </summary>
        private string SubscriptionOff { get; set; }

        /// <summary>
        /// Конструтор объекта
        /// </summary>
        public ServiceSDB()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
            ConnectionMySQL = new MySqlConnection(connectionString);
            Command = null;
            Filter = null;
            Address = null;
            listLanguage = new List<string> { "Русский", "Английский", "Немецкий", "Итальянский", "Испанский" };
            ReadEnd = "Прочитана";
            ReadNow = "Читается";
            SubscriptionOn = "Подписан";
            SubscriptionOff = "Отписан";
        }
        /// <summary>
        /// Возвращает информацию о клиенте в 2 таблицах
        /// </summary>
        /// <param name="address">Электронный адресс клиента</param>
        /// <returns>Информация о клиенте</returns>
        public DataSet GetClientInfo(string address)
        {
            string sql;
            DataSet dataSetClient = new DataSet();
            dataSetClient.Tables.Add();
            dataSetClient.Tables.Add();
            for (int i = 0; i <= 1; i++)
            {
                if (i == 0)
                {
                    sql = "SELECT * FROM clients WHERE Address = @Address";
                }
                else 
                { 
                    sql = "SELECT * FROM LevelLanguages WHERE AddressClient = @Address"; 
                }
                try
                {
                    ConnectionMySQL.Open();
                    Command = new MySqlCommand(sql, ConnectionMySQL);
                    Command.Parameters.AddWithValue("@Address", address);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(Command);
                    adapter.Fill(dataSetClient.Tables[i]);
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString);
                    throw new FaultException<MySqlException>(new MySqlException());
                }
                finally
                {
                    ConnectionMySQL.Close();
                }
            }
            return dataSetClient;
        }
        /// <summary>
        /// Возвращает информацию о книгах
        /// </summary>
        /// <param name="filter">Объект фильтра</param>
        /// <param name="address">Электронный адресс клиента</param>
        /// <returns>Информация о книгах</returns>
        public DataSet GetBook(FilterSDB filter, string address)
        {
            DataSet dataSetBook = new DataSet();
            Command = new MySqlCommand();
            Filter = filter;
            Address = address;
            string sqlWhere = "";
            AddConditionBook(ref sqlWhere);

            Command.CommandText = $"SELECT * FROM books{sqlWhere}";
            try 
            {
                ConnectionMySQL.Open();
                Command.Connection = ConnectionMySQL;
                MySqlDataAdapter adapter = new MySqlDataAdapter(Command);
                adapter.Fill(dataSetBook);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString);
                throw new FaultException<MySqlException>(new MySqlException());
            }
            finally
            {
                ConnectionMySQL.Close();
            }
            return dataSetBook;
        }

        /// <summary>
        /// Добавление условия по фильтру книг
        /// </summary>
        /// <param name="sqlWhere">Предыдущее условие</param>
        private void AddConditionBook(ref string sqlWhere)
        {
            AddStatusReadingCondinion(ref sqlWhere, Filter.Status);
            AddLanguagesCondinion(ref sqlWhere, "=", "Language", Filter.Language);
            AddNameBookCondition(ref sqlWhere, "=", "Name", Filter.ClientBook);
            AddAddressCondition(ref sqlWhere, "=", "AddressClient", Address);
            AddStartPeriodCondition(ref sqlWhere, ">", "DataGetting", Filter.StartPeriod);
        }

        /// <summary>
        /// Возвращает статистику клиента
        /// </summary>
        /// <param name="filter">Объект фильтра</param>
        /// <returns>Статистика клиента</returns>
        public DataSet GetClientStatistics(FilterSDB filter)
        {
            DataSet dataSetBook = new DataSet();
            Command = new MySqlCommand();
            Filter = filter;
            string sqlWhereBook = "";
            AddConditionClientStatisticsByBook(ref sqlWhereBook);
            string sqlWhereClient = "";
            AddConditionClientStatistics(ref sqlWhereClient);

            Command.CommandText = "SELECT Address, COUNT(b.Name) AS CountBook, SUM(Pages) AS CountPages FROM " +
                    "(SELECT DISTINCT Address FROM clients AS c " +
                    "JOIN levellanguages AS ll " +
                    "ON c.Address = ll.AddressClient" +
                    $"{sqlWhereClient}) AS Addr " +
                    "LEFT JOIN (SELECT * FROM books" +
                    $"{sqlWhereBook}) AS b " +
                    "ON Addr.Address = b.AddressClient " +
                    "GROUP BY Address";
            try 
            { 
                ConnectionMySQL.Open();
                Command.Connection = ConnectionMySQL;
                MySqlDataAdapter adapter = new MySqlDataAdapter(Command);
                adapter.Fill(dataSetBook);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString);
                throw new FaultException<MySqlException>(new MySqlException());
            }
            finally
            {
                ConnectionMySQL.Close();
            }
            return dataSetBook;
        }

        /// <summary>
        /// Добавление условия по фильтру клиентов для книг
        /// </summary>
        /// <param name="sqlWhereBook">Предыдущее условие</param>
        private void AddConditionClientStatisticsByBook(ref string sqlWhereBook)
        {
            AddLanguagesCondinion(ref sqlWhereBook, "=", "Language", Filter.Language);
            AddStartPeriodCondition(ref sqlWhereBook, ">", "DataGetting", Filter.StartPeriod);
        }

        /// <summary>
        /// Добавление условия по фильтру клиентов
        /// </summary>
        /// <param name="sqlWhereClient">Предыдущее условие</param>
        private void AddConditionClientStatistics(ref string sqlWhereClient)
        {
            AddSubscriptionCondition(ref sqlWhereClient, "=", "c.Subscription", Filter.Status);
            AddLanguagesCondinion(ref sqlWhereClient, "=", "ll.Language", Filter.Language);
            AddAddressCondition(ref sqlWhereClient, "=", "c.Address", Filter.ClientBook);
        }

        /// <summary>
        /// Добавление условия на прочитанность книги
        /// </summary>
        /// <param name="sqlWhere">Предыдущее условие</param>
        /// <param name="statusReading">Статус прочтения</param>
        private void AddStatusReadingCondinion(ref string sqlWhere, string statusReading)
        {
            if (statusReading ==  ReadEnd|| statusReading == ReadNow)
            {
                string sqlConnectionWord = GetConnectionWord(sqlWhere);
                string sqlStatusReadingCondition;
                if (statusReading == ReadEnd)
                {
                    sqlStatusReadingCondition = " DataReading IS NOT NULL";
                }
                else
                {
                    sqlStatusReadingCondition = " DataReading IS NULL";
                }
                sqlWhere = $"{sqlWhere}{sqlConnectionWord}{sqlStatusReadingCondition}";
            }
        }

        /// <summary>
        /// Добавление условия на язык
        /// </summary>
        /// <param name="sqlWhere">Предыдущее условие</param>
        /// <param name="command">Sql-комманда</param>
        /// <param name="sqlColumn">Название столбца базы даных</param>
        /// <param name="language">Язык</param>
        private void AddLanguagesCondinion(ref string sqlWhere, string operation, string sqlColumn, string language)
        {
            foreach (string lang in listLanguage)
            {
                if (language == lang)
                {
                    GetFullCondition(ref sqlWhere, operation, sqlColumn, language);
                }
            }
        }

        /// <summary>
        /// Добавление условия на название книги
        /// </summary>
        /// <param name="sqlWhere">Предыдущее условие</param>
        /// <param name="operation">Sql-комманда</param>
        /// <param name="sqlColumn">Название столбца базы даных</param>
        /// <param name="nameBook">Название книги</param>
        private void AddNameBookCondition(ref string sqlWhere, string operation, string sqlColumn, string nameBook)
        {
            if (nameBook != "" && nameBook != null)
            {
                GetFullCondition(ref sqlWhere, operation, sqlColumn, nameBook);
            }
        }

        /// <summary>
        /// Добавление условия на электронный адресс клиента
        /// </summary>
        /// <param name="sqlWhere">Начальное условие</param>
        /// <param name="operation">Sql-комманда</param>
        /// <param name="sqlColumn">Название столбца базы даных</param>
        /// <param name="address">Электронный адресс клиента</param>
        private void AddAddressCondition(ref string sqlWhere, string operation, string sqlColumn, string address)
        {
            if (address != "" && address != null)
            {
                GetFullCondition(ref sqlWhere, operation, sqlColumn, address);
            }
        }

        /// <summary>
        /// Добавление условия на подписку клиента
        /// </summary>
        /// <param name="sqlWhere">Предыдущее условие</param>
        /// <param name="operation">Sql-комманда</param>
        /// <param name="sqlColumn">Название столбца базы даных</param>
        /// <param name="subscription">Статус подписки клиента</param>
        private void AddSubscriptionCondition(ref string sqlWhere, string operation, string sqlColumn, string subscription)
        {
            if (subscription == SubscriptionOn || subscription == SubscriptionOff)
            {
                GetFullCondition(ref sqlWhere, operation, sqlColumn, subscription);
            }
        }

        /// <summary>
        /// Добавление условия на подписку клиента
        /// </summary>
        /// <param name="sqlWhere">Предыдущее условие</param>
        /// <param name="operation">Sql-комманда</param>
        /// <param name="sqlColumn">Название столбца базы даных</param>
        /// <param name="startPeriod">Дата-время начала периода фильтрации</param>
        private void AddStartPeriodCondition(ref string sqlWhere, string operation, string sqlColumn, DateTime? startPeriod)
        {
            if (startPeriod != null)
            {
                GetFullCondition(ref sqlWhere, operation, sqlColumn, startPeriod);
            }
        }

        /// <summary>
        /// Возвращение полного условия
        /// </summary>
        /// <param name="sqlWhere">Предыдущее условие</param>
        /// <param name="operation">Операция сравнения</param>
        /// <param name="sqlColumn">Название столбца базы даных, первый операнд</param>
        /// <param name="value">Значение, второй операнд</param>
        private void GetFullCondition(ref string sqlWhere, string operation, string sqlColumn, object value)
        {
            string sqlConnectionWord = GetConnectionWord(sqlWhere);
            string sqlCondition = GetCondition(operation, sqlColumn, value);
            sqlWhere = $"{sqlWhere}{sqlConnectionWord}{sqlCondition}";
        }

        /// <summary>
        /// Взвращение условия
        /// </summary>
        /// <param name="operation">Операция сравнения</param>
        /// <param name="sqlColumn">Название столбца базы даных, первый операнд</param>
        /// <param name="value">Значение, второй операнд</param>
        /// <returns></returns>
        private string GetCondition(string operation, string sqlColumn, object value)
        {
            string parameter = $"@{sqlColumn}";
            string equalCondition = $" {sqlColumn} {operation} {parameter}";
            Command.Parameters.AddWithValue(parameter, value);
            return equalCondition;
        }

        /// <summary>
        /// Возвращение соединительного слова
        /// </summary>
        /// <param name="sqlWhere">Предыдущее условие</param>
        /// <returns></returns>
        private string GetConnectionWord(string sqlWhere)
        {
            string sqlConnectionWord;
            if (sqlWhere == "" || sqlWhere == null)
            {
                sqlConnectionWord = " WHERE";
            }
            else
            {
                sqlConnectionWord = " AND";
            }
            return sqlConnectionWord;
        }
    }
}
