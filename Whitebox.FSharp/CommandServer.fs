namespace Whitebox

open System
open System.IO
open System.Diagnostics
open Whitebox.BitReader
open Whitebox.BitWriter

type CommandResult =
    | Data of Chunk list
    | Question of Chunk list

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

    let printOutput = function
        | Output data
        | Error data -> printf "%s" data
        | Input _ 
        | LineInput _ -> printf ""
        | Exit -> printf "Exit."

    let hello() = 
        reader.ReadOutput() |> printOutput
        printf "\n\n"

    let readChunks(): CommandResult =
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
        let gen() = 
            let chunk = reader.ReadOutput()
            if input chunk then ask <- true
            chunk

        let chunks = List.ofSeq (seq { 
            let mutable chunk = gen()
            while notEOF chunk do
                yield chunk
                chunk <- gen()
            yield chunk
        })

        chunks |> List.iter printOutput // print
        printf "\n\n"
        if ask then Question chunks else Data chunks

    do hello()

    member this.Command([<ParamArray>] args:string array) = 
        writer.WriteCommand("runcommand", args)
        readChunks()

    member this.Data(line: string) = 
        writer.WriteInput(line)
        readChunks()
        
    interface System.IDisposable with 
        member this.Dispose() =
            reader.Dispose()
            writer.Dispose()

