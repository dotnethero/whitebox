module Whitebox.InitCommand

open Whitebox.Types

let execute dir =
    let cmd = new CommandServer(dir)
    match cmd.Command("init") with
    | Data chunks ->
        match Parsers.alreadyExists chunks with
        | true  -> Result.Fail "Repository already exists"
        | false -> Result.Success chunks
    | _ -> Result.Fail "Init command unexpectedly requested user input"