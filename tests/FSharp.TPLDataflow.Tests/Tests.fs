module FSharp.TPLDataflow.Tests

open System.Threading.Tasks.Dataflow
open System.Threading.Tasks.Dataflow.FSharp.TPLDataflow
open Xunit
open System.Linq

[<Fact>]
let ``hello tpl world`` () =
    
    let intoWords = mapBlock (fun (s:string) -> s.Split ' ')
    let filterWords =
        mapBlock (
            fun (words:string array) ->
                words |> Array.filter (fun w -> w.Length > 3) |> Array.distinct
        )
    let eachParallel = flatpMapBlock (fun (words:string array) -> words.AsParallel().Select(id) |> seq  )
    
    let printWords = actionBlock (printfn "%s")
    
    let propogateCompletion = DataflowLinkOptions (PropagateCompletion=true)
    use intoToFilter = intoWords |> linkToWithOptions filterWords propogateCompletion
    use inParallel = filterWords |> linkToWithOptions eachParallel propogateCompletion
    use printFiltered = eachParallel |> linkToWithOptions printWords propogateCompletion
    
    intoWords.Post("hello tpl world") |> ignore
    intoWords.Complete ()
    
    printWords.Completion.Wait ()
