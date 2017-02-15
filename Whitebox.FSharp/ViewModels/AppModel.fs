namespace Whitebox.ViewModels

open Whitebox
open Whitebox.Types
open Whitebox.ViewModels
open System.ComponentModel

type IDialogService =
    abstract member OpenFolder: unit -> string option
    abstract member AskPassword: AskPasswordData -> string option

type MainWindowMode = ``Working copy`` = 0 | History = 1 | Shelves = 2

type AppModel(dialogs: IDialogService) as self =
    inherit ViewModel()
    
    let mutable mode = MainWindowMode.History
    let mutable status = ""
    let mutable tabIndex = 0
    let mutable dir : string option = Some "C:\Projects\hydrargyrum"

    do
        base.WhenPropertyChanged <@ self.Mode @>
        |> Observable.subscribe self.ModeSwitched
        |> ignore

    new() =
        let mock = { new IDialogService with
            member x.AskPassword _ = Some ""
            member x.OpenFolder() = Some ""
        } // for xaml
        AppModel(mock)

    member x.OpenRepository =
        new TrueCommand (x.OpenDialog)

    member x.Pull =
        new TrueCommand (x.PullDialog)

    member x.ParsePull = function
        | Success chunks -> x.StatusBar <- "Success..."
        | Fail chunks -> x.StatusBar <- "Fail..."
        | AskPassword (ask, push, close) -> 
            
            x.StatusBar <- "Dialog..."
            match dialogs.AskPassword(ask) with
            | None -> ()
            | Some password -> 
                x.StatusBar <- password
                password |> push |> x.ParsePull

            x.StatusBar <- "OK"
            close()

    member x.PullDialog p =
        match dir with
        | None -> ()
        | Some path -> PullCommand.execute path |> x.ParsePull
            
    member x.OpenDialog p =
        match dialogs.OpenFolder() with
        | Some path -> 
            dir <- Some path
            x.OnPropertyChanged <@ x.Mode @>
        | None -> ()

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
