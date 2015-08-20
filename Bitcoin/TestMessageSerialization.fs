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
