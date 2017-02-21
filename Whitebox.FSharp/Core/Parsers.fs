module Parsers

open System
open Whitebox
open Whitebox.Types
open FSharp.Text.RegexProvider

type AskUserRegex = Regex<"""realm: (?<realm>.+)\nurl: (?<url>.+)\nuser:""">
type AskPasswordRegex = Regex<"""realm: (?<realm>.+)\nurl: (?<url>.+)\nuser: (?<user>.+) \(fixed in hgrc or url\)\npassword:""">
type ExistsRegex = Regex<"already exists!">

let (|AskPassword|_|) chunks = 
    let regex = AskPasswordRegex()
    let m = chunks |> Chunks.getText |> regex.TypedMatch
    if m.Success 
        then 
        Some <| AskPassword { 
            Url =   m.url.Value;
            Realm = m.realm.Value;
            User =  m.user.Value }
        else None

let (|AskUser|_|) chunks = 
    let regex = AskUserRegex()
    let m = chunks |> Chunks.getText |> regex.TypedMatch
    if m.Success 
        then 
        Some <| AskUser { 
            Url =   m.url.Value;
            Realm = m.realm.Value }
        else None

let alreadyExists chunks = 
    chunks
    |> Chunks.getError
    |> ExistsRegex.TypedIsMatch

// commnds

let rec maybeAsk result =
    match result with
    | Data chunks -> 
        if chunks |> Chunks.hasError
            then MaybeAsk.Fail (chunks |> Chunks.getText)
            else MaybeAsk.Success (chunks |> Chunks.getText)

    | Callback (chunks, push, close) -> 
        match chunks with
        | AskPassword data -> Ask (AskPassword data, push >> maybeAsk, close)
        | AskUser data -> Ask (AskUser data, push >> maybeAsk, close)
        | _ -> MaybeAsk.Fail (chunks |> Chunks.getText)

// statuses

let parseFileStatus (x:string) =
    let fields = x.Split([|' '|], 2) 
    FileStatus ( Modifier = fields.[0], FilePath = fields.[1], Selected = false )

let parseFileStatuses = Chunks.getLinesFromResult >> List.map parseFileStatus

// branches

let sym = "\u0001"
let branchTemplate = "{rev}" + sym + "{node}" + sym + "{branch}"

let parseBranches chunks =
    let parseOne (x:string) =
        let fields = x.Split([|sym|], StringSplitOptions.RemoveEmptyEntries) 
        { Revnumber = Int32.Parse fields.[0]; 
        Hash = fields.[1] ; 
        Name = fields.[2] } 
    chunks |> Chunks.getChunksFromResult |> List.map parseOne

// changesets

let changesetTemplate = "{rev}" + sym + "{node}" + sym + "{author}" + sym + "{date|isodate}" + sym + "{desc}" + sym + "{branch}" + sym + "{phase}"

let parseChangeset (x:string) =
    let fields = x.Split([|sym|], StringSplitOptions.RemoveEmptyEntries) 
    { Revnumber = Int32.Parse(fields.[0]);
    Hash = fields.[1];
    Author = fields.[2];
    Date = DateTime.Parse(fields.[3]);
    Summary = fields.[4];
    Branch = fields.[5];
    Phase = fields.[6]; }

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
