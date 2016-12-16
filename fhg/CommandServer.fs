namespace Whitebox

open System
open System.IO
open System.Diagnostics
open Whitebox.BitReader
open Whitebox.BitWriter

type CommandServer(exe, cmdline) =

    let psi = 
        let psi = ProcessStartInfo(exe,cmdline) 
        psi.UseShellExecute <- false
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardInput <- true
        psi.RedirectStandardError <- true
        psi.CreateNoWindow <- false
        psi.EnvironmentVariables.["HGENCODING"] <- "utf-8";
        psi

    let proc = Process.Start(psi) 
    let reader = new BinaryReader(proc.StandardOutput.BaseStream)
    let writer = new BinaryWriter(proc.StandardInput.BaseStream)
    let readChunk() =
        match reader.ReadOutput() with
            | Output (channel, _, data) ->
                printf "%c\n" channel
                printf "%s\n" data
            | Input (channel, _) ->
                printf "Input:"
            | Exit -> 
                printf "Exit."

    member this.ReadChunks() =
        while true do
            match reader.ReadOutput() with
                | Output (channel, _, data) ->
                    printf "%c\n" channel
                    printf "%s\n" data
                | Input (channel, _) ->
                    printf "Input:"
                | Exit -> 
                    printf "Exit."
    
    member this.Hello() = readChunk()
    member this.Command([<ParamArray>] args:string array) = writer.WriteCommand("runcommand", args)

    interface System.IDisposable with 
        member this.Dispose() =
            reader.Dispose()
            writer.Dispose()
            printfn "Reader and writer disposed"
