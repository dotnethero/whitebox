module Whitebox.PullCommand

let execute dir =
    let cmd = new CommandServer(dir)
    cmd.Command("pull") |> Parsers.maybeAsk