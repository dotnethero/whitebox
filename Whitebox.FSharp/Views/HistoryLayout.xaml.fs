namespace Whitebox.Views

open Microsoft.FSharp.Core
open System.Windows
open System.Windows.Shapes
open System.Windows.Media
open System.Diagnostics
open System.Windows.Controls
open System.Windows.Controls.Primitives
open Whitebox.ViewModels

type HistoryLayoutBase = FsXaml.XAML<"Views/HistoryLayout.xaml">
type HistoryLayout() as self =
    inherit HistoryLayoutBase()
    
    let notNull x = x <> null

    let makeMarkerFor (lv: ListView) (element: ListViewItem) = 
        let location = element.TranslatePoint(Point(25.0, element.ActualHeight / 2.0 - 5.0), lv);
        let dot = new Ellipse(Stroke = Brushes.Black, StrokeThickness = 1.5, Width = 10.0, Height = 10.0)
        Canvas.SetTop  (dot, location.Y)
        Canvas.SetLeft (dot, location.X)
        dot

    let makeMarkers (lv: ListView) =
        lv.Items
        |> Seq.cast<obj>
        |> Seq.map lv.ItemContainerGenerator.ContainerFromItem
        |> Seq.cast<ListViewItem>
        |> Seq.filter notNull
        |> Seq.map (makeMarkerFor lv)

    let drawMarkersOn (canvas: Canvas) dots  =
        canvas.Children.Clear()
        dots |> Seq.iter (canvas.Children.Add >> ignore)

    do self.listview.ItemContainerGenerator.StatusChanged
        |> Observable.filter (fun _ -> self.listview.ItemContainerGenerator.Status = GeneratorStatus.ContainersGenerated)
        |> Observable.subscribe self.Draw
        |> ignore
    
    member private this.Draw e =
        this.listview
            |> makeMarkers
            |> drawMarkersOn this.canvas
