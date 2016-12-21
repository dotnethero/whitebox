namespace Whitebox.ViewModels

open System
open System.Reactive
open System.Reactive.Linq
open FSharp.Control.Reactive
open FSharp.Control.Reactive.Builders
open Whitebox
open Whitebox.Types

type WorkspaceModel private() =
    
    let dir = "C:\Projects\Tamga"

    member val DiffFile: Line list option = None with get, set
    member val Files: FileStatus list option = None with get, set
    member val CurrentFile: FileStatus option = None with get, set
    member val ShowChanges: IObservable<FileStatus list> = observe { yield StatusCommand.execute dir } with get, set

    static member Create() =
        let this = WorkspaceModel()
        let interval = TimeSpan.FromSeconds(2.0)
        let logged x = 
            x |> List.iter (printfn "%A")
            this.Files <- Some x
            
        this.ShowChanges
            |> Observable.delay interval
            |> Observable.subscribe logged
            |> ignore

        this // return constructed object

//    this.WhenAnyValue(model => model.CurrentFile)
//        .Throttle(TimeSpan.FromSeconds(0.25))
//        .ObserveOn(RxApp.TaskpoolScheduler)
//        .Where(x => x != null)
//        .Select(x => _actionCenter.Diff(x.FilePath))
//        .Subscribe(x => DiffFile = x);
