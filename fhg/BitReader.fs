module BitEx

open System
open System.IO

type Channel = char
type BufferSize = uint32
type ChannelOutput =
    | Output of Channel * BufferSize * string
    | Input of Channel * BufferSize
    | Exit

type BinaryReader with
    member this.ReadChannel(): Channel = this.ReadChar()
    member this.ReadLength(): BufferSize =
        let bytes = Array.init 4 (fun x -> byte 0)
        this.Read(bytes, 0, 4) |> ignore
        Array.Reverse(bytes)
        BitConverter.ToUInt32(bytes, 0)
    member this.ReadData(len) =
        let data = this.ReadBytes(int len);
        System.Text.Encoding.UTF8.GetString(data);

    member this.ReadOutput(): ChannelOutput =
        let channel = this.ReadChannel()
        let len = this.ReadLength()
        match channel with
            | 'L' | 'I' -> 
                Input (channel, len)
            | 'o' | 'e' -> 
                let data = this.ReadData(len)
                Output (channel, len, data)
            | _ -> Exit

        

