namespace Bitcoin

open System
open System.Text

open Bitcoin.ByteOrder
open Bitcoin.Messages

module MessageSerialization = 
    let internal serializeHeader (header : MessageHeader) : byte [] = 
        let res = 
            [ BitConverter.GetBytes(hotoleu32 (uint32 header.magic))
              // take at most 12 chars
              ASCIIEncoding.ASCII.GetBytes(header.command) |> Array.truncate 12
              // pad with null byte the rest of the command up until 12 chars
              Array.zeroCreate (max (12 - header.command.Length) 0)
              BitConverter.GetBytes(hotoleu32 header.length)
              BitConverter.GetBytes(hotoleu32 header.checksum) ]
        res |> List.reduce (Array.append)
    
    let internal deserializeHeaderOffset (header : byte []) index : MessageHeader = 
        { magic = LanguagePrimitives.EnumOfValue(letohou32 (BitConverter.ToUInt32(header, index)))
          command = ASCIIEncoding.ASCII.GetChars(header, index + 4, 12) |> Array.filter (fun x -> x <> char 0)
          length = letohou32 (BitConverter.ToUInt32(header, index + 16))
          checksum = letohou32 (BitConverter.ToUInt32(header, index + 20)) }

    let internal deserializeHeader (header : byte []) : MessageHeader = 
        deserializeHeaderOffset header 0

    let serializeMessageAddr (payload : MessageAddr) : byte [] = Array.empty<byte>
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
        | MessageAddrPayload(p) -> serializeMessageAddr p
        | MessageAlertPayload(p) -> serializeMessageAlert p
        | MessageBlockPayload(p) -> serializeMessageBlock p
        | MessageFilterAddPayload(p) -> serializeMessageFilterAdd p
        | MessageFilterClearPayload(p) -> serializeMessageFilterClear p
        | MessageFilterLoadPayload(p) -> serializeMessageFilterLoad p
        | MessageGetAddrPayload(p) -> serializeMessageGetAddr p
        | MessageGetBlocksPayload(p) -> serializeMessageGetBlocks p
        | MessageGetDataPayload(p) -> serializeMessageGetData p
        | MessageGetHeadersPayload(p) -> serializeMessageGetHeaders p
        | MessageHeadersPayload(p) -> serializeMessageHeaders p
        | MessageInvPayload(p) -> serializeMessageInv p
        | MessageMemPoolPayload(p) -> serializeMessageMemPool p
        | MessageMerkleBlockPayload(p) -> serializeMessageMerkleBlock p
        | MessageNotFoundPayload(p) -> serializeMessageNotFound p
        | MessagePingPayload(p) -> serializeMessagePing p
        | MessagePongPayload(p) -> serializeMessagePong p
        | MessageRejectPayload(p) -> serializeMessageReject p
        | MessageTxPayload(p) -> serializeMessageTx p
        | MessageVerackPayload(p) -> serializeMessageVerack p
        | MessageVersionPayload(p) -> serializeMessageVersion p
    
    let serialize (message : BitcoinMessage) : byte [] = 
        Array.append<byte> (serializeHeader message.header) (serializePayload message.payload)
