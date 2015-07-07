
module FSharp.TPLDataflow 

open System
open System.Threading.Tasks
open System.Threading.Tasks.Dataflow

let inline action f = new Action<_>(f)
let inline predicate f = new Predicate<_>(f)
let private defaultExecutionOptions = new ExecutionDataflowBlockOptions()
let inline startAsPlainTask (work : Async<unit>) = Task.Factory.StartNew(fun () -> work |> Async.RunSynchronously)

//execution blocks
let actionBlockWithOptions f opt = ActionBlock<_>(f |> action, opt)
let actionBlock f = actionBlockWithOptions f defaultExecutionOptions
let actionBlockAsyncWithOptions (f: 'a -> Async<unit>) opt = ActionBlock<_>(f >> startAsPlainTask, opt)
let actionBlockAsync f = actionBlockAsyncWithOptions f defaultExecutionOptions
   
let transformBlockWithOptions (f: 'a -> 'b) opt = TransformBlock<_,_>(f, opt)
let transformBlock f = transformBlockWithOptions f defaultExecutionOptions
let transformBlockAsyncWithOptions (f : 'a -> Async<'b>) opt = TransformBlock<'a,'b>(f >> Async.StartAsTask, opt)
let transformBlockAsync f = transformBlockAsyncWithOptions f defaultExecutionOptions

let transformManyBlockWithOptions (f : 'a -> seq<'b>) opt = TransformManyBlock<_,_>(f, opt)
let transformManyBlock (f : 'a -> seq<'b>) = transformManyBlockWithOptions f defaultExecutionOptions    
let transformManyBlockAsyncWithOptions (f : 'a -> Async<seq<'b>>) opt =  TransformManyBlock(f >> Async.StartAsTask, opt)
let transformManyBlockAsync (f : 'a -> Async<seq<'b>>) = transformManyBlockAsyncWithOptions f defaultExecutionOptions

let linkToWithOptionsAndFilter target opt pred (source : ISourceBlock<_>) =
    source.LinkTo(target, opt, pred |> predicate)
let linkToWithOptions target opt (source : ISourceBlock<_>) =
    source.LinkTo(target, opt)
let linkToWithFilter target pred (source : ISourceBlock<_>) =
    source.LinkTo(target, pred |> predicate)
let linkTo target (source : ISourceBlock<_>) =
    source.LinkTo(target)

let (&>) source target = linkTo target source

let toObservable (source : ISourceBlock<_>) = source.AsObservable()
let toObserver (target : ITargetBlock<_>) = target.AsObserver()

let (!>) message (target : ITargetBlock<_>)  = target.SendAsync(message)
let (<!) (target : ITargetBlock<_>) message  = target.SendAsync(message)  