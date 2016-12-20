namespace Whitebox

open System
open System.IO
open System.Diagnostics
open Whitebox.Types
open Whitebox.BitReader
open Whitebox.BitWriter

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
    
    let readChunks() =
        let notEOF = function
            | Exit 
            | Input _
            | LineInput _ -> false
            | _ -> true
             
        let input = function
            | Input _
            | LineInput _ -> true
            | _ -> false

        let mutable ask = false
        let readChunk() = 
            let chunk = reader.ReadOutput()
            if input chunk then ask <- true
            chunk

        let chunks = List.ofSeq (seq { 
            let mutable chunk = readChunk()
            while notEOF chunk do
                yield chunk
                chunk <- readChunk()
            yield chunk
        })

        if ask then Question chunks else Data chunks

    do reader.ReadOutput() |> ignore // hello

    member this.Command([<ParamArray>] args) = 
        writer.WriteCommand("runcommand", args)
        readChunks()

    member this.PushData(line) = 
        writer.WriteData(line)
        readChunks()
        
    interface System.IDisposable with 
        member this.Dispose() =
            reader.Dispose()
            writer.Dispose()
