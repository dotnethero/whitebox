module Whitebox.LogCommand

open System
open Whitebox
open Whitebox.Types

let sym = "\u0001"

let parseChangeset (x:string) =
    let fields = x.Split([|sym|], StringSplitOptions.RemoveEmptyEntries) 
    { Revnumber = Int32.Parse(fields.[0]);
    Hash = fields.[1];
    Author = fields.[2];
    Date = DateTime.Parse(fields.[3]);
    Summary = fields.[4];
    Branch = fields.[5]; }
        
let execute dir limit =
    use cmd = new CommandServer(dir)
    let template = "{rev}" + sym + "{node}" + sym + "{author}" + sym + "{date|isodate}" + sym + "{desc}" + sym + "{branch}"
    cmd.Command("log", "-l", string limit, "-T", template) 
    |> Chunks.getChunksFromResult
    |> List.map parseChangeset
