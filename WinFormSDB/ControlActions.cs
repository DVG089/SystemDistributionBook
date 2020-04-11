using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormSDB.WcfServiceSDB;

namespace WinFormSDB
{
    /// <summary>
    /// Класс управления действиями
    /// </summary>
    internal class ControlActions
    {
        /// <summary>
        /// Объект сервиса SDB
        /// </summary>
        private ServiceSDBClient Service_SDB;
        /// <summary>
        /// Объект формы SDB
        /// </summary>
        private FormSDB Form_SDB;
        /// <summary>
        /// Объект фильтра
        /// </summary>
        private FilterSDB Filter;
        /// <summary>
        /// Электронный адресс выбраного клиента
        /// </summary>
        private string Address;
        /// <summary>
        /// Выбранный язык
        /// </summary>
        private string Language;
        /// <summary>
        /// Дата-время начала периода фильтрации
        /// </summary>
        private DateTime? StartPeriod;

        /// <summary>
        /// Конструктор объекта управления действиями
        /// </summary>
        /// <param name="form">Объект формы SDB</param>
        public ControlActions(FormSDB form)
        {
            Service_SDB = new ServiceSDBClient();
            Form_SDB = form;
            Filter = new FilterSDB();
            Address = null;
            Language = null;
            StartPeriod = null;

        }

        /// <summary>
        /// Возвращение статистики клиентов по фильтру
        /// </summary>
        /// <returns>Статистика клиентов</returns>
        public DataSet GetClientStatisticsByFilter()
        {
            DataSet dataSet = null;
            Language = Form_SDB.cmbLanguage.Text;
            InitializationStartPeriod();
            FillFilterSDB(Form_SDB.cmbStatus.Text, Form_SDB.cmbLanguage.Text, Form_SDB.txbClientBook.Text, StartPeriod);
            try
            {
                dataSet = Service_SDB.GetClientStatistics(Filter);
            }
            catch
            {
                OutputFormError("Ошибка связи с сервисом.");
            }
            return dataSet;
        }

        /// <summary>
        /// Возвращение книг по фильтру
        /// </summary>
        /// <returns>Книги</returns>
        public DataSet GetBooksByFilter()
        {
            DataSet dataSet = null;
            Language = Form_SDB.cmbLanguage.Text;
            InitializationStartPeriod();
            FillFilterSDB(Form_SDB.cmbStatus.Text, Form_SDB.cmbLanguage.Text, Form_SDB.txbClientBook.Text, StartPeriod);
            Address = "";
            try
            {
                dataSet = Service_SDB.GetBook(Filter, Address);
            }
            catch
            {
                OutputFormError("Ошибка связи с сервисом.");
            }
            return dataSet;
        }

        /// <summary>
        /// Возвращение статистики клиентов по книге
        /// </summary>
        /// <returns>Статистика клиентов</returns>
        public DataSet GetClientStatisticsByBook()
        {
            DataSet dataSet = null;
            Address = Form_SDB.dgvBooks.Rows[Form_SDB.dgvBooks.CurrentCell.RowIndex].Cells["AddressClient"].Value.ToString();
            FillFilterSDB("", Language, Address, StartPeriod);
            try
            {
                dataSet = Service_SDB.GetClientStatistics(Filter);
            }
            catch
            {
                OutputFormError("Ошибка связи с сервисом.");
            }
            return dataSet;
        }

        /// <summary>
        /// Возвращение книг по клиенту
        /// </summary>
        /// <returns>Книги</returns>
        public DataSet GetBooksByClient()
        {
            DataSet dataSet = null;
            Address = Form_SDB.dgvClientsStatistics.Rows[Form_SDB.dgvClientsStatistics.CurrentCell.RowIndex].Cells["Address"].Value.ToString();
            FillFilterSDB("", Language, "", StartPeriod);
            try
            {
                dataSet = Service_SDB.GetBook(Filter, Address);
            }
            catch
            {
                OutputFormError("Ошибка связи с сервисом.");
            }
            return dataSet;
        }

        /// <summary>
        /// Возвращение информации по клиенту
        /// </summary>
        /// <param name="sender">Вызывающий объект</param>
        /// <returns>Информация по клиенту</returns>
        public DataSet GetClientInfo(object sender)
        {
            DataSet dataSet = null;
            if (sender == Form_SDB.btnInfoClientCS)
                Address = Form_SDB.dgvClientsStatistics.Rows[Form_SDB.dgvClientsStatistics.CurrentCell.RowIndex].Cells["Address"].Value.ToString();
            else if (sender == Form_SDB.btnInfoClientB)
                Address = Form_SDB.dgvBooks.Rows[Form_SDB.dgvBooks.CurrentCell.RowIndex].Cells["AddressClient"].Value.ToString();
            else
                Address = "";
            try
            {
                dataSet = Service_SDB.GetClientInfo(Address);
            }
            catch
            {
                OutputFormError("Ошибка связи с сервисом.");
            }
            return dataSet;
        }

        /// <summary>
        /// Создание и вывод формы ошибки
        /// </summary>
        /// <param name="error">Текст ошибки</param>
        private void OutputFormError(string error)
        {
            FormError form = new FormError(error);
            form.ShowDialog();
        }

        /// <summary>
        /// Инициализация Даты-времени начала периода фильтрации
        /// </summary>
        private void InitializationStartPeriod()
        {
            double Period = Decimal.ToDouble(Form_SDB.numPeriod.Value);
            if (Period == 0)
            {
                StartPeriod = null;
            }
            else
            {
                StartPeriod = DateTime.UtcNow.ToLocalTime().AddDays(-Period);
            }
        }

        /// <summary>
        /// Заполнение фильтра
        /// </summary>
        /// <param name="status">Фильтр по статус подписки (Подписан, Отписан) или  статусу чтения (Читается, Прочитана)</param>
        /// <param name="language">Фильтр по языку</param>
        /// <param name="clientBook">Фильтр по электронному адрессу клиента или имени книги</param>
        private void FillFilterSDB(string status, string language, string clientBook, DateTime? startPeriod)
        {
            Filter.Status = status;
            Filter.Language = language;
            Filter.ClientBook = clientBook;
            Filter.StartPeriod = startPeriod;
        }

        /// <summary>
        /// Закрытие сервиса SDB
        /// </summary>
        public void CloseService()
        {
            Service_SDB.Close();
        }
    }
}
