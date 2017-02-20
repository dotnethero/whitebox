module Whitebox.Commands

open Whitebox.Types

let start dir = new CommandServer(dir)

let currentChanges dir path =
    use cmd = start dir
    cmd.Command("diff", path) |> Parsers.parseLines

let commitChanges dir path hash =
    use cmd = start dir
    cmd.Command("diff", path, "-c", hash) |> Parsers.parseLines
    
let log dir limit =
    use cmd = start dir
    cmd.Command("log", "-l", string limit, "-T", Parsers.changesetTemplate) |> Parsers.parseChangesets
    
let currentStatus dir =
    use cmd = start dir
    cmd.Command("status") |> Parsers.parseFileStatuses

let commitStatus dir hash =
    use cmd = start dir
    cmd.Command("status", "--change", hash) |> Parsers.parseFileStatuses
    
let pull dir =
    let cmd = start dir // (!) let for ask commands
    cmd.Command("pull") |> Parsers.maybeAsk

let push dir =
    let cmd = start dir
    cmd.Command("push") |> Parsers.maybeAsk

let commit dir message files =
    let cmd = start dir
    let parameters = seq {
        yield "commit"
        yield "-A"
        yield "-m"
        yield message
        for x in files do 
            yield "-I"
            yield x
    }
    cmd.Command(parameters |> Array.ofSeq)

let init dir =
    use cmd = start dir
    match cmd.Command("init") with
    | Data chunks ->
        match Parsers.alreadyExists chunks with
        | true  -> Fail "Repository already exists"
        | false -> Success chunks
    | _ -> Fail "Init command unexpectedly requested user input"