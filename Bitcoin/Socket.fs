namespace Bitcoin

open System.Net
open System.Net.Sockets

module Socket =

    type internal Socket with
        
        member x.AsyncAccept() = Async.FromBeginEnd(x.BeginAccept, x.EndAccept)

        member x.AsyncConnect(endpoint: IPEndPoint) = 
            Async.FromBeginEnd
                (endpoint, 
                 (fun (endpoint: IPEndPoint, callback, state) -> 
                 x.BeginConnect(endpoint, callback, state)), x.EndConnect)
        
        member x.AsyncReceive(buffer : byte [], ?offset, ?count) = 
            let offset = defaultArg offset 0
            let count = defaultArg count buffer.Length
            Async.FromBeginEnd
                (buffer, offset, count, 
                 (fun (buffer, offset, count, callback, state) -> 
                 x.BeginReceive(buffer, offset, count, SocketFlags.None, callback, state)), x.EndReceive)
        
        member x.AsyncSend(buffer : byte [], ?offset, ?count) = 
            let offset = defaultArg offset 0
            let count = defaultArg count buffer.Length
            Async.FromBeginEnd
                (buffer, offset, count, 
                 (fun (buffer, offset, count, callback, state) -> 
                 x.BeginSend(buffer, offset, count, SocketFlags.None, callback, state)), x.EndSend)

    type T = Socket
