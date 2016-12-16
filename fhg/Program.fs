// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
// import sys, struct, subprocess
// 
// # connect to the server
// server = subprocess.Popen(['hg', '--config', 'ui.interactive=True', 'serve', '--cmdserver', 'pipe'],
//                           stdin=subprocess.PIPE, stdout=subprocess.PIPE)
// 

open System
open System.IO
open System.Diagnostics
open BitEx

let start (exe, cmdline) =
    let psi = new ProcessStartInfo(exe,cmdline) 
    psi.UseShellExecute <- false
    psi.RedirectStandardOutput <- true
    psi.RedirectStandardInput <- true
    psi.RedirectStandardError <- true
    psi.CreateNoWindow <- false
    let p = Process.Start(psi) 
    let reader = new BinaryReader(p.StandardOutput.BaseStream)
    let writer = new BinaryWriter(p.StandardInput.BaseStream)
    let readChunk() =
        match reader.ReadOutput() with
            | Output (channel, _, data) ->
                printf "%c\n" channel
                printf "%s\n" data
            | Input (channel, _) ->
                printf "Input:"
    
    let hello = readChunk()
    Console.ReadKey

// def readchannel(server):
//     channel, length = struct.unpack('>cI', server.stdout.read(5))
//     if channel in 'IL': # input
//         return channel, length
//     return channel, server.stdout.read(length)
// 

[<EntryPoint>]
let main argv = 
    start ("hg", "--config ui.interactive=True serve --cmdserver pipe") |> ignore
    0 // return an integer exit code



// def writeblock(data):
//     server.stdin.write(struct.pack('>I', len(data)))
//     server.stdin.write(data)
//     server.stdin.flush()
// 
// # read the hello block
// hello = readchannel(server)
// print "hello block:", repr(hello)
// 
// # write the command
// server.stdin.write('runcommand\n')
// writeblock('\0'.join(sys.argv[1:]))
// 
// # receive the response
// while True:
//     channel, val = readchannel(server)
//     if channel == 'o':
//         print "output:", repr(val)
//     elif channel == 'e':
//         print "error:", repr(val)
//     elif channel == 'r':
//         print "exit code:", struct.unpack(">l", val)[0]
//         break
//     elif channel == 'L':
//         print "(line read request)"
//         writeblock(sys.stdin.readline(val))
//     elif channel == 'I':
//         print "(block read request)"
//         writeblock(sys.stdin.read(val))
//     else:
//         print "unexpected channel:", channel, val
//         if channel.isupper(): # required?
//             break
// 
// # shut down the server
// server.stdin.close()
