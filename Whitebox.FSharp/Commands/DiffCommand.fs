module Whitebox.DiffCommand

let execute dir path =
    use cmd = new CommandServer(dir)
    cmd.Command("diff", path) |> Parsers.parseLines

let byHash dir path hash =
    use cmd = new CommandServer(dir)
    cmd.Command("diff", path, "-c", hash) |> Parsers.parseLines
