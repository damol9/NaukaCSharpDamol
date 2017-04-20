#undef DEBUG
//#define DEBUG
using System;
using System.Windows;
using System.Windows.Threading;
using System.Data;
using MySql.Data;
using System.Threading;
using MySql.Data.MySqlClient;

namespace NaukaCSharpDamol
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int timeleft;
        private string login, password;
        private static DispatcherTimer Timer1;
        static string connStr = "server=localhost;user=root;database=tsl;port=3306;password=;";
        MySqlDataReader rdr;
        public MySqlConnection conn = new MySqlConnection(connStr);

        public MainWindow()
        {
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
        private string MySQL_Query(string query)
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
                DBResult_login = MySQL_Query("SELECT konta.login FROM konta WHERE konta.login = \"" + login + "\";");
                rdr.Close();
                if (DBResult_login == "")
                {
                    Start_Odliczania();
                    MessageBox.Show("Nie ma takiego użytkownika.", "Błąd");

                }
                else
                {
                    password = Encryption.EncryptSHA512Managed(password);
                    DBResult_password = MySQL_Query("SELECT konta.password FROM konta WHERE konta.login = \"" + login + "\";");
                    rdr.Close();
                    if (DBResult_password == password && DBResult_login == login)
                    {
                        MessageBox.Show("Zalogowano.", "Informacja");
                    }
                    else
                    {
                        Start_Odliczania();
                        MessageBox.Show("Błędne hasło.", "Błąd");  
                    }
                }
            }
        }

    }
}