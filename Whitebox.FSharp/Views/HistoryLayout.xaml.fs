namespace Whitebox.Views

open Microsoft.FSharp.Core
open System.Windows
open System.Windows.Shapes
open System.Windows.Media
open System.Diagnostics
open System.Windows.Controls
open System.Windows.Controls.Primitives
open Whitebox.Types
open Whitebox.ViewModels

type HistoryLayoutBase = FsXaml.XAML<"Views/HistoryLayout.xaml">
type HistoryLayout() as self =
    inherit HistoryLayoutBase()

    let radius = 5.0
    let dia = radius * 2.0
    let left = 25.0
    
    let gatherOne (lv: ListView) (element: ListViewItem) = 
        let location = element.TranslatePoint(Point(left, element.ActualHeight / 2.0 - radius), lv);
        let commit = element.DataContext :?> Changeset
        location, commit
        
    let gatherMany (lv: ListView) =
        lv.Items
        |> Seq.cast<obj>
        |> Seq.map lv.ItemContainerGenerator.ContainerFromItem
        |> Seq.cast<ListViewItem>
        |> Seq.filter (fun item -> item <> null)
        |> Seq.map (gatherOne lv)

    let makeElements (location: Point, commit) = seq {
        let dot = new Ellipse(Stroke = Brushes.Black, StrokeThickness = 1.5, Width = dia, Height = dia)
        Canvas.SetTop  (dot, location.Y)
        Canvas.SetLeft (dot, location.X)
        yield dot :> UIElement
    }

    let drawMarkersOn (canvas: Canvas) dots  =
        canvas.Children.Clear()
        dots |> Seq.iter (canvas.Children.Add >> ignore)

    do self.listview.ItemContainerGenerator.StatusChanged
        |> Observable.filter (fun _ -> self.listview.ItemContainerGenerator.Status = GeneratorStatus.ContainersGenerated)
        |> Observable.subscribe self.Draw
        |> ignore
    
    member private this.Draw e =
        this.listview
            |> gatherMany
            |> Seq.filter (fun (loc, _) -> loc.Y > 30.0)
            |> Seq.collect makeElements
            |> drawMarkersOn this.canvas
