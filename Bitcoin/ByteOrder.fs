namespace Bitcoin

open System

module ByteOrder = 

    let seqRev xs = Seq.fold(fun acc x -> x::acc) [] xs
    //
    // Convert from little-endian to host byte order
    //
    let letohoi16 x : int16 = 
        match BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToInt16(bytes, 0)
    
    let letohoi32 x : int32 = 
        match BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToInt32(bytes, 0)
    
    let letohoi64 x : int64 = 
        match BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToInt64(bytes, 0)
    
    let letohou16 x : uint16 = 
        match BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToUInt16(bytes, 0)
    
    let letohou32 x : uint32 = 
        match BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToUInt32(bytes, 0)
    
    let letohou64 x : uint64 = 
        match BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToUInt64(bytes, 0)
    
    //
    // Convert from big-endian to host byte order
    //
    let betohoi16 x : int16 = 
        match not BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToInt16(bytes, 0)
    
    let betohoi32 x : int32 = 
        match not BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToInt32(bytes, 0)
    
    let betohoi64 x : int64 = 
        match not BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToInt64(bytes, 0)
    
    let betohou16 x : uint16 = 
        match not BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToUInt16(bytes, 0)
    
    let betohou32 x : uint32 = 
        match not BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToUInt32(bytes, 0)
    
    let betohou64 x : uint64 = 
        match not BitConverter.IsLittleEndian with
        | true -> x
        | false -> 
            let bytes = Seq.toArray (seqRev (BitConverter.GetBytes x))
            BitConverter.ToUInt64(bytes, 0)
    
    //
    // Convert from host to little-endian byte order
    //
    let hotolei16 x : int16 = letohoi16 x
    let hotolei32 x : int32 = letohoi32 x
    let hotolei64 x : int64 = letohoi64 x
    let hotoleu16 x : uint16 = letohou16 x
    let hotoleu32 x : uint32 = letohou32 x
    let hotoleu64 x : uint64 = letohou64 x
    //
    // Convert from host to big-endian byte order
    //
    let hotobei16 x : int16 = betohoi16 x
    let hotobei32 x : int32 = betohoi32 x
    let hotobei64 x : int64 = betohoi64 x
    let hotobeu16 x : uint16 = betohou16 x
    let hotobeu32 x : uint32 = betohou32 x
    let hotobeu64 x : uint64 = betohou64 x
