using PasswordGeneratorCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordGeneratorCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //The list
        List<char> chars = new List<char> { };
        //Numbers
        List<char> n = new List<char> { };
        //Lower case
        List<char> l = new List<char> { };
        //Upper case
        List<char> u = new List<char> { };

        List<CheckBox> chList = new List<CheckBox> { };

        //List<char> s = new List<char> { };
        public MainWindow()
        {
            InitializeComponent();
            //Add numbers to list
            for (char i = '0'; i <= '9'; i++)
            {
                n.Add(i);
                chars.Add(i);
            }
            //Add lower case to list
            for (char i = 'a'; i <= 'z'; i++)
            {
                l.Add(i);
                chars.Add(i);
            }
            //Add upper case to list
            for (char i = 'A'; i <= 'Z'; i++)
            {
                u.Add(i);
                chars.Add(i);
            }

            //Special characters list
            var specials = new Dictionary<char, string>
            {
                {' ', "White space"},
                { '@', "At sign"},
                {'"', "Double quote" },
                {'\\', "Backslash"},
                {'#', "Hashtag"},
                {'$', "Dollar Sign"},
                {'&', "Ampersand"},
                {'\'', "Single quote"},
                {'(', "Opening parenthesis"},
                {')', "Closing parenthesis"},
                {'*', "Asterisk"},
                {',', "Comma"},
                {'+', "Plus"},
                {'-', "Minus"},
                {'.', "Period"},
                {'/', "Slash"},
                {':', "Colon"},
                {';', "Semicolon"},
                {'<', "Less than"},
                {'=', "Equals"},
                {'>', "Greater than"},
                {'?', "Question mark"},
                {'[', "Opening square bracket"},
                {']', "Closing square bracket"},
                {'^', "Caret"},
                {'_', "Underscore"},
                {'{', "Opening curly bracket"},
                {'}', "Closing curly bracket"},
                {'|', "Vertical bar"},
                {'~', "Tilde"}
            };
            //Create the checkboxes for the specials list
            foreach (var c in specials)
            {
                //// Checkbox
                /// A local checkbox for ever char in list
                /// 
                //CheckBox checkBox = new CheckBox();
                CheckBox checkBox = new CheckBox();
                checkBox.HorizontalAlignment = HorizontalAlignment.Left;
                //Had to add a condition here because of the RecgonizeAccessKey problem
                if (c.Key == '_')
                {
                    checkBox.Content = "__" + "\t" + c.Value;
                }
                else
                {
                    checkBox.Content = c.Key + "\t" + c.Value;
                }

                //Set all checkboxes to checkedm
                checkBox.IsChecked = true;

                specialsMenuItem.Items.Add(checkBox);

                //Event handler for checked and unchecked
                checkBox.Checked += new RoutedEventHandler(checkBoxChecked);
                checkBox.Unchecked += new RoutedEventHandler(checkBoxUnChecked);

                //Add special characters to list
                chars.Add(c.Key);
            }

            //labelChars.Content = string.Join("", chars.ToArray());
            //Checkbox checked event
            void checkBoxChecked(object sender, RoutedEventArgs e)
            {
                CheckBox ch = sender as CheckBox;
                chars.Add(ch.Content.ToString()[0]);
            }
            //Checkbox unchecked event
            void checkBoxUnChecked(object sender, RoutedEventArgs e)
            {
                CheckBox ch = sender as CheckBox;
                chars.Remove(ch.Content.ToString()[0]);
            }

            using (var db = new AccountsDbContext())
            {
                data.ItemsSource = db.Passwords.ToList();
            }
        }

        public void Refresh_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AccountsDbContext())
            {
                data.ItemsSource = db.Passwords.ToList();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AccountsDbContext())
            {
                db.Passwords.Remove((AccountModel)data.SelectedValue);
                db.SaveChanges();

                data.ItemsSource = db.Passwords.ToList();
            }
        }
        private void Delete_Down(object sender, MouseButtonEventArgs e)
        {
            Button b = sender as Button;
            //b.Background = new SolidColorBrush(Color.FromArgb(71, 57, 74, 29));
        }
        private void Delete_Over(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            //b.Background = new SolidColorBrush(Color.FromArgb(74, 60, 57, 29));
        }
        private void Delete_Out(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            //b.Background = new SolidColorBrush(Color.FromArgb(61, 50, 54, 24));
        }
        private bool IsNumber(string Text)
        {
            int output;
            return int.TryParse(Text, out output);
        }
        private void OnKeyDown(object sender, TextCompositionEventArgs e)
        {
            if (IsNumber(e.Text) == false)
            {
                e.Handled = true;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            b.Background = new SolidColorBrush(Color.FromArgb(255, 153, 153, 153));

            using (var db = new AccountsDbContext())
            {
                db.Passwords.Update((AccountModel)data.SelectedValue);
                db.SaveChanges();

                data.ItemsSource = db.Passwords.ToList();
            }
        }
        private void Save_Down(object sender, MouseButtonEventArgs e)
        {
            Button b = sender as Button;
            b.Background = new SolidColorBrush(Color.FromArgb(255, 153, 153, 153));
        }
        private void Save_Over(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            b.Background = new SolidColorBrush(Color.FromArgb(255, 85, 85, 85));
        }
        private void Save_Out(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            b.Background = new SolidColorBrush(Color.FromArgb(255, 23, 23, 255));
        }

        private void w_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            //Generate password
            String generatePassword()
            {
                StringBuilder p = new StringBuilder();

                var rand = new Random();

                for (int i = 0; i <= int.Parse(passLenTxt.Text) - 1; i++)
                {
                    int index = rand.Next(chars.Count);
                    p.Append(chars[index]);
                }

                //Recurse generatePassword() until password is strong/valid
                Regex r = new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[ !\""#$%&'()*+,\-./:;<=>?@\[\]^_`{|}~])");
                if (r.IsMatch(p.ToString()))
                    return p.ToString();
                else
                {
                    return generatePassword();
                }
            }
            using (var db = new AccountsDbContext())
            {
                var a = (AccountModel)data.SelectedValue;
                a.Password = generatePassword();
                db.Passwords.Update(a);
            }
        }
        private void searchText_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox search = sender as TextBox;
            using (var db = new AccountsDbContext())
            {
                data.ItemsSource = db.Passwords.Where(w => w.Website.ToLower().Contains(search.Text.ToLower())).ToList();
            }
        }

    }
}
