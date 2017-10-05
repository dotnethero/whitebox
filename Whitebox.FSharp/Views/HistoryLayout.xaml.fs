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
open FSharp.Control.Reactive

type HistoryLayoutBase = FsXaml.XAML<"Views/HistoryLayout.xaml">
type HistoryLayout() as self =
    inherit HistoryLayoutBase()

    let radius = 5.0
    let dia = radius * 2.0
    let left = 25.0
    
    let gatherMany (lv: ListView) =
        let generator = lv.ItemContainerGenerator
        let toListViewItem item = (generator.ContainerFromItem item) :?> ListViewItem
        let toPoint (element: ListViewItem) = element.TranslatePoint(Point(left, element.ActualHeight / 2.0), lv)
        let asChangeset (ch: obj) = ch :?> Changeset
        lv.Items
            |> Seq.cast<obj>
            |> Seq.map (fun ch -> (toListViewItem ch, asChangeset ch))
            |> Seq.filter (fun (lvi, _) -> lvi <> null)
            |> Seq.map (fun (lvi, ch) -> (toPoint lvi, ch))

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
        line.StrokeThickness <- 1.5
        line.Stroke <- brush
        line :> UIElement

    let makeMarkeredLine ((p1: Point, c1), (p2: Point, c2)) = seq {
        yield makeLine ((p1, c1), (p2, c2))
        yield makeMarker (p1, c1)
        yield makeMarker (p2, c2)
    }
    
    do self.listview.ItemContainerGenerator.StatusChanged
        |> Observable.filter (fun _ -> 
            self.listview.ItemContainerGenerator.Status = GeneratorStatus.ContainersGenerated &&
            self.listview.Items.Count > 0)
        |> Observable.subscribe self.Draw
        |> ignore

    member private this.Draw e =

        let clear _ = this.canvas.Children.Clear()
        let draw el = this.canvas.Children.Add el |> ignore

        let items = 
            this.listview
            |> gatherMany
            |> Seq.filter (fun (p, _) -> p.Y > 30.0)
            |> Seq.toList

        clear()

        let rec createTree child intendation =
            let (_, hc) = child;
            let parent1 = items |> List.tryFind (fun (_,c) -> hc.Parent1 = c.Hash)
            let parent2 = items |> List.tryFind (fun (_,c) -> hc.Parent2 = c.Hash)
            match (parent1, parent2) with
                | (Some parent1, Some parent2) -> 
                    let (childPoint: Point, _) = child
                    let (parentPoint: Point, changeset) = parent2  
                    let newParentPoint = Point(childPoint.X + 50.0, parentPoint.Y)
                    let newParent2 = (newParentPoint, changeset)
                    [(child, parent1); (child, newParent2)] @ createTree parent1 intendation @ createTree newParent2 intendation

                | (Some parent1, None) -> [(child, parent1)] @ createTree parent1 intendation
                | _ -> []
        
        if not items.IsEmpty then
            createTree items.Head 0.0
                |> Seq.collect makeMarkeredLine
                |> Seq.iter draw

