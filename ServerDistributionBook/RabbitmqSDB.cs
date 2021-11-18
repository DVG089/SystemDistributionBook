using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SiteBook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServerDistributionBook
{
    /// <summary>
    /// Класс работы с RabbitMQ
    /// </summary>
    internal class RabbitmqSDB : RabbitmqSB, IDisposable
    {
        /// <summary>
        /// Объект управления сервером
        /// </summary>
        private ControlServerDB ServerDB;
        /// <summary>
        /// Прослушиватель очереди книг
        /// </summary>
        private EventingBasicConsumer ConsumerBook;
        /// <summary>
        /// Прослушиватель очереди клиентов
        /// </summary>
        private EventingBasicConsumer ConsumerClient;
        /// <summary>
        /// Название очереди нераспределённых книг
        /// </summary>
        public string QueueUnallocated;
        /// <summary>
        /// Название маршрутизации очереди нераспределённых книг
        /// </summary>
        public string RoutingUnallocated;

        /// <summary>
        /// Конструктор объекта работы с RabbitMQ
        /// </summary>
        /// <param name="serverDB">Объект управления сервером</param>
        public RabbitmqSDB(ControlServerDB serverDB)
            : base()
        {
            ServerDB = serverDB;
            InitializationStringComponentSDB();

            try
            {
                ChannelBook.QueueDeclare(QueueUnallocated, true, false, false, null);
                ChannelBook.QueueBind(QueueUnallocated, ExchangeBook, RoutingUnallocated, null);

                ConsumerBook = new EventingBasicConsumer(ChannelBook);
                ConsumerClient = new EventingBasicConsumer(ChannelClient);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }

            InitializationMethodsConsumers();
        }

        /// <summary>
        /// Инициализация строковых переменных
        /// </summary>
        private void InitializationStringComponentSDB()
        {
            QueueUnallocated = "UnallocatedBookQueue";
            RoutingUnallocated = "UnallocatedBook";
        }

        /// <summary>
        /// Инициализация методов прослушивателей очередей
        /// </summary>
        private void InitializationMethodsConsumers()
        {
            ConsumerClient.Received += ServerDB.AddDeleteClientFromRabbitMQ;
            ConsumerBook.Received += ServerDB.DistributionBookRequest;
        }

        /// <summary>
        /// Перераспределение сообщений в очередях
        /// </summary>
        /// <param name="queueOf">Название очереди, из которой берутся сообщения</param>
        /// <param name="exchange">Точка распределения</param>
        /// <param name="routingQueueIn">Название маршрутизации очереди, в которую отправляются сообщения</param>
        public void RedistributionQueue(string queueOf, string exchange, string routingQueueIn)
        {
            IModel channel = null;
            try
            {
                channel = ConnectionRabbit.CreateModel();
                while (true)
                {
                    BasicGetResult result = channel.BasicGet(queueOf, false);
                    if (result != null)
                    {
                        channel.BasicPublish(exchange, routingQueueIn, null, result.Body);
                        channel.BasicAck(result.DeliveryTag, false);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new RabbitmqException();
            }
            finally
            {
                if (channel != null) channel.Close();
            }
        }

        /// <summary>
        /// Отправка сообщения в очередь нераспределенных книг
        /// </summary>
        /// <param name="e">Сообщение RabbitMQ</param>
        public void PublishUnallocatedQueue(BasicDeliverEventArgs e)
        {
            try
            {
                ChannelBook.BasicPublish(ExchangeBook, RoutingUnallocated, null, e.Body);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
        }

        /// <summary>
        /// Перераспределение сообщений в очередях
        /// </summary>
        /// <param name="queueOf">Очередь, из которой берутся сообщения</param>
        /// <param name="exchange">Точка распределения</param>
        /// <param name="routingQueueIn">Название маршрутизации очереди, в которую отправляются сообщения</param>
        public void RedistributionQueue(Queue<string> queueOf, string exchange, string routingQueueIn)
        {
            IModel channel = null;
            try
            {
                channel = ConnectionRabbit.CreateModel();
                while (true)
                {
                    if (queueOf.Count != 0)
                    {
                        string book = queueOf.Peek();
                        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(book);
                        channel.BasicPublish(exchange, routingQueueIn, null, messageBodyBytes);
                        queueOf.Dequeue();
                    }
                    else break;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new RabbitmqException();
            }
            finally
            {
                if (channel != null) channel.Close();
            }
        }

        /// <summary>
        /// Преобразование сообщения RabbitMQ
        /// </summary>
        /// <typeparam name="T">Тип преобразования</typeparam>
        /// <param name="body">Сообщение RabbitMQ</param>
        /// <returns>Преобразованое сообщение</returns>
        public T ConvertMessageRabbit<T>(byte[] body) where T : class
        {
            string message = Encoding.UTF8.GetString(body);
            if (!(message is T objectReturns))
            {
                objectReturns = ConvertJsonRabbit<T>(message);
            }
            return objectReturns;
        }

        /// <summary>
        /// Преобразование сообщения JSON
        /// </summary>
        /// <typeparam name="T">Тип преобразования</typeparam>
        /// <param name="json">Сообщение JSON</param>
        /// <returns>Преобразованое сообщение</returns>
        public T ConvertJsonRabbit<T>(string json) where T : class
        {
            T objectReturns = JsonSerializer.Deserialize<T>(json);
            return objectReturns;
        }

        /// <summary>
        /// Запуск прослушивания очередей
        /// </summary>
        public void StartListeningQueues()
        {
            try
            {
                ChannelClient.BasicConsume(QueueClient, false, ConsumerClient);
                ChannelBook.BasicConsume(QueueBook, false, ConsumerBook);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                StopApplication();
            }
        }

        /// <summary>
        /// Подтверждение получения сообщения RabbitMQ
        /// </summary>
        /// <param name="sender">Объект получения</param>
        /// <param name="e">Сообщение RabbitMQ</param>
        public void BasicAckRabbit(object sender, BasicDeliverEventArgs e)
        {
            IModel channel;
            try
            {
                if (sender == ConsumerBook)
                {
                    channel = ChannelBook;
                }
                else
                {
                    channel = ChannelClient;
                }
                channel.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
                throw new RabbitmqException();
            }
        }
    }
}
