namespace Bitcoin.Tests

open Bitcoin.MessageSerialization
open Bitcoin.Messages
open FsUnit
open NUnit.Framework
open System
open System.Text

module TestMessageSerialization = 
    [<TestFixture>]
    type T() = 
        let lei16 : int16 = 0x0201s
        let lei32 : int32 = 0x04030201l
        let lei64 : int64 = 0x0807060504030201L
        let leu16 : uint16 = 0x0201us
        let leu32 : uint32 = 0x04030201ul
        let leu64 : uint64 = 0x0807060504030201uL
        let bei16 : int16 = 0x0102s
        let bei32 : int32 = 0x01020304l
        let bei64 : int64 = 0x0102030405060708L
        let beu16 : uint16 = 0x0102us
        let beu32 : uint32 = 0x01020304ul
        let beu64 : uint64 = 0x0102030405060708uL
        
        let createMessageHeader magic command length checksum = 
            { magic = magic
              command = command
              length = length
              checksum = checksum }
        
        let iterMessageHeaders checker = 
            Enum.GetValues(typeof<MagicNumber>)
            |> Seq.cast
            |> Seq.iter (fun magic -> 
                   let commands = 
                       seq { 
                           for i in 0..20 -> String.replicate i "1"
                       }
                   commands |> Seq.iter (fun command -> 
                                   let header = createMessageHeader magic (command.ToCharArray()) 1u 2u
                                   let serialized = serializeHeader header
                                   checker (header, serialized)))
        
        [<TestFixtureSetUp>]
        static member setup() = ()
        
        [<TestFixtureTearDown>]
        static member clean() = ()
        
        [<SetUp>]
        member x.tearUp() = ()
        
        [<TearDown>]
        member x.tearDown() = ()
        
        [<Test>]
        member x.``serialize/deserialize u16``() = 
            let x = serializeu16 leu16
            x.[0] |> should equal ((leu16 <<< 8) >>> 8)
            x.[1] |> should equal (leu16 >>> 8)
            leu16 |> should equal (deserializeu16 x 0)
        
        [<Test>]
        member x.``serialize/deserialize u32``() = 
            let x = serializeu32 leu32
            x.[0] |> should equal ((leu32 <<< 24) >>> 24)
            x.[1] |> should equal ((leu32 <<< 16) >>> 24)
            x.[2] |> should equal ((leu32 <<< 8) >>> 24)
            x.[3] |> should equal (leu32 >>> 24)
            leu32 |> should equal (deserializeu32 x 0)
        
        [<Test>]
        member x.``serialize/deserialize u64``() = 
            let x = serializeu64 leu64
            x.[0] |> should equal ((leu64 <<< 56) >>> 56)
            x.[1] |> should equal ((leu64 <<< 48) >>> 56)
            x.[2] |> should equal ((leu64 <<< 40) >>> 56)
            x.[3] |> should equal ((leu64 <<< 32) >>> 56)
            x.[4] |> should equal ((leu64 <<< 24) >>> 56)
            x.[5] |> should equal ((leu64 <<< 16) >>> 56)
            x.[6] |> should equal ((leu64 <<< 8) >>> 56)
            x.[7] |> should equal (leu64 >>> 56)
            leu64 |> should equal (deserializeu64 x 0)
        
        [<Test>]
        member x.``serialize/deserialize char array``() = 
            let str = "serializeCharArray"
            let x = serializeCharArray (str.ToCharArray()) (str.Length)
            let y = ASCIIEncoding.ASCII.GetString(x)
            y |> should equal str
            let z = deserializeCharArray x 0 x.Length
            z |> should equal str
        
        [<Test>]
        member x.``serialize/deserialize char array with filtered null bytes``() = 
            let str = "serializeCharArray"
            let arrayWithNulls = Array.zeroCreate 10 |> Array.append (serializeCharArray (str.ToCharArray()) str.Length)
            let strWithNulls = deserializeCharArray arrayWithNulls 0 arrayWithNulls.Length
            strWithNulls |> should not' (equal str)
            let arrayWithoutNulls = deserializeCharArrayFilterNull arrayWithNulls 0 arrayWithNulls.Length
            let strWithoutNulls = String arrayWithoutNulls
            strWithoutNulls |> should equal str
        
        [<Test>]
        member x.``serialize/deserialize VarInt``() = 
            let varb = VarIntFromNumber64 0x5UL
            let vars_0 = VarIntFromNumber64 0xfdUL
            let vars_1 = VarIntFromNumber64 0xffeeUL
            let vars_2 = VarIntFromNumber64 0xffffUL
            let var32_0 = VarIntFromNumber64 0xffffffeeUL
            let var32_1 = VarIntFromNumber64 0xffffffffUL
            let var64 = VarIntFromNumber64 0xffffffffffffffeeUL
            let servarb = serializeVarInt varb
            let servars_0 = serializeVarInt vars_0
            let servars_1 = serializeVarInt vars_1
            let servars_2 = serializeVarInt vars_2
            let servar32_0 = serializeVarInt var32_0
            let servar32_1 = serializeVarInt var32_1
            let servar64 = serializeVarInt var64
            let deservarb = deserializeVarInt servarb 0
            let deservars_0 = deserializeVarInt servars_0 0
            let deservars_1 = deserializeVarInt servars_1 0
            let deservars_2 = deserializeVarInt servars_2 0
            let deservar32_0 = deserializeVarInt servar32_0 0
            let deservar32_1 = deserializeVarInt servar32_1 0
            let deservar64 = deserializeVarInt servar64 0
            varb |> should equal deservarb
            vars_0 |> should equal deservars_0
            vars_1 |> should equal deservars_1
            vars_2 |> should equal deservars_2
            var32_0 |> should equal deservar32_0
            var32_1 |> should equal deservar32_1
            var64 |> should equal deservar64
        
        [<Test>]
        member x.``serialize/deserialize NetAddr``() = 
            let addr = 
                { NetAddr.time = 152u
                  NetAddr.services = 512UL
                  NetAddr.ipv6_4 = 
                      [| 0uy; 1uy; 2uy; 3uy; 4uy; 5uy; 6uy; 7uy; 8uy; 9uy; 10uy; 11uy; 12uy; 13uy; 14uy; 15uy |]
                  NetAddr.port = 1234us }
            
            let serialized = serializeNetAddr addr
            let deserialized = deserializeNetAddr serialized 0
            addr.time |> should equal deserialized.time
            addr.services |> should equal deserialized.services
            addr.port |> should equal deserialized.port
            Array.iter2 (fun x y -> x |> should equal y) addr.ipv6_4 deserialized.ipv6_4
        
        [<Test>]
        member x.``serialize/deserialize NetAddr array``() = 
            let addr = 
                { NetAddr.time = 152u
                  NetAddr.services = 512UL
                  NetAddr.ipv6_4 = 
                      [| 0uy; 1uy; 2uy; 3uy; 4uy; 5uy; 6uy; 7uy; 8uy; 9uy; 10uy; 11uy; 12uy; 13uy; 14uy; 15uy |]
                  NetAddr.port = 1234us }
            
            let addresses = 
                [| for _ in 1..10 -> addr |]
            
            let serialized = serializeNetAddrArray addresses
            let deserialized = deserializeNetAddrArray serialized 0 10
            addresses.Length |> should equal deserialized.Length
            Array.iter2 (fun x y -> 
                x.time |> should equal y.time
                x.services |> should equal y.services
                x.port |> should equal y.port
                x.ipv6_4.Length |> should equal y.ipv6_4.Length
                Array.iter2 (fun x y -> x |> should equal y) x.ipv6_4 y.ipv6_4) addresses deserialized
        
        [<Test>]
        member x.``serialized MessageHeader array size``() = 
            iterMessageHeaders (fun (header, serialized) -> serialized.Length |> should equal 24)
        
        [<Test>]
        member x.``deserialized MessageHeader equal to the original message``() = 
            iterMessageHeaders (fun (header, serialized) -> 
                let deserialized = (deserializeHeader serialized 0)
                deserialized.magic |> should equal header.magic
                deserialized.length |> should equal header.length
                deserialized.checksum |> should equal header.checksum
                // header dos not come from deserialization so it can have command
                // field larger that the maximal 12 chars as it is defined in the 
                // Bitcoin protocol specification
                if header.command.Length > 12 then deserialized.command |> should not' (equal header.command)
                deserialized.command |> should equal (header.command |> Array.truncate 12))
