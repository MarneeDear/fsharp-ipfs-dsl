namespace Ipfs.DSL.ExamplePrograms

[<AutoOpen>]
module Programs =
    open FSharpPlus
    open System.IO
    open Ipfs.CoreApi
    open Ipfs.Api
    open Ipfs.DSL
    open Ipfs

    let useDefaults (client:IpfsClient) (continuation:BootstrapDSLResultContext<'a>) =
        BootstrapProcedure(
            Effects.constant (BootstrapDSL.addDefaults client),
            Effects.constant (BootstrapDSLArgs.prepareAddDefaults Cancellation.dontUse),
            continuation) |> liftFreer
       
    type StartContext =
        | AddStream of Stream * fn:string * fo:AddFileOptions

    type EndContext =
        | Finished of IFileSystemNode

    type Ctx =
        | Start of StartContext
        | End of EndContext

    let addStreamArgs : FileSystemDSLArgsEffect<Ctx> =
        fun ctx ->
            match ctx with

            | Start(sctx) ->

                match sctx with
                | AddStream(s, fn, fo) ->
                    FileSystemDSLArgs.prepareAddStream s fn fo Cancellation.dontUse

            // we don't match on the other case to make the program crash fast,
            // the DSL assumes the context to be consistent
        

    let addStream (client:IpfsClient) (continuation:FileSystemDSLResultContext<'a>) =
        FileSystemProcedure(
            Effects.constant (FileSystemDSL.addStream client),
            addStreamArgs,
            continuation) |> liftFreer

    let mutable ctx = monad {
        File.WriteAllBytes("testFile.bin", [|24uy;55uy;22uy;66uy;0uy;99uy;|])
        use fs = File.OpenRead("testFile.bin")
        return! Start(AddStream(fs, "testFile.streamed.bin", AddFileOptions()))
    }

    let finish node = ctx <- End(Finished(node))

    let retrieveCid :FileSystemDSLResultContext<Async<unit>> = 
        fun result ->
            match result with
            | AddStreamResult(afsnode) -> async {
                let! node = afsnode
                return finish node }

            | _ -> async {return ()}

    let client = IpfsClient()
    let r = Async.RunSynchronously (IpfsDSL.run ctx (addStream client retrieveCid))