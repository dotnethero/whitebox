namespace Whitebox.Views

type MainWindowBase = FsXaml.XAML<"Views/MainWindow.xaml">
type MainWindow() =
    inherit MainWindowBase()
    override this.Exit(_,_) = this.Close()
