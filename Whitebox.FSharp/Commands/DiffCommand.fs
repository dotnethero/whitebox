module Whitebox.DiffCommand

open System
open Whitebox
open Whitebox.Types

type LineType = Other = 0 | Add = 1 | Remove = 2
type Line = {
    Text: string;
    Type: LineType;
}

//TODO: Move to common parts

let parseChunk = function
    | Output x -> Some x
    | _ -> None

let splitLines (x:string) = x.Split ([|"\r"; "\n"|], StringSplitOptions.RemoveEmptyEntries)

let getLines = 
    List.choose parseChunk 
    >> List.fold (+) "" 
    >> splitLines 
    >> List.ofArray

let getLinesFromResult result = 
    match result with
    | Data x -> x |> getLines
    | _ -> failwith "Incorrect result"

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

let execute dir path =
    use cmd = new CommandServer(dir)
    cmd.Command("diff", path)
        |> getLinesFromResult
        |> List.map parseLine

let byHash dir path hash =
    use cmd = new CommandServer(dir)
    cmd.Command("diff", path, "-c", hash)
        |> getLinesFromResult
        |> List.map parseLine
                