open System
open Whitebox

[<EntryPoint>]
let main argv = 
    let dir = "C:\Projects\Tamga"
    printfn "%s" dir
    use cmd = new CommandServer(dir)

    printfn "Press any key to log . . ."
    Console.ReadKey(true) |> ignore
    cmd.Command("log", "-l", "5") |> ignore

    printfn "Press any key to pull . . ."
    Console.ReadKey(true) |> ignore
    let result = cmd.Command("pull")
    match result with
        | Question _ ->
            printf "Password: "
            let data = Console.ReadLine()
            cmd.PushData(data) |> ignore
        | Data _ -> ()

    printfn "Press any key to exit . . ."
    Console.ReadKey(true) |> ignore
    0 // return an integer exit code
