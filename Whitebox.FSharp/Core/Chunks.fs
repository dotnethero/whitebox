module Whitebox.Chunks

open System
open Whitebox.Types

let parseOutput = function
    | Output x -> Some x
    | _ -> None
let getOutputChunks = List.choose parseOutput

let parseError = function
    | Error x -> Some x
    | _ -> None
let getErrorChunks = List.choose parseError
let hasError = List.tryFind (parseError >> Option.isSome) >> Option.isSome

let parseText = function
    | Output x -> Some x
    | Error x -> Some x
    | _ -> None
let getTextChunks = List.choose parseText

let splitLines (x:string) = x.Split ([|"\r"; "\n"|], StringSplitOptions.RemoveEmptyEntries)
let foldStrings = List.fold (+) ""

let getOutput = getOutputChunks >> foldStrings
let getError = getErrorChunks >> foldStrings
let getText = getTextChunks >> foldStrings

let getOutputLines = getOutput >> splitLines >> List.ofArray

let processData processor result =
    match result with
    | Data x -> x |> processor
    | _ -> failwith "Incorrect result"    

let getLinesFromResult = processData getOutputLines
let getChunksFromResult = processData getOutputChunks
