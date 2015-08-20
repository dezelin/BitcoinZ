namespace Bitcoin

open Bitcoin.ByteOrder
open Bitcoin.Messages
open System
open System.Text

module MessageSerialization = 
    let internal serializeu16 (x : uint16) = BitConverter.GetBytes(hotoleu16 x)
    let internal serializeu32 (x : uint32) = BitConverter.GetBytes(hotoleu32 x)
    let internal serializeu64 (x : uint64) = BitConverter.GetBytes(hotoleu64 x)
    let internal serializeCharArray (array : char []) count = 
        Array.zeroCreate (max (count - array.Length) 0) 
        |> Array.append (ASCIIEncoding.ASCII.GetBytes(array) |> Array.toSeq |> Seq.truncate count |> Seq.toArray)
    let internal deserializeu16 (array : byte []) index = letohou16 (BitConverter.ToUInt16(array, index))
    let internal deserializeu32 (array : byte []) index = letohou32 (BitConverter.ToUInt32(array, index))
    let internal deserializeu64 (array : byte []) index = letohou64 (BitConverter.ToUInt64(array, index))
    let internal deserializeCharArray (array : byte []) index count = ASCIIEncoding.ASCII.GetChars(array, index, count)
    let internal deserializeCharArrayFilterNull (array : byte []) index count = 
        ASCIIEncoding.ASCII.GetChars(array, index, count) |> Array.filter (fun x -> x <> char 0)
    
    let internal serializeVarInt (x : VarInt) : byte [] = 
        match x with
        | VarIntByte v -> [| v.value |]
        | VarIntShort v -> Array.append [| LanguagePrimitives.EnumToValue v.marker |] (serializeu16 v.value)
        | VarInt32 v -> Array.append [| LanguagePrimitives.EnumToValue v.marker |] (serializeu32 v.value)
        | VarInt64 v -> Array.append [| LanguagePrimitives.EnumToValue v.marker |] (serializeu64 v.value)
    
    let internal deserializeVarInt (array : byte []) index : VarInt = 
        let marker = LanguagePrimitives.EnumOfValue array.[index]
        match marker with
        | VarIntMarker.VarIntMarkerShort -> 
            VarIntShort { marker = marker
                          value = deserializeu16 array (index + 1) }
        | VarIntMarker.VarIntMarkerInt32 -> 
            VarInt32 { marker = marker
                       value = deserializeu32 array (index + 1) }
        | VarIntMarker.VarIntMarkerInt64 -> 
            VarInt64 { marker = marker
                       value = deserializeu64 array (index + 1) }
        | _ -> VarIntByte { value = array.[index] }
    
    let internal serializeNetAddr (addr : NetAddr) : byte [] = 
        let list = 
            [ serializeu32 addr.time
              serializeu64 addr.services
              addr.ipv6_4 |> Array.toSeq |> Seq.truncate 16 |> Seq.toArray
              serializeu16 addr.port ]
        list |> List.reduce (Array.append)
    
    let internal deserializeNetAddr (array : byte []) index : NetAddr = 
        { time = deserializeu32 array index
          services = deserializeu64 array (index + 4)
          ipv6_4 = array.[index + 12..index + 12 + 15] // slice 16 bytes starting from (index + 12)
          port = deserializeu16 array (index + 28) }
    
    let internal serializeNetAddrArray (addrs : NetAddr []) : byte [] = 
        Array.fold (fun array addr -> Array.append array (serializeNetAddr addr)) [||] addrs
    
    let internal deserializeNetAddrArray (array : byte []) index count = 
        let rec loop res index count = 
            if count = 0 then res
            // Size of NetAddr structure in bytes is NetAddrPayloadSize
            else loop ((deserializeNetAddr array index) :: res) (index + NetAddrPayloadSize) (count - 1)
        loop [] index count
        |> List.rev
        |> List.toArray
    
    let internal serializeHeader (header : MessageHeader) : byte [] = 
        let list = 
            [ serializeu32 (uint32 header.magic)
              serializeCharArray header.command 12
              serializeu32 (header.length)
              serializeu32 (header.checksum) ]
        list |> List.reduce (Array.append)
    
    let internal deserializeHeader (header : byte []) index : MessageHeader = 
        { magic = LanguagePrimitives.EnumOfValue(deserializeu32 header index)
          command = deserializeCharArrayFilterNull header (index + 4) 12
          length = deserializeu32 header (index + 16)
          checksum = deserializeu32 header (index + 20) }
    
    let serializeMessageAddr (payload : MessageAddr) : byte [] = 
        let list = 
            [ serializeVarInt payload.count
              serializeNetAddrArray payload.addrList ]
        list |> List.reduce (Array.append)
    
    //let deserializeMessageAddr (payload : byte []) : MessageAddr = []
    let serializeMessageAlert (payload : MessageAlert) : byte [] = Array.empty<byte>
    let serializeMessageBlock (payload : MessageBlock) : byte [] = Array.empty<byte>
    let serializeMessageFilterAdd (payload : MessageFilterAdd) : byte [] = Array.empty<byte>
    let serializeMessageFilterClear (payload : MessageFilterClear) : byte [] = Array.empty<byte>
    let serializeMessageFilterLoad (payload : MessageFilterLoad) : byte [] = Array.empty<byte>
    let serializeMessageGetAddr (payload : MessageGetAddr) : byte [] = Array.empty<byte>
    let serializeMessageGetBlocks (payload : MessageGetBlocks) : byte [] = Array.empty<byte>
    let serializeMessageGetData (payload : MessageGetData) : byte [] = Array.empty<byte>
    let serializeMessageGetHeaders (payload : MessageGetHeaders) : byte [] = Array.empty<byte>
    let serializeMessageHeaders (payload : MessageHeaders) : byte [] = Array.empty<byte>
    let serializeMessageInv (payload : MessageInv) : byte [] = Array.empty<byte>
    let serializeMessageMemPool (payload : MessageMemPool) : byte [] = Array.empty<byte>
    let serializeMessageMerkleBlock (payload : MessageMerkleBlock) : byte [] = Array.empty<byte>
    let serializeMessageNotFound (payload : MessageNotFound) : byte [] = Array.empty<byte>
    let serializeMessagePing (payload : MessagePing) : byte [] = Array.empty<byte>
    let serializeMessagePong (payload : MessagePong) : byte [] = Array.empty<byte>
    let serializeMessageReject (payload : MessageReject) : byte [] = Array.empty<byte>
    let serializeMessageTx (payload : MessageTx) : byte [] = Array.empty<byte>
    let serializeMessageVerack (payload : MessageVerack) : byte [] = Array.empty<byte>
    let serializeMessageVersion (payload : MessageVersion) : byte [] = Array.empty<byte>
    
    let private serializePayload (payload : MessagePayload) : byte [] = 
        match payload with
        | MessageAddr message -> serializeMessageAddr message
        | MessageAlert message -> serializeMessageAlert message
        | MessageBlock message -> serializeMessageBlock message
        | MessageFilterAdd message -> serializeMessageFilterAdd message
        | MessageFilterClear message -> serializeMessageFilterClear message
        | MessageFilterLoad message -> serializeMessageFilterLoad message
        | MessageGetAddr message -> serializeMessageGetAddr message
        | MessageGetBlocks message -> serializeMessageGetBlocks message
        | MessageGetData message -> serializeMessageGetData message
        | MessageGetHeaders message -> serializeMessageGetHeaders message
        | MessageHeaders message -> serializeMessageHeaders message
        | MessageInv message -> serializeMessageInv message
        | MessageMemPool message -> serializeMessageMemPool message
        | MessageMerkleBlock message -> serializeMessageMerkleBlock message
        | MessageNotFound message -> serializeMessageNotFound message
        | MessagePing message -> serializeMessagePing message
        | MessagePong message -> serializeMessagePong message
        | MessageReject message -> serializeMessageReject message
        | MessageTx message -> serializeMessageTx message
        | MessageVerack message -> serializeMessageVerack message
        | MessageVersion message -> serializeMessageVersion message
    
    let serialize (message : BitcoinMessage) : byte [] = 
        Array.append<byte> (serializeHeader message.header) (serializePayload message.payload)
