module Whitebox.BitReader

open System
open System.IO

type Channel = char

type BufferSize = uint32

type ChannelOutput =
    | Output of string
    | Error of string
    | Input of Channel * BufferSize
    | Exit

let utf8 = System.Text.Encoding.UTF8

type BinaryReader with
    member this.ReadChannel(): Channel = this.ReadChar()

    member this.ReadLength(): BufferSize =
        let bytes = Array.init 4 (fun x -> byte 0)
        this.Read(bytes, 0, 4) |> ignore
        Array.Reverse(bytes)
        BitConverter.ToUInt32(bytes, 0)

    member this.ReadData(len) =
        let data = this.ReadBytes(int len);
        utf8.GetString(data);

    member this.ReadOutput() =
        let channel = this.ReadChannel()
        let len = this.ReadLength()
        match channel with
            | 'L' | 'I' -> 
                Input (channel, len)
            | 'o' | 'e' -> 
                let data = this.ReadData(len)
                match channel with
                    | 'o' -> Output data
                    | 'e' -> Error data

            | _ -> Exit

