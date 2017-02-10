namespace Whitebox.ViewModels

open System.ComponentModel
open System.Reactive.Subjects
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Linq.RuntimeHelpers

type ViewModel() =

    let empty = new Subject<PropertyChangedEventArgs>()
    let changed = new Event<_,_>()
    let getName = function
        | PropertyGet(_, prop, _) -> Some prop.Name
        | _ -> None
    
    member this.OnPropertyChanged (prop: Expr) =
        let name = getName prop
        match name with
        | Some p -> changed.Trigger(this, PropertyChangedEventArgs(p))
        | None -> ()

    member this.PropertyChanged = changed.Publish

    member this.WhenPropertyChanged (prop: Expr) =
        let name = getName prop
        match name with
        | Some p -> 
            this.PropertyChanged 
            |> Observable.filter (fun x -> x.PropertyName = p)
        | None -> 
            empty :> System.IObservable<PropertyChangedEventArgs>

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = changed.Publish
