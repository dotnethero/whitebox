module Whitebox.StatusCommand

let execute dir =
    use cmd = new CommandServer(dir)
    cmd.Command("status") |> Parsers.parseFileStatuses

let byHash dir hash =
    use cmd = new CommandServer(dir)
    cmd.Command("status", "--change", hash) |> Parsers.parseFileStatuses
