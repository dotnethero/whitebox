module Program

open System
open System.Windows
open Whitebox.ViewModels
open Whitebox.Views
open Whitebox.Services

[<STAThread>]
[<EntryPoint>]
let main args =
    let ds = new DialogService()
    let vm = AppModel(ds)
    let window = new MainWindow(DataContext = vm)
    let app = new Application()
    app.Run(window)