namespace Whitebox.Services

open System.Windows.Forms
open Whitebox.Views

type DialogService() =

    interface IDialogService with
        member this.OpenFolder() =
            use dialog = new FolderBrowserDialog()
            match dialog.ShowDialog() with
            | DialogResult.OK -> Some dialog.SelectedPath
            | _ -> None

        member this.AskPassword(text) =
            let dialog = PasswordDialog(DataContext = text)
            match dialog.ShowDialog() with
            | x when x.HasValue && x.Value -> Some dialog.passwordBox.Password
            | _ -> None