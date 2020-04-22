using PasswordGeneratorCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        List<ListViewItem> cbList = new List<ListViewItem> { };

        int rowIndex;
        int indexDb;
        string notes;
        string website;
        string password;
 
        //Generate password
        String generatePassword()
        {
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
        public class SpecialsModel
        {
            public char character { get; set; }
            public string desc { get; set; }

            public SpecialsModel(char character, string desc)
            {
                this.character = character;
                this.desc = desc;
            }
        }
        //Special characters list 
        Dictionary<char, string> specials = new Dictionary<char, string>()
            {
                {' ' , "White space"},
                { '@', "At sign"},
                {'"', "Double quote"},
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
        //List<char> s = new List<char> { };
        public MainWindow()
        {
            InitializeComponent();
            //((INotifyCollectionChanged)lvSpecials.ItemsSource).CollectionChanged += new NotifyCollectionChangedEventHandler(lvChange);
            try
            {
                using (var db = new AccountsDbContext())
                {
                    data.ItemsSource = db.Passwords.ToList();
                }
                //int rowi = data.Items.IndexOf(data.Items.Count - 1);
                //DataGridRow row = (DataGridRow)data.Items[3];
                //row.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            //CheckBox all = new CheckBox();
            all.Content = "All";
            all.IsChecked = true;

            all.Checked += new RoutedEventHandler(CheckAll);
            all.Unchecked += new RoutedEventHandler(UncheckAll);

            //Create the checkboxes for the specials list
            foreach (var c in specials)
            {
                ListViewItem item = new ListViewItem();

                item.Content = c.Key + "\t" + c.Value;
                item.IsSelected = true;

                chars.Add(c.Key);
                cbList.Add(item);
            }
            lvSpecials.ItemsSource = cbList;
        }
        private void CheckAll(object sender, RoutedEventArgs e)
        {
            foreach (var c in cbList)
            {
                c.IsSelected = true;
            }
        }
        private void UncheckAll(object sender, RoutedEventArgs e)
        {
            foreach (var c in cbList)
            {
                c.IsSelected = false;
            }
        }
        private void lvChange(object sender, RoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;

            chars.Clear();
            StringBuilder str = new StringBuilder();
            foreach (ListViewItem lvi in lvSpecials.SelectedItems)
            {
                //chars.Add(lvi.Content.ToString()[0]);
                str.Append(lvi.Content.ToString()[0]);
            }
            chars = str.ToString().ToList();
        }
        public void Refresh_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AccountsDbContext())
            {
                data.ItemsSource = db.Passwords.ToList();
            }
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

        private void Save_Click(object sender, MouseEventArgs e)
        {
            using (var db = new AccountsDbContext())
            {
                try
                {
                    AccountModel acc = (AccountModel)data.Items[rowIndex];
                    //acc.Id = indexDb;
                    //acc.Notes = notes; 
                    //acc.Website = website;
                    //acc.Password = password;
                    db.Passwords.Update(acc);
                    //db.Entry(acc).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //db.Passwords.Add(new AccountModel());
                }
                
                data.ItemsSource = db.Passwords.ToList();
            }
        }
        //private DataRowView being = null;
        private void cellChange(object sender, TextCompositionEventArgs e)
        {
            //DataRowView row = e.Row.Item as DataRowView;

            //AccountModel acc = (AccountModel)row.ite;
            //website = acc.Website;
            //being = row;
            //TextBox b = sender as TextBox;
            //row.EndEdit();

            //AccountModel acc = (AccountModel)e.Text
            lblWebsite.Content = e.Text;
            //website = e.Column.ToString();
        }
        //private void test(object sender, TextChangedEventArgs e)
        //{
        //    TextBox b = sender as TextBox;
        //    lblWebsite.Content = e.Source;
        //}
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
            btnDown(sender, (MouseEventArgs)e);
            //chars.Clear();
            using (var db = new AccountsDbContext())
            {
                try
                {
                    AccountModel acc = (AccountModel)data.Items[rowIndex];
                    acc.Password = generatePassword();
                    db.Passwords.Update(acc);
                }
                catch(Exception ex)
                {
                    AccountModel acc = new AccountModel()
                    {
                        Notes = "",
                        Website = "",
                        Password = generatePassword()
                    };
                    db.Passwords.Update(acc);
                    db.SaveChanges();
                }
                data.ItemsSource = db.Passwords.ToList();
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
        private void btnSaveLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = e.Source as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#2F3D2F");
        }
        private void btnGenLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = e.Source as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#3D382C");
        }
        private void btnEnter(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = e.Source as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#6E6596");
        }
        private void btnDown(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = sender as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#868396");
        }
        private void btnUp(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = sender as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#6E6596");
        }
        private void btnCopyLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = e.Source as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#2C323D");
        }
        private void btnDeleteLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = e.Source as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#3D2C35");
        }
        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            btnDown(sender, (MouseEventArgs)e);
            using (var db = new AccountsDbContext())
            {
                try
                {
                    var s = (AccountModel)data.Items[rowIndex];
                    Clipboard.SetText(s.Password);
                }
                catch(Exception ex)
                {
                    Clipboard.SetText("");
                }
            }
        }
        private void DeleteAccount(object sender, RoutedEventArgs e)
        {
            btnDown(sender, (MouseEventArgs)e);
            try
            {
                MessageBoxResult res = MessageBox.Show("Are you sure you want to delete Id " + ((AccountModel)data.Items[rowIndex]).Id, "Deleting Account", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    using (var db = new AccountsDbContext())
                    {
                        var s = (AccountModel)data.Items[rowIndex];
                        db.Passwords.Remove(s);
                        db.SaveChanges();

                        data.ItemsSource = db.Passwords.ToList();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: account does not exist in database", "Account Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void rowEnter(object sender, MouseEventArgs e)
        {
            var row = e.Source as DataGridRow;
            rowIndex = row.GetIndex();

            Label lbl = (Label)selectID;
            try
            {
                AccountModel acc = (AccountModel)data.Items[rowIndex];
                indexDb = acc.Id;
                website = acc.Website;
                notes = acc.Notes;
                password = acc.Password;

                lbl.Content = "ID: " + indexDb;
            }
            catch
            {
                lbl.Content = "ID: ";
            }
            
        }
    }
}
