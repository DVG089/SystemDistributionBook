
using MongoDB.Bson.Serialization.Attributes;
using SiteBook;
using System;
using System.Collections.Generic;


namespace ServerDistributionBook
{
    /// <summary>
    /// Класс клиента сервера
    /// </summary>
    [BsonIgnoreExtraElements]
    class ClientServerDB : ClientSDB, ICloneable
    {
        /// <summary>
        /// Очередь книг на чтение
        /// </summary>
        public Queue<string> QueueBook { get; set; }
        /// <summary>
        /// Дата-время освобождения от чтения
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TimeRead { get; set; }
        /// <summary>
        /// Дата-время освобождения от активной книги
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TimeReadActive { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public ClientServerDB()
        { }
        /// <summary>
        /// Конструктор объекта клиента сервера
        /// </summary>
        /// <param name="clientSDB">Объект клиента </param>
        public ClientServerDB(ClientSDB clientSDB)
        {
            Surname = clientSDB.Surname;
            Name = clientSDB.Name;
            Address = clientSDB.Address;
            PagesPerDay = clientSDB.PagesPerDay;
            ReadingIntervalActive = clientSDB.ReadingIntervalActive;
            ReadingIntervalPassive = clientSDB.ReadingIntervalPassive;
            LevelLanguages = clientSDB.LevelLanguages;
            DataRegistration = clientSDB.DataRegistration;
            QueueBook = new Queue<string>();
            TimeReadActive = DateTime.Now;
            TimeRead = DateTime.Now;
        }

        /// <summary>
        /// Создание поверхностной копии объекта
        /// </summary>
        /// <returns>Поверхностная копии объекта</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
