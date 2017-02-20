﻿namespace Whitebox.ViewModels

open Whitebox
open Whitebox.Types
open Whitebox.ViewModels
open Whitebox.Services

type MainWindowMode = ``Working copy`` = 0 | History = 1 | Shelves = 2

type AppModel(dialogs: IDialogService) as self =
    inherit ViewModel()
    
    let mutable mode = MainWindowMode.``Working copy``
    let mutable status = ""
    let mutable tabIndex = 0
    let mutable branches = []
    let mutable dir : string option = Some "D:\hydrargyrum.hg"

    let modeSwitched _ =
        match self.Mode, dir with
        | MainWindowMode.``Working copy``, Some path -> self.Workspace.ShowChanges path
        | MainWindowMode.History, Some path -> self.History.ShowLog path
        | _ -> ()
        self.TabIndex <- int self.Mode
        self.StatusBar <- sprintf "%A" self.Mode
        self.Branches <- Commands.branches dir.Value

    let changeStatus status = self.StatusBar <- status
    let changeDir path =
        dir <- Some path
        self.OnPropertyChanged <@ self.Mode @>

    let rec parseMaybeAsk(success, failure) = function
        | MaybeAsk.Success _ -> success |> changeStatus
        | MaybeAsk.Fail _ -> failure |> changeStatus
        | MaybeAsk.Ask (data, push, close) -> 
            match dialogs.AskPassword(data) with
            | None -> ()
            | Some password -> 
                self.StatusBar <- password
                password |> push |> parseMaybeAsk (success, failure)
            success |> changeStatus
            close()

    let init(path) = 
        match Commands.init path with
        | Success _ ->
            changeDir path
            sprintf "Repository initialized at %s" path |> changeStatus
        | Fail message ->
            message |> changeStatus
    
    // commands

    let openCommand() = 
        match dialogs.OpenFolder() with
        | Some path -> changeDir path
        | None -> ()

    let initCommand() = 
        match dialogs.OpenFolder() with
        | Some path -> init path
        | None -> ()

    let pushCommand() = 
        match dir with
        | Some path -> Commands.push path |> parseMaybeAsk ("Push succeeded", "Push failed")
        | None -> ()

    let pullCommand() =
        match dir with
        | Some path -> Commands.pull path |> parseMaybeAsk ("Pull succeeded", "Pull failed")
        | None -> ()
      
    do
        base.WhenPropertyChanged <@ self.Mode @>
        |> Observable.subscribe modeSwitched
        |> ignore

    member x.Load _ =
        modeSwitched()

    // commands
    member x.OpenRepository = new TrueCommand (openCommand)
    member x.InitRepository = new TrueCommand (initCommand)
    member x.Pull = new TrueCommand (pullCommand)
    member x.Push = new TrueCommand (pushCommand)

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
            
    member x.Branches 
        with get() = branches
        and set(branches') =
            branches <- branches'
            x.OnPropertyChanged <@ x.Branches @>

    member val History: HistoryModel = HistoryModel() with get
    member val Workspace: WorkspaceModel = WorkspaceModel() with get
    member val Modes = [MainWindowMode.``Working copy``; MainWindowMode.History; MainWindowMode.Shelves] with get
