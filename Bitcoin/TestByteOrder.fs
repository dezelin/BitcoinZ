namespace Bitcoin.Tests

open System
open Bitcoin.ByteOrder
open NUnit.Framework
open FsUnit

module TestByteOrder = 
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
        
        [<TestFixtureSetUp>]
        static member setup() = ()
        
        [<TestFixtureTearDown>]
        static member clean() = ()
        
        [<SetUp>]
        member x.tearUp() = ()
        
        [<TearDown>]
        member x.tearDown() = ()
        
        [<Test>]
        member x.letohoi16() = 
            let x = letohoi16 lei16
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal lei16
            | false -> x |> should not' (equal lei16)
        
        [<Test>]
        member x.letohoi32() = 
            let x = letohoi32 lei32
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal lei32
            | false -> x |> should not' (equal lei32)
        
        [<Test>]
        member x.letohoi64() = 
            let x = letohoi16 lei16
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal lei16
            | false -> x |> should not' (equal lei16)
        
        [<Test>]
        member x.letohou16() = 
            let x = letohou16 leu16
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal leu16
            | false -> x |> should not' (equal leu16)
        
        [<Test>]
        member x.letohou32() = 
            let x = letohou32 leu32
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal leu32
            | false -> x |> should not' (equal leu32)
        
        [<Test>]
        member x.letohou64() = 
            let x = letohou64 leu64
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal leu64
            | false -> x |> should not' (equal leu64)
        
        [<Test>]
        member x.betohoi16() = 
            let x = betohoi16 bei16
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal bei16
            | false -> x |> should not' (equal bei16)
        
        [<Test>]
        member x.betohoi32() = 
            let x = betohoi32 bei32
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal bei32
            | false -> x |> should not' (equal bei32)
        
        [<Test>]
        member x.betohoi64() = 
            let x = betohoi16 bei16
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal bei16
            | false -> x |> should not' (equal bei16)
        
        [<Test>]
        member x.betohou16() = 
            let x = betohou16 beu16
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal beu16
            | false -> x |> should not' (equal beu16)
        
        [<Test>]
        member x.betohou32() = 
            let x = betohou32 beu32
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal beu32
            | false -> x |> should not' (equal beu32)
        
        [<Test>]
        member x.betohou64() = 
            let x = betohou64 beu64
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal beu64
            | false -> x |> should not' (equal beu64)
        
        [<Test>]
        member x.hotolei16() = 
            let x = hotolei16 lei16
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal lei16
            | false -> x |> should not' (equal lei16)
        
        [<Test>]
        member x.hotolei32() = 
            let x = hotolei32 lei32
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal lei32
            | false -> x |> should not' (equal lei32)
        
        [<Test>]
        member x.hotolei64() = 
            let x = hotolei16 lei16
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal lei16
            | false -> x |> should not' (equal lei16)
        
        [<Test>]
        member x.hotoleu16() = 
            let x = hotoleu16 leu16
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal leu16
            | false -> x |> should not' (equal leu16)
        
        [<Test>]
        member x.hotoleu32() = 
            let x = hotoleu32 leu32
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal leu32
            | false -> x |> should not' (equal leu32)
        
        [<Test>]
        member x.hotoleu64() = 
            let x = hotoleu64 leu64
            match BitConverter.IsLittleEndian with
            | true -> x |> should equal leu64
            | false -> x |> should not' (equal leu64)
        
        [<Test>]
        member x.hotobei16() = 
            let x = hotobei16 bei16
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal bei16
            | false -> x |> should not' (equal bei16)
        
        [<Test>]
        member x.hotobei32() = 
            let x = hotobei32 bei32
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal bei32
            | false -> x |> should not' (equal bei32)
        
        [<Test>]
        member x.hotobei64() = 
            let x = betohoi16 bei16
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal bei16
            | false -> x |> should not' (equal bei16)
        
        [<Test>]
        member x.hotobeu16() = 
            let x = betohou16 beu16
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal beu16
            | false -> x |> should not' (equal beu16)
        
        [<Test>]
        member x.hotobeu32() = 
            let x = betohou32 beu32
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal beu32
            | false -> x |> should not' (equal beu32)
        
        [<Test>]
        member x.hotobeu64() = 
            let x = betohou64 beu64
            match not BitConverter.IsLittleEndian with
            | true -> x |> should equal beu64
            | false -> x |> should not' (equal beu64)
        
        [<Test>]
        member x.```hotolei16/letohoi16``() = lei16 |> should equal (hotolei16 (letohoi16 lei16))
        
        [<Test>]
        member x.```hotolei32/letohoi32``() = lei32 |> should equal (hotolei32 (letohoi32 lei32))
        
        [<Test>]
        member x.```hotolei64/letohoi64``() = lei64 |> should equal (hotolei64 (letohoi64 lei64))
        
        [<Test>]
        member x.```hotoleu16/letohou16``() = leu16 |> should equal (hotoleu16 (letohou16 leu16))
        
        [<Test>]
        member x.```hotoleu32/letohou32``() = leu32 |> should equal (hotoleu32 (letohou32 leu32))
        
        [<Test>]
        member x.```hotoleu64/letohou64``() = leu64 |> should equal (hotoleu64 (letohou64 leu64))
        
        [<Test>]
        member x.```hotobei16/betohoi16``() = bei16 |> should equal (hotobei16 (betohoi16 bei16))
        
        [<Test>]
        member x.```hotobei32/betohoi32``() = bei32 |> should equal (hotobei32 (betohoi32 bei32))
        
        [<Test>]
        member x.```hotobei64/betohoi64``() = bei64 |> should equal (hotobei64 (betohoi64 bei64))
        
        [<Test>]
        member x.```hotobeu16/betohou16``() = beu16 |> should equal (hotobeu16 (betohou16 beu16))
        
        [<Test>]
        member x.```hotobeu32/betohou32``() = beu32 |> should equal (hotobeu32 (betohou32 beu32))
        
        [<Test>]
        member x.```hotobeu64/betohou64``() = beu64 |> should equal (hotobeu64 (betohou64 beu64))
