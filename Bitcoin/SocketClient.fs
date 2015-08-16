namespace Bitcoin

open System
open System.Net
open System.Net.Sockets
open System.Threading
open Bitcoin.Socket

module SocketClient = 
    type RequestCallback = byte [] * int -> unit
    
    type T = 
        { address : IPAddress
          port : int
          socket : Socket.T
          requestCallback : RequestCallback }
    
    let createFromAddress address port requestCallback = 
        { address = address
          port = port
          socket = new Socket.T(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
          requestCallback = requestCallback }
    
    let createFromHostname hostname port requestCallback = 
        let addresses = Dns.GetHostAddresses hostname
        if addresses.Length < 1 then failwithf "Can't resolve hostname: %s" hostname
        createFromAddress addresses.[0] port requestCallback
    
    let asyncConnect client = 
        let socket = client.socket
        let requestCallback = client.requestCallback
        let cancelTokenSource = new CancellationTokenSource()
        let endpoint = new IPEndPoint(client.address, client.port)
        
        let rec loop() = 
            async { 
                try 
                    socket.Poll(-1, SelectMode.SelectRead) |> ignore
                    if not socket.Connected || cancelTokenSource.IsCancellationRequested then return ignore()
                    if socket.Available > 0 then 
                        let buffer = Array.zeroCreate<byte> socket.Available
                        let! read = socket.AsyncReceive buffer
                        requestCallback (buffer, read)
                with _ -> ignore()
                do! loop()
            }
        async { 
            do! socket.AsyncConnect(endpoint)
            Async.Start(loop(), cancellationToken = cancelTokenSource.Token)
            let connectionHandle = 
                { new IDisposable with
                      member x.Dispose() = 
                          cancelTokenSource.Cancel()
                          socket.Close() }
            return connectionHandle
        }
    
    let connected client = client.socket.Connected
    let close client = client.socket.Close()
    let destroy client = client.socket.Dispose()
    
    let asyncSend client buffer = 
        async { 
            let socket = client.socket
            socket.Poll(-1, SelectMode.SelectWrite) |> ignore
            let! sent = socket.AsyncSend(buffer, 0, buffer.Length)
            return sent
        }

