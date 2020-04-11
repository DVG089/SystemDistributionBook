using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDistributionBook
{
    /// <summary>
    /// Интерфейс проверки наличия клиента
    /// </summary>
    internal interface IClientAvailability
    {
        bool ClientAvailability(string address);
    }
}
