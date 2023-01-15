using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SaperLab2WPF
{
    /// <summary>
    /// Логика взаимодействия для WindowNotMain.xaml
    /// </summary>
    public partial class WindowNotMain : Window
    {
        public WindowNotMain()
        {
            InitializeComponent();
            DataContext = new ApplyViewModelMini();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
