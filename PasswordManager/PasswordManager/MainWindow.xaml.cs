using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PasswordManager
{
    public class CButton : Border
    {
        public static int rowIndex;
        public static DataGrid data;

        public TextBlock b = new TextBlock();
        public Label lbl = new Label();

        public CButton()
        {
            Width = 100;
            Height = 40;

            Margin = new Thickness(10);
            BorderBrush = Brushes.White;
            BorderThickness = new Thickness(1);

            lbl.Foreground = Brushes.White;
            lbl.HorizontalContentAlignment = HorizontalAlignment.Center;

            Child = b;

            b.MouseEnter += mouseEnter;
            b.MouseLeave += mouseLeave;
            b.MouseDown += mouseDown;
            b.MouseUp += mouseUp;
        }

        private void mouseEnter(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = e.Source as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#6E6596");
        }
        private void mouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = e.Source as TextBlock;
            btn.Background = Background;
        }
        public void mouseDown(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = sender as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#868396");
        }
        private void mouseUp(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = sender as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#6E6596");
        }
    }
    public class CopyBtn : CButton
    {

        public CopyBtn()
        {
            lbl.Content = "Copy";

            b.MouseDown += copyMouseDown;
            b.Inlines.Add(lbl);
        }

        private void copyMouseDown(object sender, RoutedEventArgs e)
        {
            //mouseDown(sender, (MouseEventArgs)e);
            using (var db = new AccountsDbContext())
            {
                try
                {
                    var s = (AccountModel)data.Items[rowIndex];
                    Clipboard.SetText(s.Password);
                }
                catch (Exception ex)
                {
                    Clipboard.SetText("");
                }
            }
        }

    }
    public class GenerateBtn : CButton
    {
        //The list
        public static List<char> chars;
        //Numbers
        public static List<char> n;
        //Lower case
        public static List<char> l;
        //Upper case
        public static List<char> u;

        public static TextBox passLenTxt;

        public GenerateBtn()
        {
            lbl.Content = "Generate";
            b.Inlines.Add(lbl);

            b.MouseDown += generateMouseDown;
        }

        //Generate password
        private String generatePassword()
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

            return p.ToString();
        }

        private void generateMouseDown(object sender, RoutedEventArgs e)
        {
            mouseDown(sender, (MouseEventArgs)e);
            //chars.Clear();
            using (var db = new AccountsDbContext())
            {
                try
                {
                    AccountModel acc = (AccountModel)data.Items[rowIndex];
                    acc.Password = generatePassword();
                    db.Passwords.Update(acc);
                }
                catch (Exception ex)
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
    }
    public class SaveBtn : CButton
    {

        public SaveBtn()
        {
            lbl.Content = "Save";
            b.Inlines.Add(lbl);

            b.MouseDown += saveMouseDown;
        }

        private void saveMouseDown(object sender, RoutedEventArgs e)
        {

            using (var db = new AccountsDbContext())
            {
                data.ItemsSource = db.Passwords.ToList();
            }
            mouseDown(sender, (MouseEventArgs)e);
        }
    }
    public class DeleteBtn : CButton
    {

        public DeleteBtn()
        {
            lbl.Content = "Delete";
            b.Inlines.Add(lbl);

            b.MouseDown += deleteMouseDown;
        }

        private void deleteMouseDown(object sender, RoutedEventArgs e)
        {
            mouseDown(sender, (MouseEventArgs)e);
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
            catch (Exception ex)
            {
                MessageBox.Show("Error: account does not exist in database", "Account Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
    public class RefreshBtn : CButton
    {
        public RefreshBtn()
        {
            lbl.Content = "Refresh";

            b.Inlines.Add(lbl);

            b.MouseLeave += refreshMouseLeave;
        }

        private void refreshMouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            TextBlock btn = e.Source as TextBlock;
            btn.Background = (Brush)bc.ConvertFrom("#384954");
        }
    }
    public class CSpecial : TextBlock
    {
        public bool IsSelected;

        public CSpecial()
        {
            Padding = new Thickness(15, 5, 15, 5);

            Foreground = Brushes.LightGray;
            MouseLeave += mouseLeave;
            Loaded += mouseLeftButtonDown;
            MouseLeftButtonDown += mouseLeftButtonDown;
            MouseEnter += mouseEnter;
        }

        private void mouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (IsSelected)
            {
                var bc = new BrushConverter();
                Background = null;
                IsSelected = false;
            }
            else
            {
                var bc = new BrushConverter();
                Background = (Brush)bc.ConvertFrom("#2F3D2F");
                IsSelected = true;
            }
        }
        private void mouseLeave(object sender, RoutedEventArgs e)
        {
            if (!IsSelected)
            {
                var bc = new BrushConverter();
                Background = null;
            }
            else
            {
                var bc = new BrushConverter();
                Background = (Brush)bc.ConvertFrom("#2F3D2F");
            }
        }
        private void mouseEnter(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();
            Background = (Brush)bc.ConvertFrom("#3C394A");
        }
    }
    public class TBItem : CSpecial
    {
        //outside sources from class. using wpf elements
        public static List<char> chars;
        public static StackPanel lvSpecials;

        Border b = new Border();

        public TBItem(char c, string v)
        {
            

            Text = c + "\t" + v;

            b.BorderBrush = Brushes.DarkGray;
            b.BorderThickness = new Thickness(1);
            b.Margin = new Thickness(5);
            b.Child = this;

/*            lvSpecials.Children.Add(new ListViewItem()
            {
                Content = Text
            });*/
        }

        
        
        public static void CheckAll(object sender, RoutedEventArgs e)
        {
            foreach (Border c in lvSpecials.Children.OfType<Border>())
            {
                TBItem b = (TBItem)c.Child;
                b.IsSelected = true;
                var bc = new BrushConverter();
                b.Background = (Brush)bc.ConvertFrom("#2F3D2F");
            }
        }
        public static void UncheckAll(object sender, RoutedEventArgs e)
        {
            foreach (Border c in lvSpecials.Children.OfType<Border>())
            {
                TBItem b = (TBItem)c.Child;
                b.IsSelected = false;
                b.Background = null;
            }
        }
        public void lvChange(object sender, RoutedEventArgs e)
        {
            chars.Clear();
            StringBuilder str = new StringBuilder();
            foreach (TBItem lvi in lvSpecials.Children.OfType<TBItem>())
            {
                if (lvi.IsSelected)
                    str.Append(lvi.Text.ToString()[0]);
            }
            chars = str.ToString().ToList();
        }
    }
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

        List<TBItem> cbList = new List<TBItem> { };

        //single variables for selection, not from db
        int rowIndex;
        int indexDb;
        string notes;
        string website;
        string password;
 
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

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                using (var db = new AccountsDbContext())
                {
                    data.ItemsSource = db.Passwords.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            //TBItem.lvSpecials = lvSpecials;
            TBItem.chars = chars;

            CButton.data = data;
            CButton.rowIndex = rowIndex;

            GenerateBtn.chars = chars;
            GenerateBtn.n = n;
            GenerateBtn.l = l;
            GenerateBtn.u = u;
            GenerateBtn.passLenTxt = passLenTxt;

            all.Content = "(Select All)";
            all.IsChecked = true;

            all.Checked += new RoutedEventHandler(TBItem.CheckAll);
            all.Unchecked += new RoutedEventHandler(TBItem.UncheckAll);


            //Create the checkboxes for the specials list
            foreach (var c in specials)
            {
                TBItem item = new TBItem(c.Key, c.Value);

                GenerateBtn.chars.Add(c.Key);
                cbList.Add(item);
            }
            //lvSpecials.ItemsSource = cbList;
        }

/*        private void lvChange(object sender, RoutedEventArgs e)
        {
            TBItem item = sender as TBItem;

            chars.Clear();
            StringBuilder str = new StringBuilder();
            foreach (TBItem lvi in lvSpecials.Children.OfType<TBItem>())
            {
                if(lvi.IsSelected)
                    str.Append(lvi.Text.ToString()[0]);
            }
            chars = str.ToString().ToList();
        }*/
        
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

        private void windowMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
        
        private void searchText_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox search = sender as TextBox;
            using (var db = new AccountsDbContext())
            {
                data.ItemsSource = db.Passwords.Where(w => w.Website.ToLower().Contains(search.Text.ToLower())).ToList();
            }
        }
        
        private void rowEnter(object sender, MouseEventArgs e)
        {
            var row = e.Source as DataGridRow;
            CButton.rowIndex = row.GetIndex();

            //Label lbl = (Label)selectID;
            try
            {
                AccountModel acc = (AccountModel)data.Items[rowIndex];
                indexDb = acc.Id;
                website = acc.Website;
                notes = acc.Notes;
                password = acc.Password;

                //lbl.Content = "ID: " + indexDb;
            }
            catch
            {
                //lbl.Content = "ID: ";
            }
            
        }
    }
}
