namespace Whitebox.ViewModels

open Whitebox
open Whitebox.Types

type HistoryModel() as self =
    inherit ViewModel()
    
    let mutable dir : string = ""
    let mutable changesets : Changeset list = []
    let mutable diffFile : Line list = []
    let mutable files : FileStatus list = []
    let mutable currentFile : FileStatus option = None
    let mutable currentChangeset : Changeset option = None
    
    do
        base.WhenPropertyChanged <@ self.CurrentChangeset @>
        |> Observable.subscribe self.ChangesetChanged
        |> ignore

        base.WhenPropertyChanged <@ self.CurrentFile @>
        |> Observable.subscribe self.FileChanged
        |> ignore
    
    member x.FileChanged p =
        match currentChangeset, currentFile with
        | Some changeset, Some file -> self.DiffFile <- DiffCommand.byHash dir file.FilePath changeset.Hash
        | _, _ -> ()

    member x.ChangesetChanged p =
        match currentChangeset with
        | Some changeset -> 
            self.Files <- StatusCommand.byHash dir changeset.Hash
            self.DiffFile <- []
        | _ -> ()

    member x.Changesets
        with get() = changesets
        and set(changesets') =
            changesets <- changesets'
            x.OnPropertyChanged <@ x.Changesets @>
    
    member x.DiffFile
        with get() = diffFile
        and set(diffFile') =
            diffFile <- diffFile'
            x.OnPropertyChanged <@ x.DiffFile @>
            
    member x.Files
        with get() = files
        and set(files') =
            files <- files'
            x.OnPropertyChanged <@ x.Files @>

    member x.CurrentFile
        with get() = _null currentFile
        and set(currentFile') =
            currentFile <- _option currentFile'
            x.OnPropertyChanged <@ x.CurrentFile @>
            
    member x.CurrentChangeset
        with get() = _null currentChangeset
        and set(currentChangeset') =
            currentChangeset <- _option currentChangeset'
            x.OnPropertyChanged <@ x.CurrentChangeset @>
    
    member x.ShowLog path =
        dir <- path
        x.Changesets <- LogCommand.execute dir 150