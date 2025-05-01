using MedLabTab.DatabaseModels;
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

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for EditVisitAdmin.xaml
    /// </summary>
    public partial class EditVisitAdmin : Window
    {
        private Visit _originalVisit;
        private AllVisitsAdmin _parentWindow;
        public EditVisitAdmin(Visit visitToEdit, AllVisitsAdmin parentWindow)
        {
            InitializeComponent();
            _originalVisit = visitToEdit;
            _parentWindow = parentWindow;
        }
    }
}
