using GeoAvto.Model;
using GeoJSON.Net.Geometry;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Itinero;
using Itinero.Algorithms.Search.Hilbert;
using Itinero.Geo;
using Itinero.IO.Osm;
using Itinero.Osm.Vehicles;
using Itinero.Profiles;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Yandex.Geocoder;
using static System.Net.Mime.MediaTypeNames;

namespace GeoAvto.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //выбор подгрузки карты – онлайн или из ресурсов
            gMapControl1.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance; //какой провайдер карт используется (в нашем случае гугл) 
            gMapControl1.MinZoom = 2; //минимальный зум
            gMapControl1.MaxZoom = 16; //максимальный зум
            gMapControl1.Zoom = 11; // какой используется зум при открытии
            gMapControl1.Position = new GMap.NET.PointLatLng(55.7348, 37.5849);// точка в центре карты при открытии (Москва)
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter; // как приближает (просто в центр карты или по положению мыши)
            gMapControl1.CanDragMap = true; // перетаскивание карты мышью
            gMapControl1.DragButton = MouseButton.Left; // какой кнопкой осуществляется перетаскивание
            gMapControl1.ShowCenter = false; //показывать или скрывать красный крестик в центре
            gMapControl1.ShowTileGridLines = false; //показывать или скрывать тайлы
        }
        // сериализация карты, использовать однократно только при смене источника карты на новый!!!
        //var routerDb = new RouterDb();
        //using (var stream = new FileInfo(@"D:\osmMap\central-fed-district-latest.osm.pbf").OpenRead())
        //{
        //    routerDb.LoadOsmData(stream, Vehicle.Car);
        //}
        //using (var stream = new FileInfo(@"D:\osmMap\central-fed-district-latest.routerdb").Open(FileMode.Create))
        //{
        //    routerDb.Serialize(stream);
        //}


        bool isRouterOpened = false;
        private void routeBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isRouterOpened == false)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = 0;
                doubleAnimation.To = 1;
                doubleAnimation.Duration = TimeSpan.FromSeconds(0.15);
                routeOpened.BeginAnimation(Border.OpacityProperty, doubleAnimation);
                isRouterOpened = true;
            }
            else
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = 1;
                doubleAnimation.To = 0;
                doubleAnimation.Duration = TimeSpan.FromSeconds(0.15);
                routeOpened.BeginAnimation(Border.OpacityProperty, doubleAnimation);
                isRouterOpened = false;
            }
        }
        Profile profile;
        private async void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (address1Tb.Text != string.Empty && address2Tb.Text != string.Empty)
                {
                    var request1 = new GeocoderRequest { Request = address1Tb.Text };
                    var request2 = new GeocoderRequest { Request = address2Tb.Text };
                    var client = new GeocoderClient("58088810-4b37-4dc4-8af9-e4b26ec3c749");

                    var response1 = await client.Geocode(request1);
                    var response2 = await client.Geocode(request2);

                    var firstGeoObject = response1.GeoObjectCollection.FeatureMember.FirstOrDefault();
                    var secondGeoObject = response2.GeoObjectCollection.FeatureMember.FirstOrDefault();
                    var coordinate1 = firstGeoObject.GeoObject.Point.Pos;
                    var coordinate2 = secondGeoObject.GeoObject.Point.Pos;
                    string[] coordinates1Array = coordinate1.Split(' ');
                    string latitude1 = coordinates1Array[1];
                    string longtitude1 = coordinates1Array[0];
                    string[] coordinates2Array = coordinate2.Split(' ');
                    string latitude2 = coordinates2Array[1];
                    string longtitude2 = coordinates2Array[0];
                    var lat1 = Convert.ToSingle(Math.Round(Convert.ToDouble(latitude1.Replace('.', ',')), 5));
                    var lon1 = Convert.ToSingle(Math.Round(Convert.ToDouble(longtitude1.Replace('.', ',')), 5));
                    var lat2 = Convert.ToSingle(Math.Round(Convert.ToDouble(latitude2.Replace('.', ',')), 5));
                    var lon2 = Convert.ToSingle(Math.Round(Convert.ToDouble(longtitude2.Replace('.', ',')), 5));

                    using (var stream = new FileInfo(@"C:\Users\igoro\source\repos\GeoAvto\Resources\Maps\central-fed-district-latest.routerdb").Open(FileMode.Open))
                    {
                        var routeDb = RouterDb.Deserialize(stream);
                        if (fastRouteRbtn.IsChecked == true)
                        {
                            gMapControl1.Markers.Clear();
                            profile = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();
                            var router = new Router(routeDb);

                            var start = router.Resolve(profile, lat1, lon1);
                            var end = router.Resolve(profile, lat2, lon2);

                            var route = router.Calculate(profile, start, end);

                            List<PointLatLng> put = new List<PointLatLng>();
                            foreach (var item in route.Shape)
                            {
                                PointLatLng pt = new PointLatLng(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude));
                                put.Add(pt);
                            }

                            if (App.enteredUser != null)
                            {
                                Searches searches = new Searches()
                                {
                                    address_start = address1Tb.Text,
                                    address_finish = address2Tb.Text,
                                    route_distance = route.TotalDistance,
                                    route_time = route.TotalTime,
                                    time_finish = DateTime.Now.AddSeconds(route.TotalTime),
                                    user_id = App.enteredUser.id
                                };
                                App.context.Searches.Add(searches);
                                App.context.SaveChanges();
                            }
                            var floatTime = Math.Round(route.TotalTime, 0);
                            var dateTime = DateTime.MinValue.AddSeconds(floatTime);
                            if (dateTime.Hour >= 1)
                            {
                                timeTbl.Text = dateTime.Hour + " ч " + dateTime.Minute + " м";
                            }
                            else
                            {
                                timeTbl.Text = dateTime.Minute + " мин";
                            }
                            distanceTbl.Text = Math.Round(route.TotalDistance, 0) + " м";
                            departureTbl.Text = address1Tb.Text;
                            arrivalTbl.Text = address2Tb.Text;

                            GMapRoute r = new GMapRoute(put);
                            gMapControl1.Markers.Add(r);
                        }
                        else if (shortRouteRbtn.IsChecked == true)
                        {
                            gMapControl1.Markers.Clear();
                            profile = Itinero.Osm.Vehicles.Vehicle.Car.Shortest();
                            var router = new Router(routeDb);

                            var start = router.Resolve(profile, lat1, lon1);
                            var end = router.Resolve(profile, lat2, lon2);

                            var route = router.Calculate(profile, start, end);

                            List<PointLatLng> put = new List<PointLatLng>();
                            foreach (var item in route.Shape)
                            {
                                PointLatLng pt = new PointLatLng(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude));
                                put.Add(pt);
                            }

                            if (App.enteredUser != null)
                            {
                                Searches searches = new Searches()
                                {
                                    address_start = address1Tb.Text,
                                    address_finish = address2Tb.Text,
                                    route_distance = route.TotalDistance,
                                    route_time = route.TotalTime,
                                    time_finish = DateTime.Now.AddSeconds(route.TotalTime),
                                    user_id = App.enteredUser.id
                                };
                                App.context.Searches.Add(searches);
                                App.context.SaveChanges();
                            }
                            var floatTime = Math.Round(route.TotalTime, 0);
                            var dateTime = DateTime.MinValue.AddSeconds(floatTime);
                            if (dateTime.Hour >= 1)
                            {
                                timeTbl.Text = dateTime.Hour + " ч " + dateTime.Minute + " м";
                            }
                            else
                            {
                                timeTbl.Text = dateTime.Minute + " мин";
                            }
                            distanceTbl.Text = Math.Round(route.TotalDistance, 0) + " м";
                            departureTbl.Text = address1Tb.Text;
                            arrivalTbl.Text = address2Tb.Text;

                            GMapRoute r = new GMapRoute(put);
                            gMapControl1.Markers.Add(r);
                        }
                        else
                        {
                            MessageBox.Show("Выберите тип маршрута!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Введите адреса в поиск!");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Произошла ошибка при построении маршрута! Попробуйте снова или переформулируйте запрос!");
            }

        }
        string address1;
        string address2;
        private void swapBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            address1 = address1Tb.Text;
            address2 = address2Tb.Text;
            address1Tb.Text = address2;
            address2Tb.Text = address1;
        }
        bool isGasStationsVisible = false;
        GMapMarker marker;
        private void gasStationsBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isGasStationsVisible == false)
            {
                string json = File.ReadAllText(@"C:\Users\igoro\source\repos\GeoAvto\Resources\Maps\export.geojson");
                JObject obj = JObject.Parse(json);
                JArray features = (JArray)obj["features"];
                // Отображение всех заправок на карте
                foreach (JObject feature in features)
                {
                    JArray coordinates = (JArray)feature["geometry"]["coordinates"];
                    double lng = (double)coordinates[0];
                    double lat = (double)coordinates[1];
                    PointLatLng point = new PointLatLng(lat, lng);
                    marker = new GMapMarker(point);
                    var gasStationIcon = new BitmapImage();
                    gasStationIcon.BeginInit();
                    gasStationIcon.UriSource = new Uri("C:\\Users\\igoro\\source\\repos\\GeoAvto\\Resources\\Images\\gas-station.png");
                    gasStationIcon.EndInit();
                    marker.Shape = new System.Windows.Controls.Image
                    {
                        Source = gasStationIcon,
                        Width = 20,
                        Height = 20,
                    };
                    marker.Tag = "gasStation";
                    gMapControl1.Markers.Add(marker);
                    marker.Shape.MouseDown += Shape_MouseDown;
                }
                isGasStationsVisible = true;
            }
            else
            {

                foreach (GMapMarker marker in gMapControl1.Markers.Where(m => m.Tag != null))
                {
                    marker.Clear();
                }
                isGasStationsVisible = false;
            }
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
                    textBlock.Text = dateTime.Hour + " ч " + dateTime.Minute;
                }
                else
                {
                    textBlock.Text = dateTime.Minute + " мин";
                }
            }
        }

        private void profileBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (App.enteredUser != null)
            {
                ProfileWindow profileWindow = new ProfileWindow();
                profileWindow.ShowDialog();
            }
            else
            {
                var result = MessageBox.Show("Вы не вошли в приложение, авторизоваться сейчас?", "", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    AuthRegWindow authRegWindow = new AuthRegWindow();
                    authRegWindow.ShowDialog();
                    if (authRegWindow.DialogResult == true)
                    {

                    }
                }
                else
                {

                }
            }
        }
        private void Shape_MouseDown(object sender, MouseButtonEventArgs e)
        {
            marker = gMapControl1.Markers.FirstOrDefault(m => m.Shape == e.Source);
            if (marker != null)
            {
                PointLatLng point = marker.Position;
                JObject geoJson = JObject.Parse(File.ReadAllText(@"C:\Users\igoro\source\repos\GeoAvto\Resources\Maps\export.geojson"));
                JArray features = (JArray)geoJson["features"];
                foreach (JObject feature in features)
                {
                    JArray coordinates = (JArray)feature["geometry"]["coordinates"];
                    if (Math.Abs((double)coordinates[1] - point.Lat) < 0.000001) // сравнение широты с точностью до 6 знаков после запятой
                    {
                        string name = (string)feature["properties"]["name"];
                        if (name != null)
                        {
                            if (name == "ЕКА")
                            {
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "49.11",
                                    fuelMed = "53.96"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                            else if (name == "Газпромнефть")
                            {
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "49.00",
                                    fuelMed = "53.90"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                            else if (name == "Лукойл")
                            {
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "49.21",
                                    fuelMed = "55.18"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                            else if (name == "Роснефть")
                            {
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "48.30",
                                    fuelMed = "54.70"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                            else if (name == "Teboil")
                            {
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "47.99",
                                    fuelMed = "55.99"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                            else if (name == "Татнефть")
                            {
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "47.99",
                                    fuelMed = "52.69"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                            else if (name == "Shell")
                            {
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "49.89",
                                    fuelMed = "56.79"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                            else if (name == "Нефтьмагистраль")
                            {
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "49.09",
                                    fuelMed = "54.89"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("АЗС: " + name, name);
                                GasStation gasStation = new GasStation()
                                {
                                    name = name,
                                    fuelLow = "Нет данных",
                                    fuelMed = "Нет данных"
                                };
                                CurrentGasStationWindow currentGasStationWindow = new CurrentGasStationWindow(gasStation);
                                currentGasStationWindow.ShowDialog();
                            }
                        }
                        break;
                    }
                }
                //MessageBox.Show($"Широта: {point.Lat}, Долгота: {point.Lng}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
