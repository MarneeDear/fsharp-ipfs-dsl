module Tests

open FsCheck.Xunit
open Ipfs.DSL

let asyncReturn x = async { return x }

[<Property>]
let ``Declaring a zero program interpreter call produces expression of type Async<unit>`` () =
    typeof<Async<unit>> = (IpfsDSL.run (asyncReturn) (ipfs.Zero())).GetType()

[<Property>]
let ``Interpreting a zero program produces unit`` () =
    () = Async.RunSynchronously (IpfsDSL.run (asyncReturn) (ipfs.Zero()))

[<Property>]
let ``Declaring a trivial program interpreter call wraps arbitrary value in a expression of type Async<t:value>`` (x:string) =
    let program = ipfs.Return(x)
    let expression = IpfsDSL.run (asyncReturn) program
    (asyncReturn x).GetType() = expression.GetType()

[<Property>]
let ``Interpreting a trivial program unwraps arbitrary value`` (x:string) =
    let program = ipfs.Return(x)
    let result = Async.RunSynchronously (IpfsDSL.run (asyncReturn) program)
    x = result

//[<Property>]
// TODO: rework to be fed from a generator of IpfsDSL expressions
// TODO: implement mocking interpreter to solve this test case
let ``Binding a single procedure produces a program that terminates in two steps`` () =
    failwith "Not implemented"