using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using NLog;

namespace SiteBook
{
    class ProgramSite
    {
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            CommandConsoleSB commandConsole = null;
            try
            {
                commandConsole = new CommandConsoleSB();
                commandConsole.ExecutionCommand();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString);
            }
            finally
            {
                if (commandConsole != null)
                {
                    commandConsole.StopConsole();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
