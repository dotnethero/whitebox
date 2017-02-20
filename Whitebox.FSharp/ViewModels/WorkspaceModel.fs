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
    let mutable comment : string = ""

    do
        base.WhenPropertyChanged <@ self.CurrentFile @>
        |> Observable.subscribe self.FileChanged
        |> ignore

    member x.FileChanged p =
        match currentFile with
        | Some file -> self.Diff <- Commands.currentChanges dir file.FilePath
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
        with get() = _null currentFile
        and set(currentFile') =
            currentFile <- _option currentFile'
            x.OnPropertyChanged <@ x.CurrentFile @>
   
    member x.Comment
        with get() = comment
        and set(comment') =
            comment <- comment'
            x.OnPropertyChanged <@ x.Comment @>

    member x.ShowChanges path =
        dir <- path
        x.Files <- Commands.currentStatus path

    member x.Commit _ =
        x.Files 
        |> List.filter (fun f -> f.Selected) 
        |> List.map (fun f -> f.FilePath)
        |> List.toSeq
        |> Commands.commit dir x.Comment 
        |> ignore
        
    member x.OpenContainingFolder _ =
        Open.directory dir

    member x.CommitCommand = new TrueCommand (x.Commit)

    member x.OpenContainingFolderCommand =  new TrueCommand (x.OpenContainingFolder)
