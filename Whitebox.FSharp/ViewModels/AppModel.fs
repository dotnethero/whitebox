namespace Whitebox.ViewModels

open Whitebox
open Whitebox.Types
open Whitebox.ViewModels
open Whitebox.Services

type MainWindowMode = ``Working copy`` = 0 | History = 1 | Shelves = 2

type AppModel(dialogs: IDialogService) as self =
    inherit ViewModel()
    
    let mutable mode = MainWindowMode.History
    let mutable status = ""
    let mutable tabIndex = 0
    let mutable dir : string option = Some "D:\hydrargyrum.hg"

    let changeStatus status = self.StatusBar <- status
    let changeDir path =
        dir <- Some path
        self.OnPropertyChanged <@ self.Mode @>

    let rec parsePull = function
        | PullResult.Success _ -> "Pull succeeded" |> changeStatus
        | PullResult.Fail _ -> "Pull failed" |> changeStatus
        | PullResult.Ask (data, push, close) -> 
            match dialogs.AskPassword(data) with
            | None -> ()
            | Some password -> 
                self.StatusBar <- password
                password |> push |> parsePull
            "Pull succeeded" |> changeStatus
            close()

    let initRepo path = 
        match InitCommand.execute path with
        | InitResult.Success ->
            changeDir path
            sprintf "Repository initialized at %s" path |> changeStatus
        | InitResult.Fail message ->
            message |> changeStatus

    let openPullDialog() =
        match dir with
        | None -> ()
        | Some path -> PullCommand.execute path |> parsePull

    let ifsome f = function
        | Some s -> f s
        | None -> ()

    let openFolderDialog = dialogs.OpenFolder >> (ifsome changeDir)
    let openInitDialog = dialogs.OpenFolder >> (ifsome initRepo)
            
    let modeSwitched _ =
        match self.Mode, dir with
        | MainWindowMode.``Working copy``, Some path -> self.Workspace.ShowChanges path
        | MainWindowMode.History, Some path -> self.History.ShowLog path
        | _ -> ()
        self.TabIndex <- int self.Mode
        self.StatusBar <- sprintf "%A" self.Mode

    do
        base.WhenPropertyChanged <@ self.Mode @>
        |> Observable.subscribe modeSwitched
        |> ignore

    // for xaml
    new() =
        let mock = { new IDialogService with
            member x.AskPassword _ = Some ""
            member x.OpenFolder() = Some ""
        }
        AppModel(mock)

    // commands
    member x.OpenRepository = new TrueCommand (openFolderDialog)
    member x.InitRepository = new TrueCommand (openInitDialog)
    member x.Pull = new TrueCommand (openPullDialog)

    // properties
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
