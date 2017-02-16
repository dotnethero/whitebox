module Whitebox.Chunks

open System
open Whitebox.Types

let parseChunk = function
    | Output x -> Some x
    | _ -> None

let parseError = function
    | Error x -> Some x
    | _ -> None

let splitLines (x:string) = x.Split ([|"\r"; "\n"|], StringSplitOptions.RemoveEmptyEntries)

let getChunks = List.choose parseChunk
let getErrors = List.choose parseError
let foldLines = List.fold (+) ""

let getLines = 
    getChunks
    >> foldLines
    >> splitLines 
    >> List.ofArray

let processData processor result =
    match result with
    | Data x -> x |> processor
    | _ -> failwith "Incorrect result"    

let getLinesFromResult = processData getLines
let getChunksFromResult = processData getChunks
