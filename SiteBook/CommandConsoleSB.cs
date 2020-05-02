using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiteBook
{
    /// <summary>
    /// Класс команд консоли
    /// </summary>
    internal class CommandConsoleSB
    {
        /// <summary>
        /// Объект работы с RabbitMQ
        /// </summary>
        private RabbitmqSB WorkRabbit;
        /// <summary>
        /// Объект класса случайной генерации книг
        /// </summary>
        private RandomGenerationBook GenerationBook;
        /// <summary>
        /// Команда добавления книг
        /// </summary>
        private KeyValuePair<string, string> CommandAddingBooks;
        /// <summary>
        /// Команда подписки клиента
        /// </summary>
        private KeyValuePair<string, string> CommandAddingClient;
        /// <summary>
        /// Команда отписки клиента
        /// </summary>
        private KeyValuePair<string, string> CommandDeletedClient;
        /// <summary>
        /// Команда выхода из приложения
        /// </summary>
        private KeyValuePair<string, string> CommandExit;
        /// <summary>
        /// Переменная, указывающая влючено ли добавление книг
        /// </summary>
        private bool AddingBooks { get; set; }
        /// <summary>
        /// Описание действия в консоли
        /// </summary>
        private string ActionDescription { get; set; }
        /// <summary>
        /// Описание ошибки действия
        /// </summary>
        private string ErrorDescription { get; set; }
        /// <summary>
        /// Регулярное выражение
        /// </summary>
        private string Regular { get; set; }


        /// <summary>
        /// Конструктор объекта команд консоли
        /// </summary>
        public CommandConsoleSB()
        {
            WorkRabbit = new RabbitmqSB();
            CommandAddingBooks = new KeyValuePair<string, string>("1", "Запуск добавления книг");
            CommandAddingClient = new KeyValuePair<string, string>("2", "Добавление клиента");
            CommandDeletedClient = new KeyValuePair<string, string>("3", "Удаление клиента");
            CommandExit = new KeyValuePair<string, string>("4", "Выход из приложения");
            AddingBooks = false;
        }

        /// <summary>
        /// Запуск ожидания команд
        /// </summary>
        public void ExecutionCommand()
        {
            while (true)
            {
                CommandOutput();
                string choise = Console.ReadLine();
                if (choise == CommandAddingBooks.Key)
                {
                    StartCommandAddingBooks();
                }
                else if (choise == CommandAddingClient.Key)
                {
                    StartCommandAddingClient();
                }
                else if (choise == CommandDeletedClient.Key)
                {
                    StartCommandDeletedClient();
                }
                else if (choise == CommandExit.Key)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Данного кода операции не существует");
                }
            }
        }

        /// <summary>
        /// Запуск команды добавления книг
        /// </summary>
        private void StartCommandAddingBooks()
        {
            if (!AddingBooks)
            {
                StartAddingBook();
            }
            else
            {
                StopAddingBook();
            }
        }

        /// <summary>
        /// Включение добавления книг
        /// </summary>
        private void StartAddingBook()
        {
            int minInterval = GetMinInterval();
            int maxInterval = GetMaxInterval(minInterval);
            GenerationBook = new RandomGenerationBook(WorkRabbit, minInterval, maxInterval);
            GenerationBook.StartGenerationBook();
            Console.WriteLine("Успешно");
            CommandAddingBooks = new KeyValuePair<string, string>("1", "Отмена добавления книг");
            AddingBooks = true;
        }

        /// <summary>
        /// Возвращение минимального значения интервала случайного добавления книг
        /// </summary>
        /// <returns>Минимального значение интервала случайного добавления книг</returns>
        private int GetMinInterval()
        {
            int minValue = 0;
            ActionDescription = "Ввведите интервал(в секундх) случайного добавления книг\n" +
                "Ввведите начальное значение интервала";
            ErrorDescription = $"Не верно введено значение или значение меньше {minValue}";
            Predicate<int> condition = n => n >= minValue;
            return GetEnteredValue(condition);
        }

        /// <summary>
        /// Возвращение максимального значения интервала случайного добавления книг
        /// </summary>
        /// <returns>Максимальное значение интервала случайного добавления книг</returns>
        private int GetMaxInterval(int minInterval)
        {
            ActionDescription = "Ввведите конечное значение интервала";
            ErrorDescription = "Не верно введено значение или конечное значение интервала меньше начального значения интервала";
            Predicate<int> condition = n => n >= minInterval;
            return GetEnteredValue(condition);
        }

        /// <summary>
        /// Выключение добавления книг
        /// </summary>
        private void StopAddingBook()
        {
            GenerationBook.StopGenerationBook();
            CommandAddingBooks = new KeyValuePair<string, string>("1", "Запуск добавления книг");
            AddingBooks = false;
            Console.WriteLine("Успешно");
        }

        /// <summary>
        /// Старт команды подписки клиента
        /// </summary>
        private void StartCommandAddingClient()
        {
            string surname = GetSurname();
            string name = GetName();
            string address = GetAddress();
            int pagesPerDay = GetPagesPerDay();
            int readingIntervalActive = GetReadingIntervalActive();
            int readingIntervalPassive = GetReadingIntervalPassive();
            int countLanguages = GetCountLanguages();
            LevelLanguageSDB[] levelLanguages = GetLevelLanguageArray(countLanguages);
            ClientSDB client = new ClientSDB(surname, name, address, pagesPerDay, readingIntervalActive, readingIntervalPassive, levelLanguages);
            string clientJSON = JsonSerializer.Serialize<ClientSDB>(client);
            WorkRabbit.PublishClientQueue(clientJSON, WorkRabbit.PropetyAdding);
            Console.WriteLine("Успешно");
        }

        /// <summary>
        /// Возвращение фамилии клиента
        /// </summary>
        /// <returns>Фамилия клиента</returns>
        private string GetSurname()
        {
            int maxLenth = 30;
            ActionDescription = "Ввведите фамилию клиента";
            ErrorDescription = "Не верно введено значение. " +
                $"Фамилия должна содержать не более {maxLenth} прописных букв кириллицы с заглавной буквой";
            Regular = @"^[A-Я][а-я]*$";
            Predicate<string> condition = n => n.Length < maxLenth;
            return GetEnteredValue(condition, Regular);
        }

        /// <summary>
        /// Возвращение имени клиента
        /// </summary>
        /// <returns>Имя клиента</returns>
        private string GetName()
        {
            int maxLenth = 30;
            ActionDescription = "Ввведите имя клиента";
            ErrorDescription = "Не верно введено значение. " +
                $"Имя должно содержать не более {maxLenth} прописных букв кириллицы с заглавной буквой";
            Regular = @"^[A-Я][а-я]*$";
            Predicate<string> condition = n => n.Length < maxLenth;
            return GetEnteredValue(condition, Regular);
        }

        /// <summary>
        /// Возвращение электронного адресса клиента
        /// </summary>
        /// <returns>Электронный адресс клиента</returns>
        private string GetAddress()
        {
            int maxLenth = 30;
            ActionDescription = "Ввведите адресс электронной почты клиента";
            ErrorDescription = $"Не верно введено значение или число символов больше {maxLenth}";
            Regular = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            Predicate<string> condition = n => n.Length < maxLenth;
            return GetEnteredValue(condition, Regular);
        }

        /// <summary>
        /// Возвращение колличества страниц, читаемое клиентом за день
        /// </summary>
        /// <returns>Колличество страниц, читаемое клиентом за день</returns>
        private int GetPagesPerDay()
        {
            int maxPagesPerDay = 1000;
            int minPagesPerDay = 1;
            ActionDescription = "Ввведите количество страниц, читаемое клиентом за день";
            ErrorDescription = $"Не верно введено значение или количество страниц превышает {maxPagesPerDay} или меньше {minPagesPerDay}";
            Predicate<int> condition = n => n <= 1000 && n >= minPagesPerDay;
            return GetEnteredValue(condition);
        }

        /// <summary>
        /// Возвращение количество дней чтения книг подряд
        /// </summary>
        /// <returns>Количество дней чтения книг подряд</returns>
        private int GetReadingIntervalActive()
        {
            int minReadingIntervalActive = 1;
            ActionDescription = "Введите количество дней чтения книг подряд";
            ErrorDescription = $"Не верно введено значение или количество дней меньше {minReadingIntervalActive}";
            Predicate<int> condition = n => n >= minReadingIntervalActive;
            return GetEnteredValue(condition);
        }

        /// <summary>
        /// Возвращение количество дней между чтением книг
        /// </summary>
        /// <returns>Количество дней между чтением книг</returns>
        private int GetReadingIntervalPassive()
        {
            int maxReadingIntervalPassive = 20;
            ActionDescription = "Введите количество дней между чтением книг";
            ErrorDescription = $"Не верно введено значение или количество дней превышает {maxReadingIntervalPassive}";
            Predicate<int> condition = n => n <= maxReadingIntervalPassive;
            return GetEnteredValue(condition);
        }

        /// <summary>
        /// Возвращение количество языков, которыми владеет клиент
        /// </summary>
        /// <returns>Количество языков, которыми владеет клиент</returns>
        private int GetCountLanguages()
        {
            int minCountLanguage = 1;
            int maxCountLanguage = 5;
            ActionDescription = "Введите количество языков, которыми владеет клиент";
            ErrorDescription = $"Не верно введено значение или количество языков превышет {maxCountLanguage} или меньше {minCountLanguage}";
            Predicate<int> condition = n => n <= maxCountLanguage && n >= minCountLanguage;
            return GetEnteredValue(condition);
        }

        /// <summary>
        /// Возвращает массив языков и уровнь их владением
        /// </summary>
        /// <param name="countLanguages">Количество языков</param>
        /// <returns>Массив языков и уровнь их владением</returns>
        private LevelLanguageSDB[] GetLevelLanguageArray(int countLanguages)
        {
            LevelLanguageSDB[] languages = new LevelLanguageSDB[countLanguages];
            List<int> numberLanguageRetry = new List<int>();
            for (int i = 0; i < countLanguages; i++)
            {
                int numberLanguage;
                while (true)
                {
                    numberLanguage = GetNumberLanguage();
                    int j = 0;
                    foreach (int number in numberLanguageRetry)
                    {
                        if (numberLanguage == number)
                        {
                            Console.WriteLine("Данный язык уже выбран");
                            break;
                        }
                        j++;
                    }
                    if (numberLanguageRetry.Count == 0 || j == numberLanguageRetry.Count)
                    {
                        numberLanguageRetry.Add(numberLanguage);
                        break;
                    }
                }
                int level = GetLevel();
                languages[i] = new LevelLanguageSDB((BookSDB.languageEnum)numberLanguage, level);
            }
            return languages;
        }

        /// <summary>
        /// Возвращение номер языка
        /// </summary>
        /// <returns>Номер языка</returns>
        private int GetNumberLanguage()
        {
            int minNumberLanguage = 1;
            int maxNumberLanguage = 5;
            ActionDescription = "Введите язык (1-Русский, 2-Английский, 3-Немецкий, 4-Итальянский, 5-Испанский)";
            ErrorDescription = $"Не верно введено значение или номер языка превышет {maxNumberLanguage} или меньше {minNumberLanguage}";
            Predicate<int> condition = n => n <= maxNumberLanguage && n >= minNumberLanguage;
            return GetEnteredValue(condition);
        }

        /// <summary>
        /// Возвращение уровня владением языка
        /// </summary>
        /// <returns>Уровнь владением языка</returns>
        private int GetLevel()
        {
            int minLevelLanguage = 1;
            int maxLevelLanguage = 10;
            ActionDescription = $"Введите уровень владения языком (от {minLevelLanguage} до {maxLevelLanguage})";
            ErrorDescription = "Не верно введено значение";
            Predicate<int> condition = n => n <= maxLevelLanguage && n >= minLevelLanguage;
            return GetEnteredValue(condition);
        }

        /// <summary>
        /// Старт команды отписки клиента
        /// </summary>
        private void StartCommandDeletedClient()
        {
            string address = GetAddress();
            WorkRabbit.PublishClientQueue(address, WorkRabbit.PropetyDeleted);
            Console.WriteLine("Успешно");
        }

        /// <summary>
        /// Вывод команды и возвращение введенного значения
        /// </summary>
        /// <param name="condition">Условие</param>
        /// <returns>Введенное значение</returns>
        private string GetEnteredValue(Predicate<string> condition, string regular)
        {
            string value;                                                          
            while (true)
            {
                Console.WriteLine(ActionDescription);
                value = Console.ReadLine();
                if (Regex.IsMatch(value, regular, RegexOptions.IgnoreCase) && condition(value))
                    break;
                else
                    Console.WriteLine(ErrorDescription);
            }
            return value;
        }

        /// <summary>
        /// Вывод команды и возвращение введенного значения
        /// </summary>
        /// <param name="condition">Условие</param>
        /// <returns>Введенное значение</returns>
        private int GetEnteredValue(Predicate<int> condition)
        {
            int value;
            while (true)
            {
                Console.WriteLine(ActionDescription);
                if (Int32.TryParse(Console.ReadLine(), out value) && condition(value))
                    break;
                else
                    Console.WriteLine(ErrorDescription);
            }
            return value;
        }

        /// <summary>
        /// Вывод команд на консоль
        /// </summary>
        private void CommandOutput()
        {
                Console.WriteLine($"{CommandAddingBooks.Key} - {CommandAddingBooks.Value}\n" +
                    $"{CommandAddingClient.Key} - {CommandAddingClient.Value}\n" + 
                    $"{CommandDeletedClient.Key} - {CommandDeletedClient.Value}\n" +
                    $"{CommandExit.Key} - {CommandExit.Value}");
        }

        /// <summary>
        /// Остановка комманд консоли
        /// </summary>
        public void StopConsole()
        {
            WorkRabbit.Dispose();
            Environment.Exit(0);
        }
    }
}
