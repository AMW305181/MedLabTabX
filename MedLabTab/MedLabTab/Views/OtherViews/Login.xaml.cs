using MedLabTab.ViewModels;
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
using MedLabTab.DatabaseManager;
using MedLabTab.Views.MainViews;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private LoginVM LoginVM;

        public Login()
        {
            InitializeComponent();
            LoginVM = new LoginVM();
        }

        private void ZapomnialesHasla_Click(object sender, RoutedEventArgs e)
        {
            ResettingPassword resetWindow = new ResettingPassword();
            resetWindow.Show();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            InactiveLabel.Visibility = Visibility.Collapsed;
            ErrorLabel.Visibility = Visibility.Collapsed;
            string password = PasswordBox.Password;
            string passwordHashed = null;
            bool passwordValid = false;

            bool isCredNoneEmpty = LoginVM.IsCredentialsNoneEmpty(username: LoginTextBox.Text, password: password);
            bool userExists = DbManager.IsLoginTaken(login: LoginTextBox.Text);
            if (userExists) { 
                passwordHashed=DbManager.GetHashedPassword(username: LoginTextBox.Text); 
                passwordValid = PasswordHasher.Verify(password, passwordHashed);
            }
            
            bool isUserValid = DbManager.CheckUser(username: LoginTextBox.Text, password: password);
            if (isCredNoneEmpty && passwordValid)
            {
                SignedInUser User = new SignedInUser();

                DbManager.LogInUser(username: LoginTextBox.Text, user: ref User);
                if (!User.IsActive)
                {
                    InactiveLabel.Visibility = Visibility.Visible;

                }
                else
                {
                    switch (User.UserType)
                    {
                        case 1:
                            MainViewReception mainViewReception = new MainViewReception();
                            mainViewReception.Show();
                            break;
                        case 2:
                            MainViewNurse mainViewNurse = new MainViewNurse(User);
                            mainViewNurse.Show();
                            break;
                        case 3:
                            MainViewAnalyst mainViewAnalyst = new MainViewAnalyst(User);
                            mainViewAnalyst.Show();
                            break;
                        case 4:
                            MainViewPatient mainViewPatient = new MainViewPatient(User);
                            mainViewPatient.Show();
                            break;
                    }
                    this.Close();
                }
            }
            else
            {
                ErrorLabel.Visibility = Visibility.Visible;
            }


        }
    }
}
