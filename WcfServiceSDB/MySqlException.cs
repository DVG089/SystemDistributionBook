using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfServiceSDB
{
    /// <summary>
    /// Класс ошибки работы с MySQL
    /// </summary>
    [DataContract]
    public class MySqlException
    {
        private string message;

        public MySqlException()
        {
            message = "Ошибка при работе с базой данных";
        }

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}