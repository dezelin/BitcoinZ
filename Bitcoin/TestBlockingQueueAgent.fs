namespace Bitcoin.Tests

open NUnit.Framework
open FsUnit
open Bitcoin

module TestBlockingQueueAgent = 
    [<TestFixture>]
    type T() = 
        
        let testQueueOfSize queueSize messagesCount = 
            let agent = BlockingQueueAgent.T<int>(queueSize)
            
            let writer sequence = 
                async { 
                    sequence
                    |> Seq.iter (fun x -> agent.AsyncAdd x |> Async.RunSynchronously)
                    |> ignore
                }
            
            let reader count = 
                async { 
                    let rec loop acc i = 
                        if i = count then List.rev acc
                        else 
                            let x = agent.AsyncGet() |> Async.RunSynchronously
                            loop (x :: acc) (i + 1)
                    return loop [] 0
                }
            
            let testSequence = 
                seq { 
                    for i in 1..messagesCount -> i
                }
            
            Async.Start(writer testSequence)
            let queueSequence = 
                Seq.length testSequence
                |> reader
                |> Async.RunSynchronously
            queueSequence.Length |> should equal (Seq.length testSequence)
            Seq.iter2 (fun x y -> x |> should equal y) (queueSequence |> List.ofSeq) (testSequence)
        
        [<TestFixtureSetUp>]
        static member setup() = ()
        
        [<TestFixtureTearDown>]
        static member clean() = ()
        
        [<SetUp>]
        member x.tearUp() = ()
        
        [<TearDown>]
        member x.tearDown() = ()
        
        [<Test>]
        member x.``Test queue with reader/writer async tasks (queueSize = 1, messages = 1000)``() = 
            testQueueOfSize 1 1000
        
        [<Test>]
        member x.``Test queue with reader/writer async tasks (queueSize = 16, messages = 1000)``() = 
            testQueueOfSize 16 1000
        
        [<Test>]
        member x.``Test queue with reader/writer async tasks (queueSize = 128, messages = 1000)``() = 
            testQueueOfSize 128 1000
        
        [<Test>]
        member x.``Test queue with reader/writer async tasks (queueSize = 1000, messages = 1000)``() = 
            testQueueOfSize 1000 1000

        [<Test>]
        member x.``Test queue with reader/writer async tasks (queueSize = 1001, messages = 1000)``() = 
            testQueueOfSize 1001 1000