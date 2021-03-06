﻿namespace Whitebox.Views

open System

type PasswordDialogBase = FsXaml.XAML<"Views/PasswordDialog.xaml">
type PasswordDialog() =
    inherit PasswordDialogBase()

    override this.OkClick(_,_) = 
        this.DialogResult <- Nullable true
        this.Close()

    override this.CancelClick(_,_) = 
        this.DialogResult <- Nullable false
        this.Close()
