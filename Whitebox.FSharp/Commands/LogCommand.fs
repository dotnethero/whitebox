module Whitebox.LogCommand

let execute dir limit =
    use cmd = new CommandServer(dir)
    cmd.Command("log", "-l", string limit, "-T", Parsers.changesetTemplate) |> Parsers.parseChangesets
