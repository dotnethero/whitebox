module Whitebox.PullCommand

open System
open Whitebox
open Whitebox.Types

let execute dir : PushDataCommand option =

    let cmd = new CommandServer(dir)

    let pushAndClose data =
        let data = cmd.PushData data
        (cmd :> IDisposable).Dispose()
        data

    let print = List.iter (printfn "%A")

    match cmd.Command("pull") with
        | Question chunks -> 
            chunks |> print
            Some pushAndClose
        | Data chunks -> 
            chunks |> print
            None
