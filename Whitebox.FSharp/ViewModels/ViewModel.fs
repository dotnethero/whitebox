namespace Whitebox.ViewModels

open System.ComponentModel
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

type ViewModel() =

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

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = changed.Publish
