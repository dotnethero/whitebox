namespace Whitebox.Views

open Whitebox.ViewModels

type MainWindowBase = FsXaml.XAML<"Views/MainWindow.xaml">
type MainWindow(dc:AppModel) =
    inherit MainWindowBase(DataContext = dc)

    override this.Exit(_,_) = this.Close()
    override this.Loaded(_,_) = dc.Load()
