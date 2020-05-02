using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormSDB.WcfServiceSDB;

namespace WinFormSDB
{
    /// <summary>
    /// Форма SDB
    /// </summary>
    public partial class FormSDB : Form
    {
        /// <summary>
        /// Объект управления действиями
        /// </summary>
        private ControlActions Control;

        public FormSDB()
        {
            InitializeComponent();

            cmbSearch.SelectedIndex = 0;
            cmbLanguage.SelectedIndex = 0;
            ChangeControlsOnClients();
            Control = new ControlActions(this);

            //событие закрытия формы
            this.FormClosed += (sender, e) => Control.CloseService();
            //событие выбора комбобокса "Поиск по"
            cmbSearch.SelectedIndexChanged += (sender, e) => СhangeControls();
            //событие нажатия кнопки "Поиск"
            btnSearch.Click += MainSearch;
            //событие нажатия кнопки "Информация о клиенте" в таблице "Статистика по клиентам"
            btnInfoClientCS.Click += SearchClient;
            //событие нажатия кнопки "Информация о книгах"
            btnInfoBooks.Click += SearchBook;
            //событие нажатия кнопки "Информация о клиенте" в таблице "Книги"
            btnInfoClientB.Click += SearchClient;
            //событие нажатия кнопки "Статистика клиента"
            btnClientStatistics.Click += SearchStatisticsClient;
        }

        /// <summary>
        /// Поиск информации
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <param name="e">Вспомогательный объект</param>
        private void MainSearch(object sender, EventArgs e)
        {
            if (cmbSearch.Text == cmbSearch.Items[0].ToString())
            {
                MainSearchByClients();
            }
            else if (cmbSearch.Text == cmbSearch.Items[1].ToString())
            {
                MainSearchByBooks();
            }
        }

        /// <summary>
        /// Поиск информации по клиентам
        /// </summary>
        private void MainSearchByClients()
        {
            DataSet dataSet = Control.GetClientStatisticsByFilter();
            if (dataSet != null)
            {
                ClearTables();
                ChangeBunnonsBooksTable(false);
                dgvClientsStatistics.DataSource = dataSet.Tables[0];
                EditClientStatisticsTable();
                ChangeBunnonsClientStatisticsTable(true);
            }
        }

        /// <summary>
        /// Поиск информации по книгам
        /// </summary>
        private void MainSearchByBooks()
        {
            DataSet dataSet = Control.GetBooksByFilter();
            if (dataSet != null)
            {
                ClearTables();
                ChangeBunnonsClientStatisticsTable(false);
                dgvBooks.DataSource = dataSet.Tables[0];
                EditBookTable();
                ChangeBunnonsBooksTable(true);
            }
        }

        /// <summary>
        /// Поиск статистики клиента
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <param name="e">Вспомогательный объект</param>
        private void SearchStatisticsClient(object sender, EventArgs e)
        {
            DataSet dataSet = Control.GetClientStatisticsByBook();
            if (dataSet != null)
            {
                dgvClientsStatistics.DataSource = dataSet.Tables[0];
                EditClientStatisticsTable();
            }
        }
        /// <summary>
        /// Поиск книг
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <param name="e">Вспомогательный объект</param>
        private void SearchBook(object sender, EventArgs e)
        {
            DataSet dataSet = Control.GetBooksByClient();
            if (dataSet != null)
            {
                dgvBooks.DataSource = dataSet.Tables[0];
                EditBookTable();
            }
        }

        /// <summary>
        /// Поиск информации по клиенту
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <param name="e">Вспомогательный объект</param>
        private void SearchClient(object sender, EventArgs e)
        {
            DataSet dataSet = Control.GetClientInfo(sender);
            if (dataSet != null)
            {
                dgvClientInfo.DataSource = dataSet.Tables[0];
                EditClientInfoTable();
                dgvLevelLanguage.DataSource = dataSet.Tables[1];
                EditLevelLanguageTable();
            }
        }

