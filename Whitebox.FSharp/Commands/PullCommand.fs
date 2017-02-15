module Whitebox.PullCommand

open System
open Whitebox
open Whitebox.Types
open FSharp.Text.RegexProvider

[<Literal>]
let Template = """realm: (?<realm>.+)\nurl: (?<url>.+)\nuser: (?<user>.+) \(fixed in hgrc or url\)"""
type PullRegex = Regex<Template>

let regex = PullRegex()
let parse chunks = 
    let rgx =
        chunks
        |> Chunks.getChunks 
        |> Chunks.foldChunks
        |> regex.TypedMatch
    match rgx.Success with
    | false -> None
    | true ->
        Some { Url =   rgx.url.Value;
               Realm = rgx.realm.Value;
               User =  rgx.user.Value }

let rec convert result =
    match result with
    | Data chunks -> Success chunks
    | Callback (chunks, push, close) -> 
        match chunks |> parse with
        | Some data -> AskPassword (data, push >> convert, close)
        | None -> Fail chunks

let execute dir =
    let cmd = new CommandServer(dir)
    cmd.Command("pull") |> convert