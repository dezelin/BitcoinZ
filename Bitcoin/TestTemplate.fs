namespace Bitcoin.Tests

open Microsoft.VisualStudio.TestTools.UnitTesting

module TestTemplate = 
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
        member x.yourTestName() = ()
