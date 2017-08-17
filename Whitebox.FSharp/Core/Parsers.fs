module Parsers

open System
open Whitebox
open Whitebox.Types
open FSharp.Text.RegexProvider

type ExistsRegex = Regex<"already exists!">
type RealmRegex = Regex<"realm: (?<realm>.+)">
type UserRegex = Regex<"user: (?<user>.+)">
type UrlRegex = Regex<"url: (?<url>.+)">
type AskUserRegex = Regex<"user:">
type AskPasswordRegex = Regex<"password:">

let matches (rx: unit -> #System.Text.RegularExpressions.Regex) chunks =
    chunks
    |> Chunks.getTextChunks
    |> List.map (fun ch -> rx().Match ch)

let has regex chunks =
    chunks
    |> matches regex
    |> List.tryFind (fun m -> m.Success && m.Groups.Count > 1)
    |> Option.map (fun m -> m.Groups.[1].Value)

let asks rx chunks =
    chunks
    |> matches rx
    |> List.exists (fun m -> m.Success)

let (|AskSome|_|) c = 
    let url = has UrlRegex c
    match (has RealmRegex c, has UserRegex c, asks AskUserRegex c, asks AskPasswordRegex c) with
    | (Some realm, Some user, _, true) -> AskPassword { Url = url; Realm = realm; User = user } |> Some
    | (Some realm, _, true, _) -> AskUser { Url = url; Realm = realm } |> Some
    | _ -> None

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
        | AskSome data -> Ask (data, push >> maybeAsk, close)
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

let changesetTemplate = "{rev}" + sym + "{node}" + sym + "{author}" + sym + "{date|isodate}" + sym + "{desc}" + sym + "{branch}" + sym + "{phase}" + sym + "{p1node}" + sym + "{p2node}"

let parseChangeset (x:string) =
    let fields = x.Split([|sym|], StringSplitOptions.RemoveEmptyEntries) 
    { 
        Revnumber = Int32.Parse(fields.[0]);
        Hash = fields.[1];
        Author = fields.[2];
        Date = DateTime.Parse(fields.[3]);
        Summary = fields.[4];
        Branch = fields.[5];
        Phase = fields.[6]; 
        Parent1 = fields.[7]; 
        Parent2 = fields.[8]; 
    }

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
