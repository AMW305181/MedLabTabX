using System;
using System.Windows;
using System.Windows.Media;
using MedLabTab.DatabaseManager;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for ResettingPassword.xaml
    /// </summary>
    public partial class ResettingPassword : Window
    {
        public ResettingPassword()
        {
            InitializeComponent();
        }

        private void SendResetLink_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameEmailTextBox.Text.Trim();

            // Check if the username field is empty
            if (string.IsNullOrEmpty(username))
            {
                InfoLabel.Content = "Proszę wprowadzić login.";
                InfoLabel.Foreground = new SolidColorBrush(Colors.Red);
                InfoLabel.Visibility = Visibility.Visible;
                return;
            }

            // Use your existing IsLoginTaken method to check if the user exists
            bool userExists = DbManager.IsLoginTaken(username);

            if (userExists)
            {
                // User exists - show success message
                InfoLabel.Content = "Link resetujący hasło został wysłany na Twój adres email.";
                InfoLabel.Foreground = new SolidColorBrush(Colors.Green);
                InfoLabel.Visibility = Visibility.Visible;

                // This is just a placeholder for the actual email sending functionality
            }
            else
            {
                // User doesn't exist - show error message
                InfoLabel.Content = "Podany użytkownik nie istnieje w systemie.";
                InfoLabel.Foreground = new SolidColorBrush(Colors.Red);
                InfoLabel.Visibility = Visibility.Visible;
            }
        }
    }
}