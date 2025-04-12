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
using MedLabTab.ViewModels;
using MedLabTab.Views.OtherViews;

namespace MedLabTab.Views.MainViews
{
    /// <summary>
    /// Interaction logic for MainViewPatient.xaml
    /// </summary>
    public partial class MainViewPatient : Window
    {
        private SignedInUser currentUser;
        public MainViewPatient(SignedInUser user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests();
            allTests.Show();
        }

        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisits allVisits = new AllVisits();
            allVisits.Show();
        }

        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile(currentUser);
            profile.Show();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Czy na pewno chcesz się wylogować?", "Wylogowanie",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
