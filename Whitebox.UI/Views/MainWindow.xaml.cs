using System.Windows;
using Whitebox.ViewModels;

namespace Whitebox.UI.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AppModel(new DialogService());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
