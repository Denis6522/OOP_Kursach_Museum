using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OOP_Kursach_Museum
{
    public partial class Main : System.Windows.Forms.Form
    {
        public struct User
        {
            public string name;
            public int age;
            public string exhibitName;

            public User(string _name, int _age, string _exhibitName) // конструктор
            {
                name = _name;
                age = _age;
                exhibitName = _exhibitName;
            }
        }

        public struct NewUser
        {
            public string name;
            public int age;
            public string exhibitName;

            public NewUser(string _name, int _age, string _exhibitName)
            {
                name = _name;
                age = _age;
                exhibitName = _exhibitName;
            }
        }

        List<User> users = new List<User>();
        List<NewUser> newUser = new List<NewUser>();

        public Main()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
            dataGridView1.AllowUserToAddRows = false;

            DataTable table = new DataTable();
            table.Columns.Add("Автор", typeof(string));
            table.Columns.Add("Год Создания", typeof(int));
            table.Columns.Add("Название экспоната", typeof(string));

            // Чтение начальных данных из input.txt
            string filePath = "input.txt";
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length == 3 && int.TryParse(parts[1], out int age))
                    {
                        string name = parts[0];
                        string exhibitName = parts[2]; // Новое чтение для Название экспоната
                        users.Add(new User(name, age, exhibitName)); // Обновлено создание объекта User
                        newUser.Add(new NewUser(name, age, exhibitName)); // Необходимо обновить создание объекта NewUser
                        table.Rows.Add(name, age, exhibitName); // Добавлено в DataTable
                    }
                }
            }
            else
            {
                // Создаем файл, если его нет
                File.Create(filePath).Close();
            }

            dataGridView1.DataSource = table;

            //--------------------------------------
            UpdateFilterComboBoxes();
        }

        //Обновление комбобоксов
        private void UpdateFilterComboBoxes()
        {
            // Обновление комбобокса для фильтрации по имени
            ComboBoxFilterByName.Items.Clear();
            ComboBoxFilterByName.Items.Add(""); // Пустое значение для отмены фильтрации
            foreach (var user in users)
            {
                if (!ComboBoxFilterByName.Items.Contains(user.name))
                {
                    ComboBoxFilterByName.Items.Add(user.name);
                }
            }

            // Обновление комбобокса для фильтрации по Год Создания
            ComboBoxFilterByAge.Items.Clear();
            ComboBoxFilterByAge.Items.Add(""); // Пустое значение для отмены фильтрации
            foreach (var user in users)
            {
                if (!ComboBoxFilterByAge.Items.Contains(user.age.ToString()))
                {
                    ComboBoxFilterByAge.Items.Add(user.age.ToString());
                }
            }
        }

        //Добавление данных в поля по нажатию
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBoxName.Text = row.Cells["Автор"].Value.ToString();
                textBoxAge.Text = row.Cells["Год Создания"].Value.ToString();
                textBoxExhibitName.Text = row.Cells["Название экспоната"].Value.ToString(); // Обновляем значение Название экспоната

            }
        }

        //Добавить
        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBoxName.Text;
            string exhibitName = textBoxExhibitName.Text; // Новое поле "Название экспоната"
            if (int.TryParse(textBoxAge.Text, out int age))
            {
                // Добавление в список users
                users.Add(new User(name, age, exhibitName));

                // Добавление в DataTable
                DataTable table = (DataTable)dataGridView1.DataSource;
                table.Rows.Add(name, age, exhibitName);

                // Добавление в список newUser
                newUser.Add(new NewUser(name, age, exhibitName));

                // Добавление в файл
                using (FileStream fs = new FileStream("input.txt", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(name + " " + age + " " + exhibitName); // Записываем все три значения
                }
            }
            else
            {
                MessageBox.Show("Год должен быть числом");
            }
            UpdateFilterComboBoxes();
        }

        //Редактировать
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                int selectedIndex = dataGridView1.CurrentRow.Index;

                if (int.TryParse(textBoxAge.Text, out int age))
                {
                    // Обновление DataTable
                    DataTable table = (DataTable)dataGridView1.DataSource;
                    table.Rows[selectedIndex]["Автор"] = textBoxName.Text;
                    table.Rows[selectedIndex]["Год Создания"] = age;
                    table.Rows[selectedIndex]["Название экспоната"] = textBoxExhibitName.Text; // Обновляем значение Название экспоната

                    // Обновление списка users
                    users[selectedIndex] = new User(textBoxName.Text, age, textBoxExhibitName.Text);

                    // Обновление списка newUser
                    newUser[selectedIndex] = new NewUser(textBoxName.Text, age, textBoxExhibitName.Text);

                    // Запись обновленных данных обратно в файл
                    WriteDataToFile();
                }
                else
                {
                    MessageBox.Show("Год Создания должен быть числом");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для редактирования.");
            }
        }

        //Удалить        
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                int selectedIndex = dataGridView1.CurrentRow.Index;

                // Удаление из DataTable
                DataTable table = (DataTable)dataGridView1.DataSource;
                table.Rows.RemoveAt(selectedIndex);

                // Удаление из списка users
                users.RemoveAt(selectedIndex);

                // Удаление из списка newUser
                newUser.RemoveAt(selectedIndex);

                // Запись обновленных данных обратно в файл
                WriteDataToFile();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.");
            }
        }

        //Запись данных в файл
        private void WriteDataToFile()
        {
            using (FileStream fs = new FileStream("input.txt", FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                foreach (var user in newUser)
                {
                    sw.WriteLine(user.name + " " + user.age);
                }
            }
            UpdateFilterComboBoxes();
        }

        //Фильтрация
        private void button4_Click(object sender, EventArgs e)
        {
            // Получаем значения для фильтрации
            string filterByName = ComboBoxFilterByName.Text.Trim();
            string filterByAgeText = ComboBoxFilterByAge.Text.Trim();
            int filterByAge;
            bool filterByAgeEnabled = int.TryParse(filterByAgeText, out filterByAge);

            // Фильтруем данные в зависимости от выбранных фильтров
            var filteredUsers = users;

            if (!string.IsNullOrWhiteSpace(filterByName))
            {
                filteredUsers = filteredUsers.Where(u => u.name.Contains(filterByName)).ToList();
            }

            if (filterByAgeEnabled)
            {
                filteredUsers = filteredUsers.Where(u => u.age == filterByAge).ToList();
            }

            // Очищаем таблицу и добавляем отфильтрованные данные
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Clear();
            foreach (var user in filteredUsers)
            {
                table.Rows.Add(user.name, user.age, user.exhibitName);
            }
        }

        //Поиск
        private void button5_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var filteredUsers = users.Where(u => u.name.Contains(searchText) || u.age.ToString().Contains(searchText) || u.exhibitName.Contains(searchText)).ToList();
                UpdateDataGridView(filteredUsers);
            }
            else
            {
                // Если поле поиска пустое, отобразите все записи
                UpdateDataGridView(users);
            }
        }

        // Метод для обновления DataGridView
        private void UpdateDataGridView(List<User> filteredUsers)
        {
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Clear();
            foreach (var user in filteredUsers)
            {
                table.Rows.Add(user.name, user.age, user.exhibitName);
            }
        }
    }
}