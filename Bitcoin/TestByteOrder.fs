namespace Bitcoin.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Bitcoin.ByteOrder

module TestByteOrder = 
    [<TestClass>]
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
        
        [<ClassInitialize>]
        static member tearUp (context : TestContext) = ()
        
        [<ClassCleanup>]
        static member tearDown() = ()
        
        [<TestInitialize>]
        member x.setup() = ()
        
        [<TestCleanup>]
        member x.clean() = ()
        
        [<TestMethod>]
        member x.ByteOrder_letohoi16() = 
            let x = letohoi16 lei16
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, lei16)
            | false -> Assert.AreNotEqual(x, lei16)
        
        [<TestMethod>]
        member x.ByteOrder_letohoi32() = 
            let x = letohoi32 lei32
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, lei32)
            | false -> Assert.AreNotEqual(x, lei32)
        
        [<TestMethod>]
        member x.ByteOrder_letohoi64() = 
            let x = letohoi16 lei16
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, lei16)
            | false -> Assert.AreNotEqual(x, lei16)
        
        [<TestMethod>]
        member x.ByteOrder_letohou16() = 
            let x = letohou16 leu16
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, leu16)
            | false -> Assert.AreNotEqual(x, leu16)
        
        [<TestMethod>]
        member x.ByteOrder_letohou32() = 
            let x = letohou32 leu32
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, leu32)
            | false -> Assert.AreNotEqual(x, leu32)
        
        [<TestMethod>]
        member x.ByteOrder_letohou64() = 
            let x = letohou64 leu64
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, leu64)
            | false -> Assert.AreNotEqual(x, leu64)
        
        [<TestMethod>]
        member x.ByteOrder_betohoi16() = 
            let x = betohoi16 bei16
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, bei16)
            | false -> Assert.AreNotEqual(x, bei16)
        
        [<TestMethod>]
        member x.ByteOrder_betohoi32() = 
            let x = betohoi32 bei32
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, bei32)
            | false -> Assert.AreNotEqual(x, bei32)
        
        [<TestMethod>]
        member x.ByteOrder_betohoi64() = 
            let x = betohoi16 bei16
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, bei16)
            | false -> Assert.AreNotEqual(x, bei16)
        
        [<TestMethod>]
        member x.ByteOrder_betohou16() = 
            let x = betohou16 beu16
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, beu16)
            | false -> Assert.AreNotEqual(x, beu16)
        
        [<TestMethod>]
        member x.ByteOrder_betohou32() = 
            let x = betohou32 beu32
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, beu32)
            | false -> Assert.AreNotEqual(x, beu32)
        
        [<TestMethod>]
        member x.ByteOrder_betohou64() = 
            let x = betohou64 beu64
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, beu64)
            | false -> Assert.AreNotEqual(x, beu64)
        
        [<TestMethod>]
        member x.ByteOrder_hotolei16() = 
            let x = hotolei16 lei16
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, lei16)
            | false -> Assert.AreNotEqual(x, lei16)
        
        [<TestMethod>]
        member x.ByteOrder_hotolei32() = 
            let x = hotolei32 lei32
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, lei32)
            | false -> Assert.AreNotEqual(x, lei32)
        
        [<TestMethod>]
        member x.ByteOrder_hotolei64() = 
            let x = hotolei16 lei16
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, lei16)
            | false -> Assert.AreNotEqual(x, lei16)
        
        [<TestMethod>]
        member x.ByteOrder_hotoleu16() = 
            let x = hotoleu16 leu16
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, leu16)
            | false -> Assert.AreNotEqual(x, leu16)
        
        [<TestMethod>]
        member x.ByteOrder_hotoleu32() = 
            let x = hotoleu32 leu32
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, leu32)
            | false -> Assert.AreNotEqual(x, leu32)
        
        [<TestMethod>]
        member x.ByteOrder_hotoleu64() = 
            let x = hotoleu64 leu64
            match BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, leu64)
            | false -> Assert.AreNotEqual(x, leu64)
        
        [<TestMethod>]
        member x.ByteOrder_hotobei16() = 
            let x = hotobei16 bei16
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, bei16)
            | false -> Assert.AreNotEqual(x, bei16)
        
        [<TestMethod>]
        member x.ByteOrder_hotobei32() = 
            let x = hotobei32 bei32
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, bei32)
            | false -> Assert.AreNotEqual(x, bei32)
        
        [<TestMethod>]
        member x.ByteOrder_hotobei64() = 
            let x = betohoi16 bei16
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, bei16)
            | false -> Assert.AreNotEqual(x, bei16)
        
        [<TestMethod>]
        member x.ByteOrder_hotobeu16() = 
            let x = betohou16 beu16
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, beu16)
            | false -> Assert.AreNotEqual(x, beu16)
        
        [<TestMethod>]
        member x.ByteOrder_hotobeu32() = 
            let x = betohou32 beu32
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, beu32)
            | false -> Assert.AreNotEqual(x, beu32)
        
        [<TestMethod>]
        member x.ByteOrder_hotobeu64() = 
            let x = betohou64 beu64
            match not BitConverter.IsLittleEndian with
            | true -> Assert.AreEqual(x, beu64)
            | false -> Assert.AreNotEqual(x, beu64)

