using System.Windows;
using Autofac;

namespace Whitebox.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly IContainer _container;

        static App()
        {
            var builder = new ContainerBuilder();
            _container = builder.Build();
        }

        internal static TService Resolve<TService>()
        {
            return _container.Resolve<TService>();
        }
    }
}
