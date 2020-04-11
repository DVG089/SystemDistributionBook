using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfServiceSDB
{
    /// <summary>
    /// Класс фильтр запроса
    /// </summary>
    [DataContract]
    public class FilterSDB
    {
        /// <summary>
        /// Фильтр по статус подписки (Подписан, Отписан) или  статусу чтения (Читается, Прочитана)
        /// </summary>
        [DataMember]
        public string Status { get; set; }
        /// <summary>
        /// Фильтр по языку
        /// </summary>
        [DataMember]
        public string Language { get; set; }
        /// <summary>
        /// Фильтр по электронному адрессу клиента или имени книги
        /// </summary>
        [DataMember]
        public string ClientBook { get; set; }
        /// <summary>
        /// Дата-время начала периода фильтрации
        /// </summary>
        [DataMember]
        public DateTime? StartPeriod { get; set; }
    }
}