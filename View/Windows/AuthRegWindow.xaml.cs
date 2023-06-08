using GeoAvto.Model;
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

namespace GeoAvto.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthRegWindow.xaml
    /// </summary>
    public partial class AuthRegWindow : Window
    {
        public AuthRegWindow()
        {
            InitializeComponent();
        }

        private void authBtn_Click(object sender, RoutedEventArgs e)
        {
            var user = App.context.Users.SingleOrDefault(u => u.login == authLogin.Text && u.password == authPassword.Password);
            if (user != null)
            {
                App.enteredUser = user;
                this.DialogResult = true;
                ProfileWindow profileWindow = new ProfileWindow();
                profileWindow.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Пользователь не найден!", "Ошибка!");
            }
        }

        private void regBtn_Click(object sender, RoutedEventArgs e)
        {
            Users newUser = new Users()
            {
                name = regName.Text,
                login = regLogin.Text,
                password = regPassword.Password,
            };
            var user = App.context.Users.SingleOrDefault(u => u.login == regLogin.Text);
            if (user == null)
            {
                App.context.Users.Add(newUser);
                App.context.SaveChanges();
                regGrid.Visibility = Visibility.Collapsed;
                authGrid.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Пользователь с таким именем уже существует!", "Ошибка!");
            }
        }

        private void changeToLogBtn_Click(object sender, RoutedEventArgs e)
        {
            regGrid.Visibility = Visibility.Collapsed;
            authGrid.Visibility = Visibility.Visible;
        }

        private void changeToRegBtn_Click(object sender, RoutedEventArgs e)
        {
            regGrid.Visibility = Visibility.Visible;
            authGrid.Visibility = Visibility.Collapsed;
        }
    }
}
