module Whitebox.Chunks

open System
open Whitebox.Types

let parseChunk = function
    | Output x -> Some x
    | _ -> None

let splitLines (x:string) = x.Split ([|"\r"; "\n"|], StringSplitOptions.RemoveEmptyEntries)

let getChunks = 
    List.choose parseChunk

let foldChunks =
    List.fold (+) ""

let getLines = 
    getChunks
    >> foldChunks
    >> splitLines 
    >> List.ofArray

let processData processor result =
    match result with
    | Data x -> x |> processor
    | _ -> failwith "Incorrect result"    

let getLinesFromResult = processData getLines
let getChunksFromResult = processData getChunks
