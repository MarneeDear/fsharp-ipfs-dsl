namespace Ipfs.DSL

[<AutoOpen>]
module IpfsDSL =
    open SubDSLs
    open Effects  

    type IpfsDSL<'Cont,'Ctx,'R> =
        | BitswapProcedure    of pexpr:BitswapDSLEffect<'Ctx>    * pargs:BitswapDSLArgsEffect<'Ctx>    * cback:BitswapDSLResultContext<'Cont>
        | BlockProcedure      of pexpr:BlockDSLEffect<'Ctx>      * pargs:BlockDSLArgsEffect<'Ctx>      * cback:BlockDSLResultContext<'Cont>
        | BootstrapProcedure  of pexpr:BootstrapDSLEffect<'Ctx>  * pargs:BootstrapDSLArgsEffect<'Ctx>  * cback:BootstrapDSLResultContext<'Cont>
        | ConfigProcedure     of pexpr:ConfigDSLEffect<'Ctx>     * pargs:ConfigDSLArgsEffect<'Ctx>     * cback:ConfigDSLResultContext<'Cont>
        | DagProcedure        of pexpr:DagDSLEffect<'Ctx,'R>     * pargs:DagDSLArgsEffect<'Ctx>        * cback:DagDSLResultContext<'Cont,'R>
        | DhtProcedure        of pexpr:DhtDSLEffect<'Ctx>        * pargs:DhtDSLArgsEffect<'Ctx>        * cback:DhtDSLResultContext<'Cont>
        | FileSystemProcedure of pexpr:FileSystemDSLEffect<'Ctx> * pargs:FileSystemDSLArgsEffect<'Ctx> * cback:FileSystemDSLResultContext<'Cont>
        | GenericProcedure    of pexpr:GenericDSLEffect<'Ctx>    * pargs:GenericDSLArgsEffect<'Ctx>    * cback:GenericDSLResultContext<'Cont>
        | KeyProcedure        of pexpr:KeyDSLEffect<'Ctx>        * pargs:KeyDSLArgsEffect<'Ctx>        * cback:KeyDSLResultContext<'Cont>
        | NameProcedure       of pexpr:NameDSLEffect<'Ctx>       * pargs:NameDSLArgsEffect<'Ctx>       * cback:NameDSLResultContext<'Cont>
        | ObjectProcedure     of pexpr:ObjectDSLEffect<'Ctx>     * pargs:ObjectDSLArgsEffect<'Ctx>     * cback:ObjectDSLResultContext<'Cont>
        | PinProcedure        of pexpr:PinDSLEffect<'Ctx>        * pargs:PinDSLArgsEffect<'Ctx>        * cback:PinDSLResultContext<'Cont>
        | PubSubProcedure     of pexpr:PubSubDSLEffect<'Ctx>     * pargs:PubSubDSLArgsEffect<'Ctx>     * cback:PubSubDSLResultContext<'Cont>
        | SwarmProcedure      of pexpr:SwarmDSLEffect<'Ctx>      * pargs:SwarmDSLArgsEffect<'Ctx>      * cback:SwarmDSLResultContext<'Cont>

    let flatMapR : ('a -> 'b) -> (DagDSLResultContext<'Cont,'a> -> DagDSLResultContext<'Cont,'b>) -> (IpfsDSL<'Cont,'ctx,'a>) -> (IpfsDSL<'Cont,'ctx,'b>) =
        fun f alpha ->
            function
            | BitswapProcedure(proc, args, g)    -> BitswapProcedure(proc, args, g)
            | BlockProcedure(proc, args, g)      -> BlockProcedure(proc, args, g)
            | BootstrapProcedure(proc, args, g)  -> BootstrapProcedure(proc, args, g)
            | ConfigProcedure(proc, args, g)     -> ConfigProcedure(proc, args, g)

            | DagProcedure(proc, args, g) ->

                // apply f to the DSL command
                let proc' = effect {
                    let! r = proc
                    return DagDSL.flatMap f r
                }

                // apply alpha to the result context
                let g' = DagDSL.flatMapResultCtx alpha g

                // return transformed
                DagProcedure(proc', args, g')

            | DhtProcedure(proc, args, g)        -> DhtProcedure(proc, args, g)
            | FileSystemProcedure(proc, args, g) -> FileSystemProcedure(proc, args, g)
            | GenericProcedure(proc, args, g)    -> GenericProcedure(proc, args, g)
            | KeyProcedure(proc, args, g)        -> KeyProcedure(proc, args, g)
            | NameProcedure(proc, args, g)       -> NameProcedure(proc, args, g)
            | ObjectProcedure(proc, args, g)     -> ObjectProcedure(proc, args, g)
            | PinProcedure(proc, args, g)        -> PinProcedure(proc, args, g)
            | PubSubProcedure(proc, args, g)     -> PubSubProcedure(proc, args, g)
            | SwarmProcedure(proc, args, g)      -> SwarmProcedure(proc, args, g)

    let flatMap : ('a -> 'b) -> (IpfsDSL<'a,'ctx,'R>) -> (IpfsDSL<'b,'ctx,'R>) =
        fun f ->
            function
            | BitswapProcedure(proc, args, g)    -> BitswapProcedure(proc, args, g >> f)
            | BlockProcedure(proc, args, g)      -> BlockProcedure(proc, args, g >> f)
            | BootstrapProcedure(proc, args, g)  -> BootstrapProcedure(proc, args, g >> f)
            | ConfigProcedure(proc, args, g)     -> ConfigProcedure(proc, args, g >> f)
            | DagProcedure(proc, args, g)        -> DagProcedure(proc, args, g >> f)
            | DhtProcedure(proc, args, g)        -> DhtProcedure(proc, args, g >> f)
            | FileSystemProcedure(proc, args, g) -> FileSystemProcedure(proc, args, g >> f)
            | GenericProcedure(proc, args, g)    -> GenericProcedure(proc, args, g >> f)
            | KeyProcedure(proc, args, g)        -> KeyProcedure(proc, args, g >> f)
            | NameProcedure(proc, args, g)       -> NameProcedure(proc, args, g >> f)
            | ObjectProcedure(proc, args, g)     -> ObjectProcedure(proc, args, g >> f)
            | PinProcedure(proc, args, g)        -> PinProcedure(proc, args, g >> f)
            | PubSubProcedure(proc, args, g)     -> PubSubProcedure(proc, args, g >> f)
            | SwarmProcedure(proc, args, g)      -> SwarmProcedure(proc, args, g >> f)
        
    type IpfsDSLArgs =
        private
        | BitswapA    of BitswapDSLArgs
        | BlockA      of BlockDSLArgs
        | BootstrapA  of BootstrapDSLArgs
        | ConfigA     of ConfigDSLArgs
        | DagA        of DagDSLArgs
        | DhtA        of DhtDSLArgs
        | FileSystemA of FileSystemDSLArgs
        | GenericA    of GenericDSLArgs
        | KeyA        of KeyDSLArgs
        | NameA       of NameDSLArgs
        | ObjectA     of ObjectDSLArgs
        | PinA        of PinDSLArgs
        | PubSubA     of PubSubDSLArgs
        | SwarmA      of SwarmDSLArgs
        static member bind (args:BitswapDSLArgs) = BitswapA(args)
        static member bind (args:BlockDSLArgs) = BlockA(args)
        static member bind (args:BootstrapDSLArgs) = BootstrapA(args)
        static member bind (args:ConfigDSLArgs) = ConfigA(args)
        static member bind (args:DagDSLArgs) = DagA(args)
        static member bind (args:DhtDSLArgs) = DhtA(args)
        static member bind (args:FileSystemDSLArgs) = FileSystemA(args)
        static member bind (args:GenericDSLArgs) = GenericA(args)
        static member bind (args:KeyDSLArgs) = KeyA(args)
        static member bind (args:NameDSLArgs) = NameA(args)
        static member bind (args:ObjectDSLArgs) = ObjectA(args)
        static member bind (args:PinDSLArgs) = PinA(args)
        static member bind (args:PubSubDSLArgs) = PubSubA(args)
        static member bind (args:SwarmDSLArgs) = SwarmA(args)
      
    type IpfsClientProgram<'Next, 'Context, 'Produces, 'DagOut> =

        /// an expression in the embedded language about the next step in the program
        | Step of program:IpfsDSL<IpfsClientProgram<'Next,'Context,'Produces,'DagOut>,'Context,'DagOut>
    
        /// a branching step depending on the result of the effectful computation
        | Branch of visible:Effect<'Context,'Produces> * program:('Produces -> IpfsDSL<IpfsClientProgram<'Next,'Context,'Produces,'DagOut>,'Context,'DagOut>)

        /// the final step, produces a value or a new program
        | Return of value:'Next

    /// binds fsharp and embedded language expressions into programs of the embedded language
    let rec bindFreer : ('a -> IpfsClientProgram<'b,'ctx,'p,'o>) -> IpfsClientProgram<'a,'ctx,'p,'o> -> IpfsClientProgram<'b,'ctx,'p,'o> =
        fun f ->
            function
            | Step(clientDSL) -> Step(flatMap (bindFreer f) clientDSL)
            | Branch(eff, program) -> Branch(eff, fun x -> flatMap (bindFreer f) (program x))
            | Return(value) -> f value

    /// lifts expressions of the embedded language into programs of the embedded language
    let liftFreer : IpfsDSL<'a,'ctx,'r> -> IpfsClientProgram<'a,'ctx,'p,'r> =
        fun statement -> Step (flatMap Return statement)

    type IpfsClientProgramBuilder() =
        member this.Return              x = Return x
        member this.ReturnFrom          x = x
        member this.Zero               () = Return ()
        member this.Bind          (ma, f) = bindFreer f ma
        member this.Delay (f: unit -> 'a) = f
        member this.Run               (f) = f()

    let ipfs = IpfsClientProgramBuilder()

    let rec run : ('ctx) -> (IpfsClientProgram<'a, 'ctx, 'p, 'o>) -> Async<'a> =
        fun ctx freer -> async {
            match freer with

            | Return(a) -> return a

            | Branch(eff, conditionalProgram) -> 
                let branchResult = runEffect ctx eff
                let program = conditionalProgram branchResult
                return! run ctx (Step program)

            | Step(BitswapProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = BitswapDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(BlockProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = BlockDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(BootstrapProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = BootstrapDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(ConfigProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = ConfigDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(DagProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = DagDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(DhtProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = DhtDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(FileSystemProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = FileSystemDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(GenericProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = GenericDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(KeyProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = KeyDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(NameProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = NameDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(ObjectProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = ObjectDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram
                
            | Step(PinProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = PinDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(PubSubProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = PubSubDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram

            | Step(SwarmProcedure(p, a, cont)) ->
                let p' = runEffect ctx p
                let a' = runEffect ctx a
                let! partialResult = SwarmDSL.interpret p' a'
                let nextProgram = cont partialResult
                return! run ctx nextProgram
        }