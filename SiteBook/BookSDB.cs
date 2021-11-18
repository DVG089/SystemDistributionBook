using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteBook
{
    /// <summary>
    /// Класс книги
    /// </summary>
    public class BookSDB
    {
        public enum languageEnum { Russian = 1, English, Deutsch, Italian, Spanish }
        /// <summary>
        /// Язык книги
        /// </summary>
        public languageEnum Language { get; set; }
        /// <summary>
        /// Название книги
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Количество страниц книги
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public BookSDB()
        {
        }

        /// <summary>
        /// Конструктор объекта книги
        /// </summary>
        /// <param name="language">Язык книги</param>
        /// <param name="pages">Количество страниц книги</param>
        /// <param name="name">Название книги</param>
        public BookSDB(languageEnum language, int pages, string name)
        {
            Language = language;
            Pages = pages;
            Name = name;
        }

    }
}
