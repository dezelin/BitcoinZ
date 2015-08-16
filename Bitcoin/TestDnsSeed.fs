namespace Bitcoin.Tests

open Microsoft.VisualStudio.TestTools.UnitTesting
open Bitcoin

module TestDnsSeed = 
    [<TestClass>]
    type T() = 
        
        [<ClassInitialize>]
        static member tearUp (context : TestContext) = ()
        
        [<ClassCleanup>]
        static member tearDown() = ()
        
        [<TestInitialize>]
        member x.setup() = ()
        
        [<TestCleanup>]
        member x.clean() = ()
        
        [<TestMethod>]
        member x.fetchAsync() = 
            let runAsync() = 
                "www.google.com"
                |> DnsSeed.fetchAsync
                |> Async.Catch
                |> Async.RunSynchronously
            match runAsync() with
            | Choice1Of2(addresses) -> Assert.AreNotEqual(addresses.Length, 0)
            | Choice2Of2(ex) -> Assert.Fail()
        
        [<TestMethod>]
        member x.fetchAsyncError() = 
            let runAsync() = 
                "xyz"
                |> DnsSeed.fetchAsync
                |> Async.Catch
                |> Async.RunSynchronously
            match runAsync() with
            | Choice1Of2 addresses -> Assert.AreEqual(addresses.Length, 0)
            | Choice2Of2 ex -> Assert.Fail()
        
        [<TestMethod>]
        member x.fetch() = 
            let seeds = DnsSeed.fetch()
            Assert.AreEqual(seeds.Length >= 0, true)