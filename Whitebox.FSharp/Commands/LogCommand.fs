module Whitebox.LogCommand

open System
open Whitebox
open Whitebox.Types

type Changeset = { 
    Revnumber: int;
    Date: DateTime;
    Hash: string;
    Author: string;
    Summary: string;
    Branch: string; 
}

let sym = "\u0001"

let execute dir limit =

    let createCmd dir = new CommandServer(dir)
    let template = "{rev}" + sym + "{node}" + sym + "{author}" + sym + "{date|isodate}" + sym + "{desc}" + sym + "{branch}"

    let parseLine (x:string) =
        let fields = x.Split([|sym|], StringSplitOptions.RemoveEmptyEntries) 
        {   
            Revnumber = Int32.Parse(fields.[0]);
            Hash = fields.[1];
            Author = fields.[2];
            Date = DateTime.Parse(fields.[3]);
            Summary = fields.[4];
            Branch = fields.[5]; 
        }

    let parseChunk = function
        | Output x -> parseLine x |> Some
        | _ -> None

    use cmd = createCmd(dir)
    match cmd.Command("log", "-l", string limit, "-T", template) with
        | Data x -> x |> List.choose parseChunk
        | _ -> failwith "Incorrect result"
