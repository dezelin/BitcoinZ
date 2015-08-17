namespace Bitcoin.Tests

open Bitcoin
open NUnit.Framework
open FsUnit

module TestDnsSeed = 
    [<TestFixture>]
    type T() = 
        
        [<TestFixtureSetUp>]
        static member setup() = ()
        
        [<TestFixtureTearDown>]
        static member clean() = ()
        
        [<SetUp>]
        member x.tearUp() = ()
        
        [<TearDown>]
        member x.tearDown() = ()
        
        [<Test>]
        member x.fetchAsync() = 
            let runAsync() = 
                "www.google.com"
                |> DnsSeed.fetchAsync
                |> Async.Catch
                |> Async.RunSynchronously
            match runAsync() with
            | Choice1Of2(addresses) -> addresses.Length |> should not' (equal 0)
            | Choice2Of2(ex) -> Assert.Fail()
        
        [<Test>]
        member x.fetchAsyncError() = 
            let runAsync() = 
                "xyz"
                |> DnsSeed.fetchAsync
                |> Async.Catch
                |> Async.RunSynchronously
            match runAsync() with
            | Choice1Of2 addresses -> addresses.Length |> should equal 0
            | Choice2Of2 ex -> Assert.Fail()
        
        [<Test>]
        member x.fetch() = 
            let seeds = DnsSeed.fetch()
            seeds.Length |> should be (greaterThanOrEqualTo 0)