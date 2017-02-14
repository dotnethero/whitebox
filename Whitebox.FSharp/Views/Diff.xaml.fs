namespace Whitebox.Views

open System.Windows.Documents
open System.Collections.Generic
open Whitebox.Types
open System.Windows
open System.Windows.Media

// Demonstrates technique for adding "code behind" logic
// Use a different name for the base class, then inherit to add code behind
type DiffBase = FsXaml.XAML<"Views/Diff.xaml">

// Inherited class is MainView, which is referred to/used in MainWindow.xaml directly
type Diff() =
    inherit DiffBase()

    // Note the event handler for the XAML-specified event.    
    // Unlike in C#, the type provider exposes this as a virtual method,
    // which you can override as needed

    let convertType t =
        match t with
        | LineType.Add -> Color.FromRgb(232uy, 249uy, 210uy)
        | LineType.Remove -> Color.FromRgb(241uy, 218uy, 221uy)
        | _ -> Color.FromRgb(0uy, 0uy, 0uy)
    
    let convertLine line =
        let color = convertType line.Type
        let para = new Paragraph(Margin = new Thickness(), Background = new SolidColorBrush(color))
        para.Inlines.Add line.Text
        para
            
    override this.DataContextChanged(_,e) = 
        let context = unbox<IEnumerable<Line>> e.NewValue
        let doc = new FlowDocument()
        let add x = doc.Blocks.Add(x)
        context |> Seq.map (convertLine >> add) |> ignore
        //RichDiff.Document = document
        ()
