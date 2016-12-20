module Whitebox.DiffCommand

open Whitebox
open Whitebox.Types

let getType (line:string) =
    if line.Length = 0 
    then 
        LineType.Other
    else 
        match line.[0] with
        | '+' -> LineType.Add
        | '-' -> LineType.Remove
        | _ -> LineType.Other

let parseLine (x:string) = 
    { Text = x;
    Type = getType x; }

let processResult = Chunks.getLinesFromResult >> List.map parseLine

let execute dir path =
    use cmd = new CommandServer(dir)
    cmd.Command("diff", path) |> processResult

let byHash dir path hash =
    use cmd = new CommandServer(dir)
    cmd.Command("diff", path, "-c", hash) |> processResult
