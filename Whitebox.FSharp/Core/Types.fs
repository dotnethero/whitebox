module Whitebox.Types

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
    | Question of Chunk list
