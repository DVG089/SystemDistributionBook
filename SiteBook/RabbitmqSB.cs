
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteBook
{
    /// <summary>
    /// Базовый класс работы с RabbitMQ
    /// </summary>
    public class RabbitmqSB
    {
        /// <summary>
        /// Соединение с RabbitMQ
        /// </summary>
        protected IConnection ConnectionRabbit;
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Канал связи для передачи книг
        /// </summary>
        protected IModel ChannelBook;
        /// <summary>
        /// Канал связи для передачи клиентов
        /// </summary>
        protected IModel ChannelClient;
        /// <summary>
        /// Свойство сообщения
        /// </summary>
        private IBasicProperties PropetyClient;
        /// <summary>
        /// Название точки распределения книг
        /// </summary>
        public string ExchangeBook;
        /// <summary>
        /// Название точки распределения клиентов
        /// </summary>
        public string ExchangeClient;
        /// <summary>
        /// Название очереди книг
        /// </summary>
        public string QueueBook;
        /// <summary>
        /// Название маршрутизации очереди книг
        /// </summary>
        public string RoutingBook;
        /// <summary>
        /// Название очереди клиентов
        /// </summary>
        public string QueueClient;
        /// <summary>
        /// Название маршрутизации очереди клиентов
        /// </summary>
        public string RoutingClient;
        /// <summary>
        /// Название типа свойства добавления клиента
        /// </summary>
        public string PropetyAdding;
        /// <summary>
        /// Название типа свойства удаления клиента
        /// </summary>
        public string PropetyDeleted;

        /// <summary>
        /// Конструктор объекта работы с RabbitMQ
        /// </summary>
        public RabbitmqSB()
        {
            try
            {
                IniInitializationComponent();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
        }

        /// <summary>
        /// Инициализация компонентов
        /// </summary>
        protected void IniInitializationComponent()
        {
            InitializationStringComponentSB();
            InitializationConnectionRabbitSB();

            ChannelBook = ConnectionRabbit.CreateModel();
            ChannelBook.ExchangeDeclare(ExchangeBook, ExchangeType.Direct);
            ChannelBook.QueueDeclare(QueueBook, true, false, false, null);
            ChannelBook.QueueBind(QueueBook, ExchangeBook, RoutingBook, null);

            ChannelClient = ConnectionRabbit.CreateModel();
            ChannelClient.ExchangeDeclare(ExchangeClient, ExchangeType.Direct);
            ChannelClient.QueueDeclare(QueueClient, true, false, false, null);
            ChannelClient.QueueBind(QueueClient, ExchangeClient, RoutingClient, null);

            PropetyClient = ChannelClient.CreateBasicProperties();
        }

        /// <summary>
        /// Инициализация строковых переменных
        /// </summary>
        private void InitializationStringComponentSB()
        {
            ExchangeBook = "BookSDB";
            ExchangeClient = "ClientSDB";
            QueueBook = "BookQueue";
            RoutingBook = "Book";
            QueueClient = "ClientQueue";
            RoutingClient = "Client";
            PropetyAdding = "Adding";
            PropetyDeleted = "Deleted";
        }

        /// <summary>
        /// Инициализация подключения к RabbitMQ
        /// </summary>
        private void InitializationConnectionRabbitSB()
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.Uri = new Uri(ConfigurationManager.ConnectionStrings["RabbitMQ"].ConnectionString);
                ConnectionRabbit = factory.CreateConnection();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
        }

        /// <summary>
        /// Отправка сообщения в очередь книг
        /// </summary>
        /// <param name="bookJSON">Книга в JSON</param>
        public void PublishBookQueue(string bookJSON)
        {
            try
            {
                byte[] bookBytes = Encoding.UTF8.GetBytes(bookJSON);
                ChannelBook.BasicPublish(ExchangeBook, RoutingBook, null, bookBytes);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new RabbitmqException();
            }
        }

        /// <summary>
        /// Отправка сообщения в очередь клиентов
        /// </summary>
        /// <param name="clientJSON">Клиент в JSON</param>
        /// <param name="propety">Тип свойстава сообщения</param>
        public void PublishClientQueue(string clientJSON, string propety)
        {
            try
            {
                PropetyClient.Type = propety;
                byte[] clientBytes = Encoding.UTF8.GetBytes(clientJSON);
                ChannelClient.BasicPublish(ExchangeClient, RoutingClient, PropetyClient, clientBytes);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (ConnectionRabbit != null) ConnectionRabbit.Close();
            if (ChannelClient != null) ChannelClient.Close();
            if (ChannelBook != null) ChannelBook.Close();
        }

        /// <summary>
        /// Завершение приложения
        /// </summary>
        public void StopApplication()
        {
            Dispose();
            Environment.Exit(0);
        }
    }
}
