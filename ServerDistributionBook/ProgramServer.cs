using NLog;
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
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
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
            catch (Exception exception)
            {
                if (!ControlServerDB.CheckException(exception))
                {
                    Log.Error(exception.ToString);
                }
            }
            finally
            {
                if (serverDB != null)
                {
                    serverDB.StopServer();
                }
                else
                {
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
