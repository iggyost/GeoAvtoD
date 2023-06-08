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
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            historyLb.ItemsSource = App.context.Searches.Where(s => s.user_id == App.enteredUser.id).ToList();
            userNameTbl.Text = "Профиль: " + App.enteredUser.name;
            cardsCb.ItemsSource = App.context.Cards.Where(c => c.user_id == App.enteredUser.id).ToList();
            if (cardsCb.Items.Count != 0)
            {
                cardsCb.SelectedIndex = 0;
            }
        }

        private void authBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void routeTimeTbl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            int index = int.Parse(textBlock.Tag.ToString());
            var currentSearch = App.context.Searches.Where(s => s.id == index).FirstOrDefault();
            if (currentSearch != null)
            {
                var floatTime = Math.Round(currentSearch.route_time, 0);
                var dateTime = DateTime.MinValue.AddSeconds(floatTime);
                if (dateTime.Hour >= 1)
                {
                    textBlock.Text = dateTime.Hour + " ч " + dateTime.Minute + " мин";
                }
                else
                {
                    textBlock.Text = dateTime.Minute + " мин";
                }
            }
        }

        private void addCardBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNewCardWindow addNewCardWindow = new AddNewCardWindow();
            addNewCardWindow.ShowDialog();
            if (addNewCardWindow.DialogResult == true)
            {
                cardsCb.ItemsSource = App.context.Cards.Where(c => c.user_id == App.enteredUser.id).ToList();
            }
        }

        private void removeCardBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedCard = cardsCb.SelectedItem as Cards;
            if (selectedCard != null)
            {
                App.context.Cards.Remove(selectedCard);
                App.context.SaveChanges();
                cardsCb.ItemsSource = App.context.Cards.Where(c => c.user_id == App.enteredUser.id).ToList();
            }
            else
            {
                MessageBox.Show("Выберите карту!", "Внимание!");
            }
        }
    }
}
