namespace Whitebox.ViewModels

open Whitebox
open Whitebox.Types
open Whitebox.ViewModels
open System.ComponentModel
open System.Windows.Forms

type MainWindowMode = ``Working copy`` = 0 | History = 1 | Shelves = 2

type AppModel() as self =
    inherit ViewModel()
    
    let mutable mode = MainWindowMode.History
    let mutable status = ""
    let mutable tabIndex = 0
    let mutable dir : string option = None

    do
        base.WhenPropertyChanged <@ self.Mode @>
        |> Observable.subscribe self.ModeSwitched
        |> ignore

    member x.OpenRepository =
        new TrueCommand (fun p -> x.OpenDialog())

    member x.OpenDialog() =
        let dialog = new FolderBrowserDialog()
        let result = dialog.ShowDialog()
        if result = DialogResult.OK then 
            dir <- Some dialog.SelectedPath
            x.OnPropertyChanged <@ x.Mode @>

    member x.ModeSwitched p =
        match x.Mode, dir with
        | MainWindowMode.``Working copy``, Some path -> x.Workspace.ShowChanges path
        | MainWindowMode.History, Some path -> x.History.ShowLog path
        | _ -> ()
        x.TabIndex <- int x.Mode
        x.StatusBar <- sprintf "%A" x.Mode

    member x.Mode 
        with get() = mode
        and set(mode') =
            mode <- mode'
            x.OnPropertyChanged <@ x.Mode @>
     
    member x.TabIndex 
        with get() = tabIndex
        and set(tabIndex') =
            tabIndex <- tabIndex'
            x.OnPropertyChanged <@ x.TabIndex @>
    
    member x.StatusBar 
        with get() = status
        and set(status') =
            status <- status'
            x.OnPropertyChanged <@ x.StatusBar @>

    member val History: HistoryModel = HistoryModel() with get
    member val Workspace: WorkspaceModel = WorkspaceModel() with get
    member val Modes = [MainWindowMode.``Working copy``; MainWindowMode.History; MainWindowMode.Shelves] with get
