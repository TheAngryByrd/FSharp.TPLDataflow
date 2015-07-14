namespace Microsoft.Threading.Tasks.Dataflow.FSharp

[<AutoOpen>]
module TPLDataflowEx =
    open System
    open System.Threading
    open System.Threading.Tasks.Dataflow
    
    type ITargetBlock<'a> with     
        member x.AsyncSend<'a> (msg, ct) = DataflowBlock.SendAsync<'a>(x,msg,ct) |> Async.AwaitTask
        member x.AsyncSend<'a> msg = x.AsyncSend(msg, CancellationToken.None)

    type ISourceBlock<'a> with
        member x.AsyncReceive<'a> (ct : CancellationToken) = DataflowBlock.ReceiveAsync<'a>(x,ct) |> Async.AwaitTask
        member x.AsyncReceive<'a> (timeout : TimeSpan, ct : CancellationToken) = DataflowBlock.ReceiveAsync<'a>(x,timeout,ct) |> Async.AwaitTask |> Async.Catch
        member x.AsyncReceive<'a> (timeout : TimeSpan) = x.AsyncReceive(timeout, CancellationToken.None)
        member x.AsyncReceive<'a> () = x.AsyncReceive(CancellationToken.None)
        member x.AsyncOutputAvailable<'a> ct = x.OutputAvailableAsync(ct) |> Async.AwaitTask
        member x.AsyncOutputAvailable<'a> () = x.AsyncOutputAvailable(CancellationToken.None)

