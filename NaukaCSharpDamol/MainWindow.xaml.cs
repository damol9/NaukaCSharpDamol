//#undef DEBUG
#define DEBUG
using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using MySql.Data.MySqlClient;
//using System.IO;

namespace NaukaCSharpDamol
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int timeleft;
        public static bool isWindow2Active = false;
        private string login, password;
        private static DispatcherTimer Timer1;
        private static string connStr = "server=localhost;user=root;database=tsl;port=3306;password=;";
        public static MySqlDataReader rdr;
        private static MySqlConnection conn = new MySqlConnection(connStr);
        private string LoggedAccount;
        public MainWindow()
        {
            //if (!File.Exists(Directory.GetCurrentDirectory() + "\\MySql.Data.dll"))
            //{
            //    MessageBoxResult result = MessageBox.Show("Brak pliku MySql.Data.dll");
            //    if (result==MessageBoxResult.OK)Environment.Exit(0);
            //}
            //if (!File.Exists(Directory.GetCurrentDirectory() + "\\MySql.Web.dll"))
            //{
            //    MessageBox.Show("Brak pliku MySql.Web.dll");
            //}

            Window1 Okno = new Window1();
            Okno.Show();
            MySQL_Init(Okno);
            

        }
        private void MySQL_Init(Window1 uchwyt)
        {
            try
            {
                conn.Open();
                Thread.Sleep(1000);
                uchwyt.Close();

            }
            catch (Exception ex)
            {
                #if (DEBUG)
                    MessageBox.Show(ex.ToString(), "DEBUG INFORMATION");
                #endif

                MessageBox.Show("Can't connect to MySQL database. Exiting.");
                Environment.Exit(0);
            }
        }
        public static string MySQL_LoginQuery(string query)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                rdr = cmd.ExecuteReader();
                if (rdr.Read()) return rdr[0].ToString();
                else return "";


            }
            catch (Exception ex)
            {
                #if (DEBUG)
                    MessageBox.Show(ex.ToString(), "DEBUG INFORMATION");
                    MessageBox.Show(ex.GetType().ToString(), "Error - debug");
                #endif
                MessageBox.Show("There was an error when processing your request.", "Error");
                return "";

            }
        }
        public static void MySQL_UpdateQuery(string query)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                #if (DEBUG)
                    MessageBox.Show(ex.ToString(), "DEBUG INFORMATION");
                    MessageBox.Show(ex.GetType().ToString(), "Error - debug");
                #endif
                MessageBox.Show("There was an error when processing your request.", "Error");

            }  
        }
        private void Odliczanie(Object source, EventArgs e)
        {
            if (timeleft > 0)
            {
                TimerUpdate();
                timeleft--;
            }
            else
            {
                Licznik.Content = "";
                Timer1.Stop();
                Button1.IsEnabled = true;
            }
        }
        private void TimerUpdate()
        {
            Licznik.Content = timeleft;
        }
        private void Start_Odliczania()
        {
            timeleft = 5;
            Timer1 = new DispatcherTimer();
            Timer1.Tick += new EventHandler(Odliczanie);
            Timer1.Interval = new TimeSpan(0, 0, 1);
            Timer1.Start();
            Button1.IsEnabled = false;
        }

        private void Label_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isWindow2Active)
            {
                Window2 okno2 = new Window2();
                isWindow2Active = true;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Button1.IsEnabled = true;
            LogoutButton.IsEnabled = false;
            LoggedAs.Content = "---";
            LoggedAccount = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string DBResult_login, DBResult_password;
            login = TextBoxA.Text;
            password = PasswordBox1.Password;
            if (login == "" || password == "")
            {
                MessageBox.Show("Żadne z pól nie może być puste.", "Błąd");
            }
            else
            {
                DBResult_login = MySQL_LoginQuery("SELECT konta.login FROM konta WHERE konta.login = \"" + login + "\";");
                rdr.Close();
                if (DBResult_login == "")
                {
                    Start_Odliczania();
                    MessageBox.Show("Nie ma takiego użytkownika.", "Błąd");

                }
                else
                {
                    password = Encryption.EncryptSHA512Managed(password);
                    DBResult_password = MySQL_LoginQuery("SELECT konta.password FROM konta WHERE konta.login = \"" + login + "\";");
                    rdr.Close();
                    if (DBResult_password == password && DBResult_login == login)
                    {
                        MySQL_UpdateQuery("UPDATE `konta` SET `LastLogin` = CURRENT_TIMESTAMP WHERE `konta`.`login` = \""+login+"\";");
                        MessageBox.Show("Zalogowano.", "Informacja");
                        Button1.IsEnabled = false;
                        LogoutButton.IsEnabled = true;
                        LoggedAs.Content = login;
                        LoggedAccount = login;
                        TextBoxA.Text = "";
                        PasswordBox1.Password = "";
                    }
                    else
                    {
                        Start_Odliczania();
                        TextBoxA.Text = "";
                        PasswordBox1.Password = "";
                        MessageBox.Show("Błędne hasło.", "Błąd");
                    }
                }
            }
        }

    }
}