module Whitebox.InitCommand

open System
open Whitebox
open Whitebox.Types
open FSharp.Text.RegexProvider

type ExistsRegex = Regex<"already exists!">
let alreadyExists chunks = 
    let exists = 
        chunks
        |> Chunks.getErrors 
        |> Chunks.foldLines
        |> ExistsRegex.TypedIsMatch
    if exists then Fail "Repository already exists" else Success()

let execute dir =
    let cmd = new CommandServer(dir)
    match cmd.Command("init") with
    | Data chunks -> alreadyExists chunks
    | _ -> Fail "Init command unexpectedly requested user input"