module TPLDataflow =

    open System
    open System.Threading.Tasks
    open System.Threading.Tasks.Dataflow

    let inline action f = new Action<_>(f)
    let inline predicate f = new Predicate<_>(f)
    let private defaultExecutionOptions = new ExecutionDataflowBlockOptions()
    let private defaultDataflowLinkOptions = new DataflowLinkOptions()
    let inline startAsPlainTask (work : Async<unit>) = Task.Factory.StartNew(fun () -> work |> Async.RunSynchronously)

    /// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> class with the specified action.</summary>  
    /// <param name="f">The function to invoke with each data element received.</param>
    /// <param name="executionOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" />.</param>
    let actionBlockWithOptions f executionOptions = ActionBlock<_>(f |> action, executionOptions)

    /// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> class with the specified action.</summary>  
    /// <param name="f">The function to invoke with each data element received.</param>
    let actionBlock f = actionBlockWithOptions f defaultExecutionOptions

    /// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> class with the specified action.</summary>  
    /// <param name="f">The async function to invoke with each data element received.</param>
    /// <param name="executionOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" />.</param>
    let actionBlockAsyncWithOptions (f: 'a -> Async<unit>) executionOptions = ActionBlock<_>(f >> startAsPlainTask, executionOptions)

    /// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> class with the specified action.</summary>  
    /// <param name="f">The async function to invoke with each data element received.</param>
    let actionBlockAsync f = actionBlockAsyncWithOptions f defaultExecutionOptions

    
    /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" /> with the specified <see cref="T:System.Func`2" /></summary>
    /// <param name="f">The function to tranform each element received.</param>
    /// <param name="executionOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" />.</param>
    let mapBlockWithOptions (f: 'a -> 'b) executionOptions = TransformBlock<_,_>(f, executionOptions)
    
    /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" /> with the specified <see cref="T:System.Func`2" /></summary>
    /// <param name="f">The function to tranform each element received.</param>
    let mapBlock f = mapBlockWithOptions f defaultExecutionOptions
    
    /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" /> with the specified <see cref="T:System.Func`2" /></summary>
    /// <param name="f">The async function to tranform each element received.</param>
    /// <param name="executionOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" />.</param>
    let mapBlockAsyncWithOptions (f : 'a -> Async<'b>) executionOptions = TransformBlock<'a,'b>(f >> Async.StartAsTask, executionOptions)
    
    /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" /> with the specified <see cref="T:System.Func`2" /></summary>
    /// <param name="f">The async function to tranform each element received.</param>
    let mapBlockAsync f = mapBlockAsyncWithOptions f defaultExecutionOptions  
  
    ///  <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /> with the specified function</summary>  
    /// <param name="f">The function to tranform each element received. All of the data from the returned in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> will be made available as output from this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /></param>
    /// <param name="executionOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</param>
    let flatpMapBlockWithOptions (f : 'a -> seq<'b>) executionOptions = TransformManyBlock<_,_>(f, executionOptions)

    ///  <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /> with the specified function</summary>  
    /// <param name="f">The function to tranform each element received. All of the data from the returned in the <see cref="T:System.Collections.Generic.IEnumer
    let flatpMapBlock (f : 'a -> seq<'b>) = flatpMapBlockWithOptions f defaultExecutionOptions    

    ///  <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /> with the specified function</summary>  
    /// <param name="f">The async function to tranform each element received. All of the data from the returned in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> will be made available as output from this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /></param>
    /// <param name="executionOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</param>
    let flatpMapBlockAsyncWithOptions (f : 'a -> Async<seq<'b>>) executionOptions =  TransformManyBlock(f >> Async.StartAsTask, executionOptions)

    ///  <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /> with the specified function</summary>  
    /// <param name="f">The async function to tranform each element received. All of the data from the returned in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> will be made available as output from this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /></param>
    let flatpMapBlockAsync (f : 'a -> Async<seq<'b>>) = flatpMapBlockAsyncWithOptions f defaultExecutionOptions

    /// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>   
    /// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect the source.</param>
    /// <param name="dataflowOptions">>A <see cref="T:System.Threading.Tasks.Dataflow.DataflowLinkOptions" /> instance that configures the link.</param>
    /// <param name="filter">The filter a message must pass in order for it to propagate from the source to the target.</param>
    /// <param name="source">The source from which to link.</param>
    /// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>      
    let linkToWithOptionsAndFilter target dataflowOptions filter (source : ISourceBlock<_>) =
        source.LinkTo(target, dataflowOptions, filter |> predicate)

    /// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>   
    /// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect the source.</param>
    /// <param name="dataflowOptions">>A <see cref="T:System.Threading.Tasks.Dataflow.DataflowLinkOptions" /> instance that configures the link.</param>
    /// <param name="source">The source from which to link.</param>
    /// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>      
    let linkToWithOptions target dataflowOptions (source : ISourceBlock<_>) =
        source.LinkTo(target, dataflowOptions)

    /// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>   
    /// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect the source.</param>
    /// <param name="filter">The filter a message must pass in order for it to propagate from the source to the target.</param>
    /// <param name="source">The source from which to link.</param>
    /// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>      
    let linkToWithFilter target filter (source : ISourceBlock<_>) =
        source.LinkTo(target, filter |> predicate)

    /// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>   
    /// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect the source.</param>
    /// <param name="source">The source from which to link.</param>
    /// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>      
    let linkTo target (source : ISourceBlock<_>) =
        linkToWithOptions target defaultDataflowLinkOptions source

    /// <summary>Signals to the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> that it should not accept nor produce any more messages nor consume any more postponed messages.</summary>
    /// <param name="source">The source to signal completion</param>
    let complete (source : IDataflowBlock) = source.Complete()
   
    /// <summary>Causes the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> to complete in a <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state.</summary>
    /// <param name="ex">The <see cref="T:System.Exception" /> that caused the faulting.</param>
    /// <param name="source">The source to signal failure</param>
    let fail ex (source : IDataflowBlock) = source.Fault ex

    /// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>   
    /// <param name="source">The source from which to link.</param> 
    /// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect the source.</param>
    ///<returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>  
    let (&>) source target = linkTo target source

    /// <summary>Creates a new <see cref="T:System.IObservable`1" /> abstraction over the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" />.</summary>
    /// <param name="source">The source to wrap</param>
    let toObservable (source : ISourceBlock<_>) = source.AsObservable()
    
    /// <summary>Creates a new <see cref="T:System.IObserver`1" /> abstraction over the <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary> 
    /// <param name="target">The target to wrap</param>
    let toObserver (target : ITargetBlock<_>) = target.AsObserver()

    /// <summary>Asynchronously offers a message to the target message block, allowing for postponement.</summary>
    /// <param name="message">The message being offered to the target.</param>
    /// <param name="target">The target to which to post the data.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous send. If the target accepts and consumes the offered element during the call to <see cref="M:System.Threading.Tasks.Dataflow.DataflowBlock.SendAsync``1(System.Threading.Tasks.Dataflow.ITargetBlock{``0},``0)" />, upon return from the call the resulting <see cref="T:System.Threading.Tasks.Task`1" /> will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will return true. If the target declines the offered element during the call, upon return from the call the resulting <see cref="T:System.Threading.Tasks.Task`1" /> will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will return false. If the target postpones the offered element, the element will be buffered until such time that the target consumes or releases it, at which point the task will complete, with its <see cref="P:System.Threading.Tasks.Task`1.Result" /> indicating whether the message was consumed. If the target never attempts to consume or release the message, the returned task will never complete.</returns>   
    let (!>) message (target : ITargetBlock<_>)  = target.AsyncSend(message)
     
    /// <summary>Asynchronously offers a message to the target message block, allowing for postponement.</summary>
    /// <param name="message">The message being offered to the target.</param>
    /// <param name="target">The target to which to post the data.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous send. If the target accepts and consumes the offered element during the call to <see cref="M:System.Threading.Tasks.Dataflow.DataflowBlock.SendAsync``1(System.Threading.Tasks.Dataflow.ITargetBlock{``0},``0)" />, upon return from the call the resulting <see cref="T:System.Threading.Tasks.Task`1" /> will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will return true. If the target declines the offered element during the call, upon return from the call the resulting <see cref="T:System.Threading.Tasks.Task`1" /> will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will return false. If the target postpones the offered element, the element will be buffered until such time that the target consumes or releases it, at which point the task will complete, with its <see cref="P:System.Threading.Tasks.Task`1.Result" /> indicating whether the message was consumed. If the target never attempts to consume or release the message, the returned task will never complete.</returns> 
    let (<!) (target : ITargetBlock<_>) message  = message !> target