namespace Whitebox.ViewModels

open System.Windows.Forms

type PasswordViewModel(text) =
    inherit ViewModel()
    member val Text: string = text with get, set
    member val Password: string = "" with get, set

type IDialogService =
    abstract member OpenFolder: unit -> string option
    abstract member AskPassword: string -> string option

type DialogService() =

    interface IDialogService with
        member this.OpenFolder() =
            use dialog = new FolderBrowserDialog()
            match dialog.ShowDialog() with
            | DialogResult.OK -> Some dialog.SelectedPath
            | _ -> None

        member this.AskPassword(text) =
            Some "stub"
            //let vm = PasswordViewModel(text)
            //let dialog = PasswordDialog(vm)
            //match dialog.ShowDialog() with
            //| x when x.HasValue && x.Value -> Some vm.Password
            //| _ -> None