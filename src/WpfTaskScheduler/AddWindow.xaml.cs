using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfTaskScheduler
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public string date;
        public string ladaPriora;
        public string name;
        public MainWindow main;
        public AddWindow()
        {
            InitializeComponent();
            comboBox.Items.Clear();
            foreach (string item in new string[] { "1", "2", "3", "4" })
            {
                comboBox.Items.Add(item);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (datePicker.Text.Replace(" ","") == "" || comboBox.Text.Replace(" ", "") == "" || textBox.Text.Replace(" ", "") == "") { MessageBox.Show("Некорректный ввод.");return;}
            date = datePicker.Text;
            ladaPriora = comboBox.Text;
            name = textBox.Text;
            main.AddTaskSucessful(date, ladaPriora, name);
            Close();
        }
    }
}
