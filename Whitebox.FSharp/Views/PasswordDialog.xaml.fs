namespace Whitebox.Views

open System
open System.Windows.Forms
open Whitebox.ViewModels

type PasswordDialogBase = FsXaml.XAML<"Views/PasswordDialog.xaml">
type PasswordDialog() =
    inherit PasswordDialogBase()

    override this.OkClick(_,_) = 
        this.DialogResult <- Nullable true
        this.Close()

    override this.CancelClick(_,_) = 
        this.DialogResult <- Nullable false
        this.Close()

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
