﻿namespace Whitebox.Services

open Whitebox.Types

type IDialogService =
    abstract member OpenFolder: unit -> string option
    abstract member AskPassword: AskPasswordOrUser -> (string * string) option
