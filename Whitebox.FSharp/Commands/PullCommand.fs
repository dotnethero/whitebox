module Whitebox.PullCommand

open System
open Whitebox
open Whitebox.Types

let execute dir =
    let cmd = new CommandServer(dir)
    cmd.Command("pull")
   