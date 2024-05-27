using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OOP_Kursach_Museum
{
    /// <summary>
    /// Основная форма приложения для управления музейными экспонатами.
    /// </summary>
    public partial class Main : Form
    {
        private List<Museum> users = new List<Museum>();
        private ContextMenuStrip contextMenuStrip;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public Main()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            buttonDeleteDatabase.Click += buttonDeleteDatabase_Click;
            button2.Click += button2_Click; // Добавляем обработчик для button2
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            textBox1.TextChanged += TextBox1_TextChanged;
            ComboBoxFilterByName.SelectedIndexChanged += FilterComboBoxes_Changed;
            ComboBoxFilterByYear.SelectedIndexChanged += FilterComboBoxes_Changed;
            checkBoxFilterOnExhibit.CheckedChanged += FilterComboBoxes_Changed;

            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Удалить").Click += Delete_Click;
            dataGridView1.ContextMenuStrip = contextMenuStrip;
            dataGridView1.MouseDown += DataGridView1_MouseDown;
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            DataTable table = new DataTable();
            table.Columns.Add("Автор", typeof(string));
            table.Columns.Add("Год Создания", typeof(int));
            table.Columns.Add("Название экспоната", typeof(string));
            table.Columns.Add("На выставке", typeof(bool));

            users = FileManager.ReadFromFile();
            foreach (var user in users)
            {
                table.Rows.Add(user.Name, user.Year, user.ExhibitName, user.OnExhibit);
            }

            dataGridView1.DataSource = table;
            UpdateFilterComboBoxes();
            UpdateExhibitCount();
        }

        /// <summary>
        /// Обновляет значения в комбобоксах фильтров.
        /// </summary>
        private void UpdateFilterComboBoxes()
        {
            ComboBoxFilterByName.Items.Clear();
            ComboBoxFilterByName.Items.Add("");
            foreach (var user in users)
            {
                if (!ComboBoxFilterByName.Items.Contains(user.Name))
                {
                    ComboBoxFilterByName.Items.Add(user.Name);
                }
            }

            ComboBoxFilterByYear.Items.Clear();
            ComboBoxFilterByYear.Items.Add("");
            foreach (var user in users)
            {
                if (!ComboBoxFilterByYear.Items.Contains(user.Year.ToString()))
                {
                    ComboBoxFilterByYear.Items.Add(user.Year.ToString());
                }
            }
        }

        /// <summary>
        /// Обработчик клика по ячейке DataGridView.
        /// </summary>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBoxName.Text = row.Cells["Автор"].Value.ToString();
                textBoxYear.Text = row.Cells["Год Создания"].Value.ToString();
                textBoxExhibitName.Text = row.Cells["Название экспоната"].Value.ToString();
                checkBoxOnExhibit.Checked = (bool)row.Cells["На выставке"].Value;
            }
        }

        /// <summary>
        /// Обработчик клика по кнопке добавления экспоната.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBoxName.Text;
            string exhibitName = textBoxExhibitName.Text;
            bool onExhibit = checkBoxOnExhibit.Checked;
            if (int.TryParse(textBoxYear.Text, out int year))
            {
                int currentYear = DateTime.Now.Year;
                if (year <= currentYear)
                {
                    var museum = new Museum(name, year, exhibitName, onExhibit);
                    users.Add(museum);
                    DataTable table = (DataTable)dataGridView1.DataSource;
                    table.Rows.Add(name, year, exhibitName, onExhibit);
                    FileManager.AppendToFile(museum);
                    UpdateExhibitCount();
                }
                else
                {
                    MessageBox.Show("Год не может быть больше текущего года");
                }
            }
            else
            {
                MessageBox.Show("Год должен быть числом");
            }
            UpdateFilterComboBoxes();
        }

        /// <summary>
        /// Записывает данные пользователей в файл.
        /// </summary>
        private void WriteDataToFile()
        {
            FileManager.WriteToFile(users);
            UpdateFilterComboBoxes();
        }

        /// <summary>
        /// Обработчик изменения фильтров.
        /// </summary>
        private void FilterComboBoxes_Changed(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Обработчик изменения текста в текстовом поле поиска.
        /// </summary>
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Применяет фильтры к списку пользователей.
        /// </summary>
        private void ApplyFilters()
        {
            string filterByName = ComboBoxFilterByName.Text.Trim();
            string filterByYearText = ComboBoxFilterByYear.Text.Trim();
            int filterByYear;
            bool filterByYearEnabled = int.TryParse(filterByYearText, out filterByYear);
            bool filterByOnExhibit = checkBoxFilterOnExhibit.Checked;

            var filteredUsers = users;
            if (!string.IsNullOrWhiteSpace(filterByName))
            {
                filteredUsers = filteredUsers.Where(u => u.Name.Contains(filterByName)).ToList();
            }

            if (filterByYearEnabled)
            {
                filteredUsers = filteredUsers.Where(u => u.Year == filterByYear).ToList();
            }

            if (filterByOnExhibit)
            {
                filteredUsers = filteredUsers.Where(u => u.OnExhibit).ToList();
            }

            string searchText = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredUsers = filteredUsers.Where(u => u.Name.Contains(searchText) || u.Year.ToString().Contains(searchText) || u.ExhibitName.Contains(searchText)).ToList();
            }

            UpdateData(filteredUsers);
            UpdateExhibitCount();
        }

        /// <summary>
        /// Обновляет данные в DataGridView.
        /// </summary>
        private void UpdateData(List<Museum> filteredUsers)
        {
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Clear();
            foreach (var user in filteredUsers)
            {
                table.Rows.Add(user.Name, user.Year, user.ExhibitName, user.OnExhibit);
            }
        }

        /// <summary>
        /// Обновляет количество экспонатов.
        /// </summary>
        private void UpdateExhibitCount()
        {
            labelCount.Text = $"Общее количество экспонатов: {dataGridView1.Rows.Count}";
            int exhibitCount = users.Count(u => u.OnExhibit);
            labelExhibitCount.Text = $"Экспонатов на выставке: {exhibitCount}";
        }

        /// <summary>
        /// Обработчик клика по кнопке удаления базы данных.
        /// </summary>
        private void buttonDeleteDatabase_Click(object sender, EventArgs e)
        {
            users.Clear();
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Clear();
            FileManager.DeleteFile();
            UpdateExhibitCount();
            UpdateFilterComboBoxes();
        }

        /// <summary>
        /// Обработчик изменения значения ячейки DataGridView.
        /// </summary>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string name = dataGridView1.Rows[e.RowIndex].Cells["Автор"].Value.ToString();
                int year = (int)dataGridView1.Rows[e.RowIndex].Cells["Год Создания"].Value;
                string exhibitName = dataGridView1.Rows[e.RowIndex].Cells["Название экспоната"].Value.ToString();
                bool onExhibit = (bool)dataGridView1.Rows[e.RowIndex].Cells["На выставке"].Value;

                users[e.RowIndex] = new Museum(name, year, exhibitName, onExhibit);
                WriteDataToFile();
                UpdateExhibitCount();
            }
        }

        /// <summary>
        /// Обработчик изменения состояния редактируемой ячейки DataGridView.
        /// </summary>
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Обработчик нажатия правой кнопкой мыши на DataGridView.
        /// </summary>
        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = dataGridView1.HitTest(e.X, e.Y);
                dataGridView1.ClearSelection();
                if (hit.RowIndex >= 0)
                {
                    dataGridView1.Rows[hit.RowIndex].Selected = true;
                    contextMenuStrip.Show(dataGridView1, e.Location);
                }
            }
        }

        /// <summary>
        /// Обработчик клика по пункту контекстного меню "Удалить".
        /// </summary>
        private void Delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                users.RemoveAt(index);
                dataGridView1.Rows.RemoveAt(index);
                WriteDataToFile();
                UpdateExhibitCount();
                UpdateFilterComboBoxes();
            }
        }

        /// <summary>
        /// Обработчик клика по кнопке редактирования экспоната.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                int selectedIndex = dataGridView1.CurrentRow.Index;
                string previousYear = dataGridView1.CurrentRow.Cells["Год Создания"].Value.ToString();

                if (int.TryParse(textBoxYear.Text, out int year))
                {
                    // Валидация года                
                    int currentYear = DateTime.Now.Year;
                    if (year > currentYear)
                    {
                        MessageBox.Show("Год создания не может быть больше текущего года.");
                        textBoxYear.Text = previousYear; // Возвращаем предыдущее значение
                        return;
                    }

                    // Обновление DataTable
                    DataTable table = (DataTable)dataGridView1.DataSource;
                    table.Rows[selectedIndex]["Автор"] = textBoxName.Text;
                    table.Rows[selectedIndex]["Год Создания"] = year;
                    table.Rows[selectedIndex]["Название экспоната"] = textBoxExhibitName.Text;
                    table.Rows[selectedIndex]["На выставке"] = checkBoxOnExhibit.Checked;

                    // Обновление списка users
                    users[selectedIndex] = new Museum(textBoxName.Text, year, textBoxExhibitName.Text, checkBoxOnExhibit.Checked);

                    // Запись обновленных данных обратно в файл
                    WriteDataToFile();

                    // Обновление количества экспонатов
                    UpdateExhibitCount();
                }
                else
                {
                    MessageBox.Show("Год Создания должен быть числом.");
                    textBoxYear.Text = previousYear; // Возвращаем предыдущее значение
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для редактирования.");
            }
        }
    }
}
