namespace Whitebox.ViewModels

type PasswordViewModel(text) =
    inherit ViewModel()
    member val Text: string = text with get, set
    member val Password: string = "" with get, set