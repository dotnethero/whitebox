namespace Whitebox.ViewModels

open Whitebox
open Whitebox.Types
open Whitebox.ViewModels
open System.ComponentModel

type MainWindowMode = WorkingCopy = 0 | History = 1 | Shelves = 2

type AppModel() as self =
    inherit ViewModel()
    
    let mutable mode = MainWindowMode.History
    let mutable status = ""
    
    do
        base.PropertyChanged
        |> Observable.filter (fun x -> x.PropertyName <> "StatusBar")
        |> Observable.subscribe (fun x -> self.StatusBar <- sprintf "%A" x.PropertyName)
        |> ignore
    
    member x.Mode 
        with get() = mode
        and set(mode') =
            mode <- mode'
            x.OnPropertyChanged <@ x.Mode @>
    
    member x.StatusBar 
        with get() = status
        and set(status') =
            status <- status'
            x.OnPropertyChanged <@ x.StatusBar @>

    member val History: HistoryModel = HistoryModel() with get, set
    member val Workspace: WorkspaceModel = WorkspaceModel() with get, set
    member val TabIndex: int = 0 with get, set
    member val Modes = [MainWindowMode.WorkingCopy; MainWindowMode.History; MainWindowMode.Shelves] with get
