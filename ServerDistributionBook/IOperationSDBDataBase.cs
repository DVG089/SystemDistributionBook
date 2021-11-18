using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDistributionBook
{
    /// <summary>
    /// Интерфейс операций сервера для баз данных
    /// </summary>
    internal interface IOperationSDBDataBase: IOperationSDB
    {
        void ImplementationBook(string address, DateTime dateTime);
        void DeleteLastBook(string address);
    }
}
