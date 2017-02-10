namespace Whitebox.ViewModels

open Whitebox
open Whitebox.Types
open Whitebox.ViewModels
open System.ComponentModel

type MainWindowMode = ``Working copy`` = 0 | History = 1 | Shelves = 2

type AppModel() as self =
    inherit ViewModel()
    
    let mutable mode = MainWindowMode.History
    let mutable status = ""
    let mutable tabIndex = 0
    let mutable path = "C:\Projects\hydrargyrum"

    do
        base.WhenPropertyChanged <@ self.Mode @>
        |> Observable.subscribe self.ModeSwitched
        |> ignore
    
    member x.ModeSwitched p =
        match x.Mode with
        | MainWindowMode.``Working copy`` -> x.Workspace.ShowChanges path
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

    member val History: HistoryModel = HistoryModel() with get, set
    member val Workspace: WorkspaceModel = WorkspaceModel() with get, set
    member val Modes = [MainWindowMode.``Working copy``; MainWindowMode.History; MainWindowMode.Shelves] with get
