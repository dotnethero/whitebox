namespace Whitebox.ViewModels

open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Windows.Input
open FSharp.Control.Reactive
open FSharp.Control.Reactive.Builders
open Whitebox
open Whitebox.Types

type WorkspaceModel() =
    inherit ModelBase()
    
    let dir = "C:\Projects\Tamga"

    let mutable files : FileStatus list option = None
    let mutable currentFile : FileStatus option = None
    let mutable diff : Line list option = None

    member x.Files
        with get() = files
        and set(v) = 
            files <- v
            x.OnPropertyChanged(<@ x.Files @>)
            
    member x.CurrentFile
        with get() = currentFile
        and set(v) = 
            currentFile <- v
            x.OnPropertyChanged(<@ x.CurrentFile @>)
            
    member x.Diff
        with get() = diff
        and set(v) = 
            diff <- v
            x.OnPropertyChanged(<@ x.Diff @>)

    member val ShowChanges = ReactiveCommand(fun x -> StatusCommand.execute dir) with get

    static member Create() =
        let this = WorkspaceModel()
        let assign x = 
            List.iter (printfn "%A") x
            this.Files <- Some x
        
        this.ShowChanges
            |> Observable.subscribe assign
            |> ignore
            
        this // return constructed object

//    this.WhenAnyValue(model => model.CurrentFile)
//        .Throttle(TimeSpan.FromSeconds(0.25))
//        .ObserveOn(RxApp.TaskpoolScheduler)
//        .Where(x => x != null)
//        .Select(x => _actionCenter.Diff(x.FilePath))
//        .Subscribe(x => DiffFile = x);
