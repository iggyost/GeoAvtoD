using GeoAvto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddNewCardWindow.xaml
    /// </summary>
    public partial class AddNewCardWindow : Window
    {
        public AddNewCardWindow()
        {
            InitializeComponent();
        }
        Regex isDigit = new Regex("^[0-9]+$");
        bool firstBlock = false;
        bool secondBlock = false;
        bool thirdBlock = false;
        bool fourthBlock = false;
        bool isAllBlockFourDigits = false;
        private void firstBlockCardNumberTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isDigit.IsMatch(firstBlockCardNumberTb.Text))
            {
                if (firstBlockCardNumberTb.Text.Length == 4)
                {
                    secondBlockCardNumberTb.IsEnabled = true;
                    secondBlockCardNumberTb.Focus();
                }
                else
                {
                    secondBlockCardNumberTb.Text = string.Empty;
                    thirdBlockCardNumberTb.Text = string.Empty;
                    fourthBlockCardNumberTb.Text = string.Empty;
                    secondBlockCardNumberTb.IsEnabled = false;
                    thirdBlockCardNumberTb.IsEnabled = false;
                    fourthBlockCardNumberTb.IsEnabled = false;
                }
                firstBlock = true;
            }
            else
            {
                firstBlock = false;
            }
        }

        private void secondBlockCardNumberTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isDigit.IsMatch(secondBlockCardNumberTb.Text))
            {
                if (secondBlockCardNumberTb.Text.Length == 4)
                {
                    thirdBlockCardNumberTb.IsEnabled = true;
                    thirdBlockCardNumberTb.Focus();
                }
                else
                {
                    thirdBlockCardNumberTb.Text = string.Empty;
                    fourthBlockCardNumberTb.Text = string.Empty;
                    thirdBlockCardNumberTb.IsEnabled = false;
                    fourthBlockCardNumberTb.IsEnabled = false;
                }
                secondBlock = true;
            }
            else
            {
                secondBlock = false;
            }
            if (secondBlockCardNumberTb.Text.Length == 0)
            {
                if (Keyboard.IsKeyDown(Key.Back))
                {
                    firstBlockCardNumberTb.Focus();
                }
            }
        }

        private void thirdBlockCardNumberTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isDigit.IsMatch(thirdBlockCardNumberTb.Text))
            {
                if (thirdBlockCardNumberTb.Text.Length == 4)
                {
                    fourthBlockCardNumberTb.IsEnabled = true;
                    fourthBlockCardNumberTb.Focus();
                }
                else
                {
                    fourthBlockCardNumberTb.Text = string.Empty;
                    fourthBlockCardNumberTb.IsEnabled = false;
                }
                thirdBlock = true;
            }
            else
            {
                thirdBlock = false;
            }
            if (thirdBlockCardNumberTb.Text.Length == 0)
            {
                if (Keyboard.IsKeyDown(Key.Back))
                {
                    secondBlockCardNumberTb.Focus();
                }
            }
        }

        string validationLength;
        private void fourthBlockCardNumberTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isDigit.IsMatch(fourthBlockCardNumberTb.Text))
            {
                if (fourthBlockCardNumberTb.Text.Length == 4)
                {
                    validationLength = string.Empty;
                    isAllBlockFourDigits = true;
                }
                else
                {
                    validationLength = "В каждом блоке должно быть 4 цифры! ";
                    isAllBlockFourDigits = false;
                }
                fourthBlock = true;
            }
            else
            {
                fourthBlock = false;
            }
            if (fourthBlockCardNumberTb.Text.Length == 0)
            {
                if (Keyboard.IsKeyDown(Key.Back))
                {
                    thirdBlockCardNumberTb.Focus();
                }
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addCardBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dateTime = DateTime.Now.Year.ToString();
                var year = dateTime.Remove(0, 2);
                if (isAllBlockFourDigits == true && int.Parse(monthCardTb.Text) <= 12 && int.Parse(monthCardTb.Text) > 0 && int.Parse(yearCardTb.Text) >= int.Parse(year))
                {
                    var cvvCode = int.Parse(codeCardsTb.Text);
                    var first = firstBlockCardNumberTb.Text;
                    var second = secondBlockCardNumberTb.Text;
                    var third = thirdBlockCardNumberTb.Text;
                    var fourth = fourthBlockCardNumberTb.Text;
                    Cards newCard = new Cards()
                    {
                        card_number = first + " " + second + " " + third + " " + fourth,
                        cvv_code = cvvCode.ToString(),
                        card_date = monthCardTb.Text + "/" + yearCardTb.Text,
                        cardtypes_id = 1,
                        user_id = App.enteredUser.id
                    };
                    App.context.Cards.Add(newCard);
                    App.context.SaveChanges();
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не все поля заполнены верно!", "Внимание!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте введенные данные!", "Ошибка!");
            }
        }
    }
}
