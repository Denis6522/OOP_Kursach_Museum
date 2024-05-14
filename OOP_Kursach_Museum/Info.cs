using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Kursach_Museum
{
    public partial class Info : System.Windows.Forms.Form
    {
        public Info()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main mainForm = new Main(); // Создание объекта главной формы
            mainForm.Show(); // Отображение главной формы
            this.Hide(); // Скрытие текущей формы
        }
    }
}
