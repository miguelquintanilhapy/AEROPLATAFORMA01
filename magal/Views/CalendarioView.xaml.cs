using System.Windows;
using System.Windows.Controls;
using magal.ViewModels;

namespace magal.Views
{
    public partial class CalendarioView : UserControl
    {
        public CalendarioView()
        {
            InitializeComponent();
            this.DataContext = new CalendarioViewModel();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalendarioViewModel vm)
                vm.Carregar();
        }
    }
}