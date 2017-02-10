module Whitebox.StatusCommand

open Whitebox
open Whitebox.Types

let parseFileStatus (x:string) =
    let fields = x.Split([|' '|], 2) 
    { Modifier = fields.[0]; FilePath = fields.[1]; }

let processResult = Chunks.getLinesFromResult >> List.map parseFileStatus

let execute dir =
    use cmd = new CommandServer(dir)
    cmd.Command("status") |> processResult

let byHash dir hash =
    use cmd = new CommandServer(dir)
    cmd.Command("status", "--change", hash) |> processResult
