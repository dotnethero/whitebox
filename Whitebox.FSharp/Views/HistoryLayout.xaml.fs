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
        let location = element.TranslatePoint(Point(left, element.ActualHeight / 2.0), lv);
        let commit = element.DataContext :?> Changeset
        location, commit
        
    let gatherMany (lv: ListView) =
        lv.Items
        |> Seq.cast<obj>
        |> Seq.map lv.ItemContainerGenerator.ContainerFromItem
        |> Seq.cast<ListViewItem>
        |> Seq.filter (fun item -> item <> null)
        |> Seq.map (gatherOne lv)

    let makeMarker (location: Point, _) = 
        let dot = new Ellipse(Fill = Brushes.White, Stroke = Brushes.Black, StrokeThickness = 1.5, Width = dia, Height = dia)
        Canvas.SetTop  (dot, location.Y - radius)
        Canvas.SetLeft (dot, location.X - radius)
        dot :> UIElement
    
    let makeLine ((p1: Point, _), (p2: Point, _)) = 
        let line = Line()
        line.X1 <- p1.X
        line.Y1 <- p1.Y
        line.X2 <- p2.X
        line.Y2 <- p2.Y
        let brush = Brushes.Black
        line.StrokeThickness <- 2.0
        line.Stroke <- brush
        line :> UIElement
    
    do self.listview.ItemContainerGenerator.StatusChanged
        |> Observable.filter (fun _ -> self.listview.ItemContainerGenerator.Status = GeneratorStatus.ContainersGenerated)
        |> Observable.subscribe self.Draw
        |> ignore
        
    member private this.Draw e =

        let clear _ = this.canvas.Children.Clear()
        let draw el = this.canvas.Children.Add el |> ignore

        let items = 
            this.listview
            |> gatherMany
            |> Seq.filter (fun (loc, _) -> loc.Y > 30.0)
            |> Seq.toList

        clear()
        items
            |> List.allPairs items
            |> List.filter (fun ((_, c1), (_, c2)) -> 
                c1.Parent1 = c2.Hash ||
                c1.Parent2 = c2.Hash )
            |> List.map makeLine
            |> List.iter draw

        items
            |> List.map makeMarker
            |> List.iter draw

