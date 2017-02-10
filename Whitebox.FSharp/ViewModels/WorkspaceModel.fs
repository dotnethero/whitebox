namespace Whitebox.ViewModels
open System
open Whitebox
open Whitebox.Types

type WorkspaceModel() as self =
    inherit ViewModel()

    let mutable dir : string = ""
    let mutable files : FileStatus list = []
    let mutable diff : Line list = []
    let mutable currentFile : FileStatus option = None

    do
        base.WhenPropertyChanged <@ self.CurrentFile @>
        |> Observable.subscribe self.FileChanged
        |> ignore

    member x.FileChanged p =
        match currentFile with
        | Some file -> self.Diff <- DiffCommand.execute dir file.FilePath
        | None -> ()

    member x.Files
        with get() = files
        and set(files') =
            files <- files'
            x.OnPropertyChanged <@ x.Files @>

    member x.Diff
        with get() = diff
        and set(diff') =
            diff <- diff'
            x.OnPropertyChanged <@ x.Diff @>
            
    member x.CurrentFile
        with get() = 
            match currentFile with
            | Some file -> file
            | None -> null :> obj :?> FileStatus

        and set(currentFile') =
            currentFile <- 
                match box currentFile' with
                | null -> None
                | _ -> Some currentFile'
            x.OnPropertyChanged <@ x.CurrentFile @>

    member x.ShowChanges path =
        dir <- path
        x.Files <- StatusCommand.execute path

