﻿module Whitebox.Types

open System

type Channel = char
type BufferSize = uint32

type Chunk =
    | Output of string
    | Error of string
    | Input of BufferSize
    | LineInput of BufferSize
    | Exit

type LineType = Other = 0 | Add = 1 | Remove = 2
type Line = {
    Text: string;
    Type: LineType; }

type Changeset = { 
    Revnumber: int;
    Date: DateTime;
    Hash: string;
    Author: string;
    Summary: string;
    Branch: string;
    Phase: string; }

type FileStatus() =  
    member val Modifier: string = "" with get, set
    member val FilePath: string = "" with get, set
    member val Selected: bool = false with get, set
    
[<NoComparison; NoEquality>]
type CommandResult =
    | Data of Chunk list
    | Callback of Chunk list * push: (string -> CommandResult) * close: (unit -> unit)

type AskPassword = {
    Url: string;
    Realm: string;
    User: string }

[<NoComparison; NoEquality>]
type MaybeAsk<'Success, 'Fail> =
    | Success of 'Success
    | Fail of 'Fail
    | Ask of AskPassword * push: (string -> MaybeAsk<'Success, 'Fail>) * close: (unit -> unit)

type Result<'Success, 'Fail> =
    | Success of 'Success
    | Fail of 'Fail
