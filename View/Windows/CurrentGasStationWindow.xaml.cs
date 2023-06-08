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
    /// Логика взаимодействия для CurrentGasStationWindow.xaml
    /// </summary>
    public partial class CurrentGasStationWindow : Window
    {
        public static GasStation gStation;
        public CurrentGasStationWindow(GasStation gasStation)
        {
            InitializeComponent();
            var selectedGasStation = gasStation;
            gStation = gasStation;
            gasStationNameTbl.Text = gasStation.name;
            if (gasStation.fuelLow == "Нет данных")
            {
                fuelLowTbl.Text = gasStation.fuelLow;
                fuelMedTbl.Text = gasStation.fuelMed;
                payBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                fuelLowTbl.Text = gasStation.fuelLow + " ₽";
                fuelMedTbl.Text = gasStation.fuelMed + " ₽";
                payBtn.Visibility = Visibility.Visible;
            }
        }

        private void payBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.enteredUser == null)
            {
                MessageBox.Show("Пожалуйста, авторизуйтесь для оплаты!", "Внимание!");
            }
            else
            {
                PaymentWindow paymentWindow = new PaymentWindow(gStation);
                paymentWindow.ShowDialog();
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void closeWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
