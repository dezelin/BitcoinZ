namespace Bitcoin.Tests

open System
open Bitcoin.Messages
open Bitcoin.MessageSerialization
open NUnit.Framework
open FsUnit

module TestMessageSerialization = 
    [<TestFixture>]
    type T() = 
        
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
        member x.``serialized MessageHeader array size``() = 
            iterMessageHeaders (fun (header, serialized) -> serialized.Length |> should equal 24)
        
        [<Test>]
        member x.``deserialized MessageHeader equal to the original message``() = 
            iterMessageHeaders (fun (header, serialized) -> 
                let deserialized = (deserializeHeader serialized)
                deserialized.magic |> should equal header.magic
                deserialized.length |> should equal header.length
                deserialized.checksum |> should equal header.checksum
                // header dos not come from deserialization so it can have command
                // field larger that the maximal 12 chars as it is defined in the 
                // Bitcoin protocol specification
                if header.command.Length > 12 then deserialized.command |> should not' (equal header.command)
                deserialized.command |> should equal (header.command |> Array.truncate 12))
