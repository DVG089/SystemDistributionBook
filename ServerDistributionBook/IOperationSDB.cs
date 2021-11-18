using SiteBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDistributionBook
{
    /// <summary>
    /// Интерфейс операций сервера
    /// </summary>
    internal interface IOperationSDB
    {
        void AddClient(ClientServerDB clientServer);
        void DeleteClient(string address);
        void AddBook(string address, string bookJSON, DateTime dateTime);
    }
}
