namespace Whitebox.ViewModels

open System
open System.Reactive
open System.Reactive.Linq
open Whitebox
open Whitebox.Types
open ReactiveUI
open ReactiveUI.Fody.Helpers

type Diff = Diff of Line list
type FileList = FileList of FileStatus list
type Cmd = Cmd
type Cmd<'T,'V> = ReactiveCommand<'T,'V>

type WorkspaceModel private(x:int) =
    
    let dir = "C:\Projects\Tamga"
    let status() = StatusCommand.execute dir |> FileList
    let showChanges = Cmd.Create(Func<FileList> status)
    
    [<Reactive>]
    member val DiffFile: Diff option = None with get, set

    [<Reactive>]
    member val Files: FileList option = None with get, set

    [<Reactive>]
    member val CurrentFile: FileStatus option = None with get, set

    member val ShowChanges: Cmd<Unit, FileList> = showChanges

    new() as this =
        WorkspaceModel(1)
        then 
            this.ShowChanges
            |> Observable.subscribe (fun x -> 
                this.Files <- Some x 
                printfn "%A" x)
            |> ignore

       
    //public WorkspaceModel(IHgActionCenter actionCenter)
    //{
    //    _actionCenter = actionCenter;

    //    ShowChanges = ReactiveCommand.Create(_actionCenter.Status, this.WhenNotNull(x => x._actionCenter.ActivePath), RxApp.TaskpoolScheduler);
    //    ShowChanges
    //        .Subscribe(x => Files = x);

    //    this.WhenAnyValue(model => model.CurrentFile)
    //        .Throttle(TimeSpan.FromSeconds(0.25))
    //        .ObserveOn(RxApp.TaskpoolScheduler)
    //        .Where(x => x != null)
    //        .Select(x => _actionCenter.Diff(x.FilePath))
    //        .Subscribe(x => DiffFile = x);
    //}