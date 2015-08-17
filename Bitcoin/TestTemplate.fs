namespace Bitcoin.Tests

open NUnit.Framework
open FsUnit

module TestTemplate = 
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
        member x.yourTestName() = ()
