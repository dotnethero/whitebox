namespace Whitebox.ViewModels

open System
open System.Reactive
open System.Reactive.Subjects
open System.Windows.Input

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
