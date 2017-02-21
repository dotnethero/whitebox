namespace Whitebox.Services

open System.Windows.Forms
open Whitebox.Views
open Whitebox.Types
open System.Windows.Media

type DialogService() =

    interface IDialogService with
        member this.OpenFolder() =
            use dialog = new FolderBrowserDialog()
            match dialog.ShowDialog() with
            | DialogResult.OK -> Some dialog.SelectedPath
            | _ -> None

        member this.AskPassword(ask) =
            let create text readonlyUser = 
                let window = PasswordDialog(DataContext = text)
                window.userBox.IsReadOnly <- readonlyUser
                window.userBox.Background <- if readonlyUser then SolidColorBrush(Colors.WhiteSmoke) else SolidColorBrush(Colors.White)
                window

            let dialog = 
                match ask with
                | AskUser text -> create text false
                | AskPassword text -> create text true

            match dialog.ShowDialog() with
            | x when x.HasValue && x.Value -> Some (dialog.userBox.Text, dialog.passwordBox.Password)
            | _ -> None