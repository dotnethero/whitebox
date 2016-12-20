open System
open Whitebox
open Whitebox.Types
open Whitebox.ViewModels

let ``test status`` dir =
    StatusCommand.execute dir
        |> List.iter (printfn "%A")

let ``test diff`` dir =
    DiffCommand.execute dir "heads.txt"
        |> List.iter (printfn "%A")
        
let ``test diff by hash`` dir =
    DiffCommand.byHash dir ".hgtags" "1f7ad170cb95"
        |> List.iter (printfn "%A")

let ``test log`` dir =
    LogCommand.execute dir 10
        |> List.iter (printfn "%A")

let ``test pull`` dir =
    match PullCommand.execute dir with
        | Some cmd ->
            printf "Password: "
            let pswd = Console.ReadLine()
            let result = cmd pswd
            match result with
                | Data chunks -> chunks |> List.iter (printfn "%A")
                | _ -> ()

        | None -> printfn "Exit."

[<EntryPoint>]
let main argv = 
    let dir = "C:\Projects\Tamga"
    printfn "Workspace: %s" dir
    
    let m = WorkspaceModel()
    m.ShowChanges.Execute().Subscribe();

    printfn "Press any key to exit . . ."
    Console.ReadKey(true) |> ignore
    0 // return an integer exit code
