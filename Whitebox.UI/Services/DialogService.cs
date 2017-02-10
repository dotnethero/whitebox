using System;
using System.Windows.Forms;
using Microsoft.FSharp.Core;
using Whitebox.ViewModels;
using Whitebox.UI.Views;

namespace Whitebox.UI
{
    public class DialogService : IDialogService
    {
        FSharpOption<string> IDialogService.OpenFolder()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return new FSharpOption<string>(dialog.SelectedPath);
            }
            return FSharpOption<string>.None;
        }

        FSharpOption<string> IDialogService.AskPassword(string text)
        {
            var context = new PasswordViewModel
            {
                Text = text
            };
            var dialog = new PasswordDialog(context);
            if (dialog.ShowDialog() == true)
            {
                return new FSharpOption<string>(context.Password);
            }
            return FSharpOption<string>.None;
        }
    }
}
