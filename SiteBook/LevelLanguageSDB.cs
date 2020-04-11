using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteBook
{
    /// <summary>
    /// Класс владения языком
    /// </summary>
    public class LevelLanguageSDB
    {
        /// <summary>
        /// Язык
        /// </summary>
        public BookSDB.languageEnum Language { get; set; }
        /// <summary>
        /// Уровень владения
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public LevelLanguageSDB()
        {
        }
        /// <summary>
        /// Конструктор объекта язык-уровень
        /// </summary>
        /// <param name="language">Язык</param>
        /// <param name="level">Уровень владения</param>
        public LevelLanguageSDB(BookSDB.languageEnum language, int level)
        {
            Language = language;
            Level = level;
        }
    }
}
