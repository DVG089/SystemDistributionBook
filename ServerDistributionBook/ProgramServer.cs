using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDistributionBook
{
    class ProgramServer
    {
        static void Main(string[] args)
        {
            string executionResult = "Успешно завершено";
            ControlServerDB serverDB = null;
            try
            {

                int daysSecond = ReadDaysSecond();
                serverDB = new ControlServerDB(daysSecond);
                if (serverDB.MongoClientsAvailability())
                {
                    serverDB.ObjectOverload();
                }
                serverDB.StartListeningQueuesRabbit();
                Console.WriteLine("Сервер запущен. Нажмите любую клавишу для заершения работы.");
                Console.ReadKey();
            }

            catch (System.TimeoutException)
            {
                executionResult = "Ошибка соединения с MongoDB";
            }
            catch (MongoDB.Driver.MongoConnectionException)
            {
                executionResult = "Ошибка соединения с MongoDB";
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                executionResult = "Ошибка соединения с MySQL";
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
            {
                executionResult = "Ошибка соединения с RabbitMQ";
            }
            catch (System.IO.IOException)
            {
                executionResult = "Ошибка соединения с RabbitMQ";
            }
            catch (Exception exception)
            {
                executionResult = exception.Message;
            }
            finally
            {
                if (serverDB != null)
                {
                    serverDB.StopServer(executionResult);
                }
                else
                {
                    ControlServerDB.WriteExecutionResult(executionResult);
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Возвращение секундного эквивалента дня из консоли
        /// </summary>
        /// <returns>Секундный эквивалент дня</returns>
        private static int ReadDaysSecond()
        {
            int daysSecond;
            while (true)
            {
                Console.WriteLine("Введите секундный эквивалент дня");
                if (Int32.TryParse(Console.ReadLine(), out daysSecond) && daysSecond >= 0)
                    break;
                else
                    Console.WriteLine("Не верно введено значение");
            }
            return daysSecond;
        }
    }
}
