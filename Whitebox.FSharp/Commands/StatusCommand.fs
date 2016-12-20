module Whitebox.StatusCommand

open System
open Whitebox
open Whitebox.Types

type FileStatus = { 
    Modifier: string;
    FilePath: string;
}

let execute dir =

    let createCmd dir = new CommandServer(dir)

    let parseLine (x:string) =
        let fields = x.Split([|' '|], 2) 
        {   
            Modifier = fields.[0];
            FilePath = fields.[1];
        }

    let parseChunk = function
        | Output x -> Some x
        | _ -> None

    let splitLines (x:string) = x.Split ([|"\r"; "\n"|], StringSplitOptions.RemoveEmptyEntries)

    use cmd = createCmd(dir)
    match cmd.Command("status") with
        | Data x -> 
            x 
            |> List.choose parseChunk
            |> List.fold (+) ""
            |> splitLines
            |> List.ofArray
            |> List.map parseLine

        | _ -> failwith "Incorrect result"
