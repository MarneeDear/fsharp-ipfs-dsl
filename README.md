# IPFS DSL

A declarative embedded language for building compositional programs and protocols over the InterPlanetary File System.

## Language example (docs to come)

The following lazy computation expression yields an AST that captures the notion of adding a in-memory stream to the local IPFS node, which can be reasoned about and transformed.

```fsharp
let addFileStreamProgram (client:IpfsClient) (file:Stream) (name:string) (options:AddFileOptions) (receiver:Cid -> Async<unit>) (cont:Cid -> IpfsClientProgram<Async<unit>,'b>)= ipfs {
        let command =
            // which sub-DSL?
            FileSystemProcedure(
                // where to invoke?
                FileSystemDSL.addStream client,
                // prepare the arguments
                FileSystemDSLArgs.prepareAddStream file name options Cancellation.dontUse,
                // and decide what to do with results
                fun r ->
                    match r with

                    // no escaping from the async monad
                    | FileSystemR(AddStreamResult(futureNode)) -> async {
                        let! node = futureNode
                        let cont' = cont node.Id
                        // use the receiver to implement shared memory model
                        do! receiver node.Id
                        // optionally, invoke another program
                        do! IpfsDSL.run (fun x -> x) cont'}

                    | _ -> async { return ()})
        // don't forget to lift the command into a program
        return! liftFree command
    }
```

To run this abstract syntax tree, you would

```fsharp
/// captures the result
let monitor cid = async { do printfn "%A" cid }

/// program that follows
let continuation = fun cid -> someOtherProgram cid

/// passing parameters to close the free terms
let programInstance = addFileStreamProgram client file name fileoptions monitor continuation

/// invoke the interpreter
do IpfsDSL.run monitor programInstance
```

### What's in the box

Main DSL | Sub DSL | Args | Result
---------|---------|------|-------
BitswapProcedure | BitswapDSL | BitswapDSLArgs | BitswapDSLResult
BlockProcedure | BlockDSL | BlockDSLArgs | BlockDSLResult
BootstrapProcedure | BootstrapDSL | BootstrapDSLArgs | BootstrapDSLResult
ConfigProcedure | ConfigDSL | ConfigDSLArgs | ConfigDSLResult
DagProcedure | DagDSL<'Out> | DagDSLArgs | DagDSLResult<'Out>
DhtProcedure | DhtDSL | DhtDSLArgs | DhtDSLResult
DnsProcedure | DnsDSL | DnsDSLArgs | DnsDSLResult
FileSystemProcedure | FileSystemDSL | FileSystemDSLArgs | FileSystemDSLResult
GenericProcedure | GenericDSL | GenericDSLArgs | GenericDSLResult
KeyProcedure | KeyDSL | KeyDSLArgs | KeyDSLResult
NameProcedure | NameDSL | NameDSLArgs | NameDSLResult
ObjectProcedure | ObjectDSL | ObjectDSLArgs | ObjectDSLResult
PinProcedure | PinDSL | PinDSLArgs | PinDSLResult
PubSubProcedure | PubSubDSL | PubSubDSLArgs | PubSubDSLResult
SwarmProcedure | SwarmDSL | SwarmDSLArgs | SwarmDSLResult

All of these correspond to calling the lower-level IPFS API methods. Some names may be changed slightly.


#### Typeparams and free

> **'Out** - DagDSL includes a method that deserializes a JSON object to a native .NET object. That method gets parameterized with the 'Out parameter even if you don't use the DagDSL in your program because it's subsumed to the main IpfsDSL. If do use it, remember that you can **flatMapR** to change the return type.

The other and more important typeparam is **'Cont**, it is used to capture the type of the next step of your program. Examine the free monad construction for the **IpfsDSL**.

```fsharp
/// the "free monad", also called a program, this little recursive structure models all possible
    /// execution scenarios of using the IPFS API with the embedded langauge, more precisely,
    /// the following two can occur:
    /// (1) the program you write never terminates, and gets stuck in an infinite recursive loop
    /// (2) the program iterates recursively until it reaches a value, which it returns and terminates
    type IpfsClientProgram<'Cont,'R> =
        /// Free, the recursive step,
        /// a statement in the embedded language about the next step in the program
        | Free of IpfsDSL<IpfsClientProgram<'Cont,'R>,'R>

        /// Return, the final state,
        /// a pure value returned by the program
        | Return of 'Cont
```

## Acknowledgements

Built over the [net-ipfs-api](https://github.com/richardschneider/net-ipfs-api) and [net-ipfs-core](https://github.com/richardschneider/net-ipfs-core) by Richard Schneider. Many thanks!

## MIT License

Copyright © 2018 ČlovëekProjeqt [(cloveekprojeqt@gmail.com)](mailto:cloveekprojeqt@gmail.com) 