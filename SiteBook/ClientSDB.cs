using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteBook
{
    /// <summary>
    /// Класс клиента
    /// </summary>
    public class ClientSDB
    {
        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Электронный адресс доставки книги
        /// </summary>
        [BsonId]
        public string Address { get; set; }
        /// <summary>
        /// Количество читаемых страниц в день
        /// </summary>
        public int PagesPerDay { get; set; }
        /// <summary>
        /// Количество дней чтения подряд
        /// </summary>
        public int ReadingIntervalActive { get; set; }
        /// <summary>
        /// Количество дней отдыха
        /// </summary>
        public int ReadingIntervalPassive { get; set; }
        /// <summary>
        /// Дата регистрации подписки клиента
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DataRegistration { get; set; }
        /// <summary>
        /// Языки и уровень владения
        /// </summary>
        public LevelLanguageSDB[] LevelLanguages { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public ClientSDB()
        {
        }
        /// <summary>
        /// Конструктор обЪекта клиента
        /// </summary>
        /// <param name="surname">Фамилия</param>
        /// <param name="name">Имя</param>
        /// <param name="address">Электронный адресс</param>
        /// <param name="pagesPerDay">Количество читаемых страниц в день</param>
        /// <param name="readingIntervalActive">Количество дней чтения подряд</param>
        /// <param name="readingIntervalPassive">Количество дней отдыха</param>
        /// <param name="levelLanguages">Языки и уровень владения</param>
        public ClientSDB(string surname, string name, string address,
            int pagesPerDay, int readingIntervalActive, int readingIntervalPassive, LevelLanguageSDB[] levelLanguages)
        {
            Surname = surname;
            Name = name;
            Address = address;
            PagesPerDay = pagesPerDay;
            ReadingIntervalActive = readingIntervalActive;
            ReadingIntervalPassive = readingIntervalPassive;
            LevelLanguages = levelLanguages;
            DataRegistration = DateTime.UtcNow.ToLocalTime();
        }
    }
}
