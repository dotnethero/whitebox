namespace Whitebox.Types

open System

type Channel = char

type BufferSize = uint32

type Chunk =
    | Output of string
    | Error of string
    | Input of BufferSize
    | LineInput of BufferSize
    | Exit

type CommandResult =
    | Data of Chunk list
    | Callback of Chunk list * push: (string -> CommandResult) * close: (unit -> unit)

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
    Branch: string; }

type FileStatus = { 
    Modifier: string;
    FilePath: string; }