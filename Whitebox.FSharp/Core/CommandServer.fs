﻿namespace Whitebox

open System
open System.IO
open System.Diagnostics
open Whitebox.Types
open Whitebox.BitReader
open Whitebox.BitWriter

module Open =
    let directory path =
        let psi = ProcessStartInfo("explorer", path) 
        Process.Start(psi) |> ignore

type CommandServer(path) =

    let psi = 
        let psi = ProcessStartInfo("hg", "--config ui.interactive=True serve --cmdserver pipe") 
        psi.UseShellExecute <- false
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardInput <- true
        psi.RedirectStandardError <- true
        psi.CreateNoWindow <- true
        psi.WorkingDirectory <- path
        psi.EnvironmentVariables.["HGENCODING"] <- "utf-8";
        psi

    let proc = Process.Start(psi) 
    let reader = new BinaryReader(proc.StandardOutput.BaseStream)
    let writer = new BinaryWriter(proc.StandardInput.BaseStream)
    
    let rec readChunks() =
        let (|NotEOF|_|) = function
            | Exit 
            | Input _
            | LineInput _ -> None
            | x -> Some x
             
        let (|NeedAsk|) = function
            | Input _
            | LineInput _ -> true
            | _ -> false

        let mutable ask = false
        let readChunk() = 
            let chunk = reader.ReadOutput()
            match chunk with 
            | NeedAsk(b) -> ask <- b
            chunk

        let rec getChunks lst =
            match readChunk() with
            | NotEOF x -> x :: getChunks lst
            | _ -> []
            
        let chunks = getChunks[]
        if ask then Callback (chunks, push, close) else Data chunks

    and push data =
        writer.WriteData(data)
        readChunks()

    and close() = 
        reader.Dispose()
        writer.Dispose()

    do reader.ReadOutput() |> ignore // hello

    member this.Command([<ParamArray>] args) = 
        writer.WriteCommand("runcommand", args)
        readChunks()

    member this.PushData(line) = 
        writer.WriteData(line)
        readChunks()
        
    interface System.IDisposable with 
        member this.Dispose() =
            close()
