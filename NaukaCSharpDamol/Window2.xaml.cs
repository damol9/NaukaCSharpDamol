using System;
using System.Windows;

namespace NaukaCSharpDamol
{
    /// <summary>
    /// Logika interakcji dla klasy Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        private void Window2_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.isWindow2Active=false;
        }

        private bool isPasswordStrong(string password)
        {
            if(password.Length>=8 && password.Length<=20)
            {
                bool tCapital = false, tNormal = false, tNumber = false;
                for(int i = 0; i < password.Length; i++)
                {
                    if (password[i] >= 'A' && password[i] <= 'Z')
                    {
                        tCapital = true;
                    }       
                    else if (password[i] >= 'a' && password[i] <= 'z')
                    {
                        tNormal = true;
                    }
                    else if (password[i] >= '0' && password[i] <= '9')
                    {
                        tNumber = true;
                    }
                }
                //MessageBox.Show(tCapital.ToString()+tNormal.ToString()+tNumber.ToString());
                if (tCapital == true && tNormal == true && tNumber == true)
                    return true;
                else return false;
            }
            else return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = Login_Window2.Text;
            string password = Password_Window2.Password;
            if (login != "" && password != "" && PasswordRepeat_Windows2.Password != "")
            {
                string DBResult_login = MainWindow.MySQL_LoginQuery("SELECT konta.login FROM konta WHERE konta.login = \"" + login + "\";");
                MainWindow.rdr.Close();
                if (DBResult_login.ToLower() != login.ToLower())
                {
                    if (password == PasswordRepeat_Windows2.Password)
                    {
                        if (isPasswordStrong(password))
                        {
                            if (RulesCheck_Windows2.IsChecked == true)
                            {
                                try
                                {
                                    MainWindow.MySQL_UpdateQuery("INSERT INTO `konta` (`id`, `login`, `password`, `DateRegistered`, `LastLogin`) VALUES(NULL, '" + Login_Window2.Text + "', '" + Encryption.EncryptSHA512Managed(Password_Window2.Password) + "', CURRENT_TIMESTAMP, '')");
                                    MessageBox.Show("Zarejestrowano pomyślnie.", "Informacja");
                                }
                                catch(Exception ex)
                                {
                                    #if (DEBUG)
                                        MessageBox.Show(ex.ToString(), "DEBUG INFORMATION");
                                        MessageBox.Show(ex.GetType().ToString(), "Error - debug");
                                    #endif
                                    MessageBox.Show("Wystąpił nieznany błąd, skontaktuj się z administratorem.", "Error");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Musisz zaakceptować regulamin.", "Błąd");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Hasło nie spełnia wymogów. Hasło musi posiadać 8-20 znaków, przynajmniej jedną cyfrę, jedną małą oraz jedną dużą literę.", "Błąd");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Podane hasła nie zgadzają się.", "Błąd");
                    }
                }
                else
                {
                    MessageBox.Show("Konto o takim loginie już istnieje.", "Błąd");
                }
            }
            else
            {
                MessageBox.Show("Żadne z pól nie może pozostać puste.", "Błąd");
            }
        }
    }
}
