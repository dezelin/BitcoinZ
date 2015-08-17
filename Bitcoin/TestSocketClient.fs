namespace Bitcoin.Tests

open System.Net
open System.Net.Sockets
open NUnit.Framework
open FsUnit
open Bitcoin

module TestSocketClient = 
    [<TestFixture>]
    type T() = 
        
        static let localIp() = 
            let zeroIp = [| 0uy; 0uy; 0uy; 0uy |]
            let addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            if addresses.Length = 0 then zeroIp
            else 
                try 
                    let localIp = 
                        Seq.find (fun (y : IPAddress) -> y.AddressFamily = AddressFamily.InterNetwork) addresses
                    localIp.GetAddressBytes()
                with _ -> zeroIp
        
        static let address = localIp() //[| 127uy; 0uy; 0uy; 0uy |]
        static let port = 5678
        static let server : SocketServer.T = 
            SocketServer.createFromAddress (new IPAddress(address)) port 
                (fun (buffer, read, callback) -> callback (buffer))
        
        [<TestFixtureSetUp>]
        static member setup() = 
            SocketServer.start server
            |> Async.RunSynchronously
            |> ignore
        
        [<TestFixtureTearDown>]
        static member clean() = ()
        
        [<SetUp>]
        member x.tearUp() = ()
        
        [<TearDown>]
        member x.tearDown() = ()
        
        [<Test>]
        member x.initialize() = 
            let data = "data"B
            
            let client = 
                SocketClient.createFromAddress (IPAddress(address)) port (fun (buffer, read) -> 
                    let bufferStr = System.Text.Encoding.ASCII.GetString buffer
                    let dataStr = System.Text.Encoding.ASCII.GetString data
                    bufferStr |> should equal dataStr)
            
            let connection = SocketClient.asyncConnect client |> Async.RunSynchronously
            data
            |> SocketClient.asyncSend client
            |> Async.RunSynchronously
            |> ignore
            Async.Sleep 2000
            |> Async.RunSynchronously
            |> ignore
            connection.Dispose()
