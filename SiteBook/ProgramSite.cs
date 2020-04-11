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

namespace SiteBook
{
    class ProgramSite
    {
        static void Main(string[] args)
        {
            string executionResult = "Успешно завершено";
            CommandConsoleSB commandConsole = null;
            try
            {
                commandConsole = new CommandConsoleSB();
                commandConsole.ExecutionCommand();
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
                if (commandConsole != null)
                {
                    commandConsole.StopConsole(executionResult);
                }
                else
                {
                    CommandConsoleSB.WriteExecutionResult(executionResult);
                    Environment.Exit(0);
                }
            }
        }
    }
}
