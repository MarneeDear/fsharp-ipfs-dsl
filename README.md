# IPFS DSL

A declarative embedded language for building compositional programs and protocols over the InterPlanetary File System.

```fsharp
let addFileStreamProgram (client:IpfsClient) (file:Stream) (name:string) (options:AddFileOptions) (receiver:Cid -> Async<unit>) (cont:Cid -> IpfsClientProgram<Async<unit>,'b>)= ipfs {
        let command =
            FileSystemProcedure(
                FileSystemDSL.addStream client,
                FileSystemDSLArgs.prepareAddStream file name options Cancellation.dontUse,
                fun r ->
                    match r with

                    | FileSystemR(AddStreamResult(futureNode)) -> async {
                        let! node = futureNode
                        let cont' = cont node.Id
                        do! receiver node.Id
                        do! IpfsDSL.run (fun x -> x) cont'}

                    | _ -> async { return ()})
        return! liftFree command
    }
```