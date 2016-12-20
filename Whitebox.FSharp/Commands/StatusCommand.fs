module Whitebox.StatusCommand

open Whitebox
open Whitebox.Types

let parseFileStatus (x:string) =
    let fields = x.Split([|' '|], 2) 
    { Modifier = fields.[0]; FilePath = fields.[1]; }

let execute dir =
    use cmd = new CommandServer(dir)
    cmd.Command("status")
    |> Chunks.getLinesFromResult
    |> List.map parseFileStatus
