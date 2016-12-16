namespace Whitebox

open System
open System.IO
open System.Diagnostics
open Whitebox.BitReader
open Whitebox.BitWriter

type CommandServer(path) =

    let logger = printfn

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

    let printOutput = function
        | Output data -> logger "Output: %s" data
        | Error data -> logger "Error: %s" data
        | Input (channel, _) -> logger "input:"
        | Exit -> logger "Exit."

    let readChunk() = reader.ReadOutput() |> printOutput

    member this.ReadChunks() =
        let generator _ = reader.ReadOutput()
        let isNotExit = function
            | Exit -> false
            | _ -> true

        Seq.initInfinite generator
            |> Seq.takeWhile isNotExit
            |> Seq.iter printOutput
            
    member this.Hello() = readChunk()
    member this.Command([<ParamArray>] args:string array) = 
        logger "Command: %s\n" (String.concat " " args)
        writer.WriteCommand("runcommand", args)

    interface System.IDisposable with 
        member this.Dispose() =
            reader.Dispose()
            writer.Dispose()
            printfn "Reader and writer disposed"