        /// <summary>
        /// Редактирование таблицы книг
        /// </summary>
        private void EditBookTable()
        {
            dgvBooks.Columns[0].Width = 200;
            dgvBooks.Columns[0].HeaderText = "Адресс клиента";
            dgvBooks.Columns[1].HeaderText = "Язык";
            dgvBooks.Columns[2].HeaderText = "Название книги";
            dgvBooks.Columns[3].Width = 70;
            dgvBooks.Columns[3].HeaderText = "Страниц";
            dgvBooks.Columns[4].HeaderText = "Дата получения";
            dgvBooks.Columns[5].HeaderText = "Дата прочтения";
            dgvBooks.Columns[6].Visible = false;
        }
        /// <summary>
        /// Редактирование таблицы статистики по клиентам
        /// </summary>
        private void EditClientStatisticsTable()
        {
            dgvClientsStatistics.Columns[0].Width = 200;
            dgvClientsStatistics.Columns[0].HeaderText = "Адресс клиента";
            dgvClientsStatistics.Columns[1].HeaderText = "Прочитано книг";
            dgvClientsStatistics.Columns[2].HeaderText = "Прочитано страниц";
        }

        /// <summary>
        /// Редактирование таблицы информации по клиенту
        /// </summary>
        private void EditClientInfoTable()
        {
            dgvClientInfo.Columns[0].Width = 200;
            dgvClientInfo.Columns[0].HeaderText = "Адресс клиента";
            dgvClientInfo.Columns[1].HeaderText = "Фамилия";
            dgvClientInfo.Columns[2].HeaderText = "Имя";
            dgvClientInfo.Columns[3].HeaderText = "Чтение страниц в день";
            dgvClientInfo.Columns[3].Width = 115;
            dgvClientInfo.Columns[4].HeaderText = "Количество дней чтения";
            dgvClientInfo.Columns[5].HeaderText = "Количество дней отдыха";
            dgvClientInfo.Columns[6].HeaderText = "Дата регистрации";
            dgvClientInfo.Columns[7].HeaderText = "Статус подписки";
        }

        /// <summary>
        /// Редактирование таблицы языков клиента и уровней их владением
        /// </summary>
        private void EditLevelLanguageTable()
        {
            dgvLevelLanguage.Columns[0].Visible = false;
            dgvLevelLanguage.Columns[1].HeaderText = "Язык";
            dgvLevelLanguage.Columns[2].Width = 120;
            dgvLevelLanguage.Columns[2].HeaderText = "Уровень чтения (от 1 до 10)";
        }

        /// <summary>
        /// Изменение включения кнопок таблицы книг
        /// </summary>
        /// <param name="enabled">Включение: true-включить, false-выключить</param>
        private void ChangeBunnonsBooksTable(bool enabled)
        {
            btnInfoClientB.Enabled = enabled;
            btnClientStatistics.Enabled = enabled;
        }

        /// <summary>
        /// Изменение включения кнопок таблицы статистики по клиентам
        /// </summary>
        /// <param name="enabled">Включение: true-включить, false-выключить</param>
        private void ChangeBunnonsClientStatisticsTable(bool enabled)
        {
            btnInfoClientCS.Enabled = enabled;
            btnInfoBooks.Enabled = enabled;
        }

        /// <summary>
        /// Очистка таблиц
        /// </summary>
        private void ClearTables()
        {
            dgvClientsStatistics.DataSource = null;
            dgvLevelLanguage.DataSource = null;
            dgvClientInfo.DataSource = null;
            dgvBooks.DataSource = null;
        }

        /// <summary>
        /// Изменение контролов
        /// </summary>
        private void СhangeControls()
        {
            if (cmbSearch.Text == cmbSearch.Items[0].ToString())
            {
                ChangeControlsOnClients();
            }
            else if (cmbSearch.Text == cmbSearch.Items[1].ToString())
            {
                ChangeControlsOnBooks();
            }            
        }

        /// <summary>
        /// Изменение контролов на поиск по клиентам
        /// </summary>
        private void ChangeControlsOnClients()
        {
            lblStatus.Text = "Статус подписки";
            lblClientBook.Text = "Адресс клиента";
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new object[] {
                                    "Все",
                                    "Подписан",
                                    "Отписан"});
            cmbStatus.SelectedIndex = 0;
        }

        /// <summary>
        /// Изменение контролов на поиск по книгам
        /// </summary>
        private void ChangeControlsOnBooks()
        {
            lblStatus.Text = "Статус чтения";
            lblClientBook.Text = "Название книги";
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new object[] {
                                    "Все",
                                    "Прочитана",
                                    "Читается"});
            cmbStatus.SelectedIndex = 0;
        }
    }
}
