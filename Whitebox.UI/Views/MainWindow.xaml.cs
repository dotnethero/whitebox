using System.Windows;
using Whitebox.ViewModels;

namespace Whitebox.UI.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AppModel();
            //DataContext = App.Resolve<AppModel>();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
