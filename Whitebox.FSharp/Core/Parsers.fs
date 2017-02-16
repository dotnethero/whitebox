module Parsers

open System
open Whitebox
open Whitebox.Types
open FSharp.Text.RegexProvider

type AskPasswordRegex = Regex<"""realm: (?<realm>.+)\nurl: (?<url>.+)\nuser: (?<user>.+) \(fixed in hgrc or url\)""">
type ExistsRegex = Regex<"already exists!">

let askPassword chunks = 
    let regex = AskPasswordRegex()
    let m =
        chunks
        |> Chunks.getChunks 
        |> Chunks.foldLines
        |> regex.TypedMatch
    if m.Success 
        then 
        Some { Url =   m.url.Value;
               Realm = m.realm.Value;
               User =  m.user.Value }
        else None

let alreadyExists chunks = 
    chunks
    |> Chunks.getErrors 
    |> Chunks.foldLines
    |> ExistsRegex.TypedIsMatch

// commnds

let rec maybeAsk result =
    match result with
    | Data chunks -> MaybeAsk.Success chunks
    | Callback (chunks, push, close) -> 
        match chunks |> askPassword with
        | Some data -> MaybeAsk.Ask (data, push >> maybeAsk, close)
        | None -> MaybeAsk.Fail chunks

// statuses

let parseFileStatus (x:string) =
    let fields = x.Split([|' '|], 2) 
    { Modifier = fields.[0]; FilePath = fields.[1]; }

let parseFileStatuses = Chunks.getLinesFromResult >> List.map parseFileStatus

// changesets

let sym = "\u0001"
let changesetTemplate = "{rev}" + sym + "{node}" + sym + "{author}" + sym + "{date|isodate}" + sym + "{desc}" + sym + "{branch}"

let parseChangeset (x:string) =
    let fields = x.Split([|sym|], StringSplitOptions.RemoveEmptyEntries) 
    { Revnumber = Int32.Parse(fields.[0]);
    Hash = fields.[1];
    Author = fields.[2];
    Date = DateTime.Parse(fields.[3]);
    Summary = fields.[4];
    Branch = fields.[5]; }

let parseChangesets = Chunks.getChunksFromResult >> List.map parseChangeset

let parseLineType (line:string) =
    if line.Length = 0 
    then LineType.Other
    else 
    match line.[0] with
        | '+' -> LineType.Add
        | '-' -> LineType.Remove
        | _ -> LineType.Other

let parseLine (x:string) = 
    { Text = x;
    Type = parseLineType x; }

let parseLines = Chunks.getLinesFromResult >> List.map parseLine
