using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SiteBook
{
    /// <summary>
    /// Класс случайной генерации книг
    /// </summary>
    class RandomGenerationBook
    {
        /// <summary>
        /// Объект случайной генерации
        /// </summary>
        private Random Random;
        /// <summary>
        /// Журнал сообщений Nlog
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Объект, отправляющий сигнал отмены
        /// </summary>
        private CancellationTokenSource CancelTokenSource;
        /// <summary>
        /// Отменяющий токен
        /// </summary>
        private CancellationToken Token;
        /// <summary>
        /// Задача случайной генерации книг
        /// </summary>
        private Task TaskRandomGenerationBook;
        /// <summary>
        /// Объект работы с RabbitMQ
        /// </summary>
        private RabbitmqSB WorkRabbit;
        /// <summary>
        /// Объект команд консоли
        /// </summary>
        private CommandConsoleSB CommandConsole;
        /// <summary>
        /// Название книги
        /// </summary>
        private string NameBook { get; set; }
        /// <summary>
        /// Количество страниц книги
        /// </summary>
        private int Pages { get; set; }
        /// <summary>
        /// Язык книги
        /// </summary>
        private BookSDB.languageEnum Language { get; set; }
        /// <summary>
        /// Объект книги
        /// </summary>
        private BookSDB Book { get; set; }
        /// <summary>
        /// Минимальное значение интервала добавления книг
        /// </summary>
        private int MinInterval { get; set; }
        /// <summary>
        /// Максимальное значение интервала добавления книг
        /// </summary>
        private int MaxInterval { get; set; }
        /// <summary>
        /// Минимальная цифра в названии книги
        /// </summary>
        private int MinNumberName { get; set; }
        /// <summary>
        /// Максимальная цифра в названии книги не включительно
        /// </summary>
        private int MaxNumberName { get; set; }
        /// <summary>
        /// Минимальный номер языка книги
        /// </summary>
        private int MinNumberLanguage { get; set; }
        /// <summary>
        /// Максимальный номер языка книги не включительно
        /// </summary>
        private int MaxNumberLanguage { get; set; }
        /// <summary>
        /// Минимальное количество страниц книги
        /// </summary>
        private int MinNumberPages { get; set; }
        /// <summary>
        /// Максимальное количество страниц книги не включительно
        /// </summary>
        private int MaxNumberPages { get; set; }

        public RandomGenerationBook(RabbitmqSB workRabbit, int minInterval, int maxInterval)
        {
            Random = new Random();
            CancelTokenSource = null;
            WorkRabbit = workRabbit;
            TaskRandomGenerationBook = new Task(GenerationBook);
            MinNumberName = 1;
            MaxNumberName = 100000;
            MinNumberLanguage = 1;
            MaxNumberLanguage = 6;
            MinNumberPages = 5;
            MaxNumberPages = 1500;
            MinInterval = minInterval;
            MaxInterval = maxInterval;
        }

        /// <summary>
        /// Генерация случайных книг
        /// </summary>
        private void GenerationBook()
        {
            try
            {
                TryGenerationBook();
            }
            catch (Exception exception)
            {
                if (!(exception is RabbitmqException))
                {
                    Log.Error(exception.ToString);
                }
                CommandConsole.StopConsole();
            }
        }

        /// <summary>
        /// Генерация случайных книг без обработки исключений
        /// </summary>
        private void TryGenerationBook()
        {
            int MilisecondOfSecond = 1000;
            CancelTokenSource = new CancellationTokenSource();
            Token = CancelTokenSource.Token;
            while (!Token.IsCancellationRequested)
            {
                NameBook = "Book" + Random.Next(MinNumberName, MaxNumberName).ToString();
                Pages = Random.Next(MinNumberPages, MaxNumberPages);
                Language = (BookSDB.languageEnum)Random.Next(MinNumberLanguage, MaxNumberLanguage);
                Book = new BookSDB(Language, Pages, NameBook);
                int timeSleep = Random.Next(MinInterval, MaxInterval) * MilisecondOfSecond;
                Thread.Sleep(timeSleep);
                string bookJSON = JsonSerializer.Serialize<BookSDB>(Book);
                WorkRabbit.PublishBookQueue(bookJSON);
            }
            CancelTokenSource = null;
        }

        /// <summary>
        /// Запуск задачи случайной генерации книг
        /// </summary>
        public void StartGenerationBook()
        {
            TaskRandomGenerationBook.Start();
        }

        /// <summary>
        /// Остановка задачи случайной генерации книг
        /// </summary>
        public void StopGenerationBook()
        {
            if (CancelTokenSource != null)
            {
                CancelTokenSource.Cancel();
            }
        }
    }
}
