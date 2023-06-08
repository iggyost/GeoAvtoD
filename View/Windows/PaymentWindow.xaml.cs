using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
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
    /// Логика взаимодействия для PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        public static GasStation gStation;
        public PaymentWindow(GasStation gasStation)
        {
            InitializeComponent();
            gStation = gasStation;
            cardCb.ItemsSource = App.context.Cards.Where(c => c.user_id == App.enteredUser.id).ToList();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void payBtn_Click(object sender, RoutedEventArgs e)
        {
            if (cardCb.SelectedIndex != -1 && costTbl.Text != null)
            {
                MessageBox.Show("Оплачено!");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста выбери карту и количество литров бензина!");
            }
        }
        private void litersSlider_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void fuelLowRdbtn_Checked(object sender, RoutedEventArgs e)
        {
            var roundedFuel = Math.Round(double.Parse(gStation.fuelLow, System.Globalization.CultureInfo.InvariantCulture), 0);
            var sldValue = Convert.ToInt32(Math.Round(litersSlider.Value, 0));
            var cost = roundedFuel * sldValue;
            costTbl.Text = Convert.ToString(cost) + " ₽";
        }

        private void fuelMedRdbtn_Checked(object sender, RoutedEventArgs e)
        {
            var roundedFuel = Math.Round(double.Parse(gStation.fuelMed, System.Globalization.CultureInfo.InvariantCulture), 0);
            var sldValue = Convert.ToInt32(Math.Round(litersSlider.Value, 0));
            var cost = roundedFuel * sldValue;
            costTbl.Text = Convert.ToString(cost) + " ₽";
        }

        private void litersSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (fuelLowRdbtn.IsChecked == true)
            {
                var roundedFuel = Math.Round(double.Parse(gStation.fuelLow, System.Globalization.CultureInfo.InvariantCulture), 0);
                var sldValue = Convert.ToInt32(Math.Round(litersSlider.Value, 0));
                var cost = roundedFuel * sldValue;
                costTbl.Text = Convert.ToString(cost) + " ₽";
            }
            else if (fuelMedRdbtn.IsChecked == true)
            {
                var roundedFuel = Math.Round(double.Parse(gStation.fuelMed, System.Globalization.CultureInfo.InvariantCulture), 0);
                var sldValue = Convert.ToInt32(Math.Round(litersSlider.Value, 0));
                var cost = roundedFuel * sldValue;
                costTbl.Text = Convert.ToString(cost) + " ₽";
            }
            else
            {

            }
        }
    }
}
