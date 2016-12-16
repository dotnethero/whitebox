module Whitebox.BitWriter

open System
open System.IO

let utf8 = System.Text.Encoding.UTF8

type BinaryWriter with

    member this.WriteLength(len:uint32) =
        let bytes = BitConverter.GetBytes(len)
        Array.Reverse(bytes)
        this.Write(bytes)

    member this.WriteCommand (command:string, [<ParamArray>] args:string array) =
        let comBytes = utf8.GetBytes(command + "\n")
        this.Write(comBytes)
        let writeArgs() =
            let argString = String.concat "\u0000" args
            let argBytes = utf8.GetBytes(argString)
            this.WriteLength(uint32 argBytes.Length)
            this.Write(argBytes)
            this.Flush()

        match args.Length with
            | 0 -> ()
            | _ -> writeArgs()
                

    member this.WriteInput (line:string) =
        let comBytes = utf8.GetBytes(line + "\n")
        this.WriteLength(uint32 comBytes.Length)
        this.Write(comBytes)
        this.Flush()
