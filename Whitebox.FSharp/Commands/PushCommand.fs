module Whitebox.PushCommand

let execute dir =
    let cmd = new CommandServer(dir)
    cmd.Command("push") |> Parsers.maybeAsk