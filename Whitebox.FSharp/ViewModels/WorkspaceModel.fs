namespace Whitebox.ViewModels
open Whitebox
open Whitebox.Types

type WorkspaceModel() =
    
    let dir = """C:\Projects\Tamga.hg"""
    member val Files : FileStatus list option = None with get, set
    member val CurrentFile : FileStatus option = None with get, set
    member val Diff : Line list option = None with get, set
    member val ShowChanges = ReactiveCommand(fun _ -> StatusCommand.execute dir) with get
