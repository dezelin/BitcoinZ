namespace Bitcoin

open System
open System.Net
open System.Net.Sockets
open System.Threading

open Bitcoin.Socket

module SocketServer = 

    type RequestResultCallback = byte[] -> unit
    type RequestCallback = (byte[] * int * RequestResultCallback) -> unit

    type T = 
        { addr : IPAddress
          port : int 
          requestCallback: RequestCallback }    

    let createFromAddress address port requestCallback = 
        { addr = address
          port = port
          requestCallback = requestCallback }

    
    let createFromHostname hostname port requestCallback = 
        let addresses = Dns.GetHostAddresses(hostname)
        if addresses.Length < 1 then failwithf "Can't resolve hostname: %s" hostname
        createFromAddress addresses.[0]  port  requestCallback
    
    let start server = 
        let endpoint = IPEndPoint(server.addr, server.port)
        let cancelTokenSource = new CancellationTokenSource()
        let listener = new Socket.T(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        listener.Bind(endpoint)
        listener.Listen(int SocketOptionName.MaxConnections)

        let requestCallback = server.requestCallback

        let rec loop() = 
            async { 
                try
                    let! socket = listener.AsyncAccept()
                    socket.Poll(-1, SelectMode.SelectRead) |> ignore
                    if cancelTokenSource.IsCancellationRequested then
                        return ignore()

                    if socket.Connected && socket.Available > 0 then 
                        let buffer = Array.zeroCreate<byte> socket.Available
                        let! read = socket.AsyncReceive buffer
                        requestCallback(buffer, read, fun buffer -> socket.Send buffer |> ignore)

                with _ -> ignore()

                do! loop()
            }


        Async.Start(loop(), cancellationToken = cancelTokenSource.Token)
        
        { // Return disposable handle
            new IDisposable with
                member x.Dispose() = 
                    cancelTokenSource.Cancel()
                    listener.Close() 
        }
