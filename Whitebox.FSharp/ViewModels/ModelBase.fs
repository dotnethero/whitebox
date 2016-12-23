namespace Whitebox.ViewModels


open System
open System.ComponentModel
open System.Reactive
open System.Reactive.Subjects
open System.Windows.Input
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

type ReactiveCommand<'T>(action) =
    let canExecuteEvent = new Event<_,_>()
    let subject = new Subject<'T>()
    interface ICommand with
        member this.Execute _ = subject.OnNext(action())
        member this.CanExecute _ = true
        [<CLIEvent>] 
        member this.CanExecuteChanged = canExecuteEvent.Publish
    interface IObservable<'T> with
        member this.Subscribe observer = subject.Subscribe observer

type ModelBase() =
    let propertyChanged = new Event<_, _>()
    let toPropName(query : Expr) = 
        match query with
        | PropertyGet(a, b, list) ->
            b.Name
        | _ -> ""

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = propertyChanged.Publish

    abstract member OnPropertyChanged: string -> unit
    default x.OnPropertyChanged(propertyName : string) =
        propertyChanged.Trigger(x, new PropertyChangedEventArgs(propertyName))

    member x.OnPropertyChanged(expr: Expr) =
        let propName = toPropName(expr)
        x.OnPropertyChanged(propName)