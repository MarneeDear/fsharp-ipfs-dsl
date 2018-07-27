namespace Ipfs.DSL

[<AutoOpen>]
module Effects = 

    type Effect<'Context, 'Produces> = 'Context -> 'Produces

    type EffectBuilder() =
        member this.Return(x) : Effect<'Context, 'Produces>     = fun _ -> x
        member this.ReturnFrom (x :Effect<'Context, 'Produces>) = x
        member this.Zero() : Effect<'Context, unit>             = ignore
        member this.Bind(f :Effect<'Context, 'T>, g: 'T -> Effect<'Context, 'S>) : Effect<'Context, 'S> =
            fun c -> g (f c) c

    let effect = new EffectBuilder()

    let contextFor<'Context> () : Effect<'Context, 'Context> = id

    let runEffect ctx (eff: Effect<'Context, 'Produces>) = eff ctx

    let constant r = effect { return r }

