namespace Bitcoin

open System

module Messages = 
    type VarIntMarker = 
        | VarIntMarkerShort = 0xfduy
        | VarIntMarkerInt32 = 0xfeuy
        | VarIntMarkerInt64 = 0xffuy
    
    type VarIntByte = 
        { value : uint8 // Should be < 0xfd
                        }
    
    type VarIntShort = 
        { marker : VarIntMarker // Should always be 0xfd
          value : uint16 }
    
    type VarInt32 = 
        { marker : VarIntMarker // Should always be 0xfe
          value : uint32 }
    
    type VarInt64 = 
        { marker : VarIntMarker // Should always be 0xff
          value : uint64 }
    
    type VarInt = 
        | VarIntByte of VarIntByte
        | VarIntShort of VarIntShort
        | VarInt32 of VarInt32
        | VarInt64 of VarInt64
    
    let VarIntFromNumber64(x : uint64) = 
        if x < 0xfdUL then VarIntByte { value = uint8 x }
        elif x > 0xfdUL && x <= 0xffffUL then 
            VarIntShort { marker = VarIntMarker.VarIntMarkerShort
                          value = uint16 x }
        elif x <= 0xffffffffUL then 
            VarInt32 { marker = VarIntMarker.VarIntMarkerInt32
                       value = uint32 x }
        else 
            VarInt64 { marker = VarIntMarker.VarIntMarkerInt64
                       value = uint64 x }
    
    let VarIntToNumber64(x : VarInt) = 
        match x with
        | VarIntByte v -> uint64 v.value
        | VarIntShort v -> uint64 v.value
        | VarInt32 v -> uint64 v.value
        | VarInt64 v -> uint64 v.value
    
    type VarString = 
        { length : VarInt // Length of the string
          string : char [] // The string itself (can be empty)
                           }
    
    let internal NetAddrPayloadSize = 30
    
    type NetAddr = 
        { // the Time (version >= 31402). Not present in version message.
          time : uint32
          // same service(s) listed in version
          services : uint64
          // IPv6 address. Network byte order. The original client only 
          // supported IPv4 and only read the last 4 bytes to get the IPv4 address. 
          // However, the IPv4 address is written into the message as a 16 byte 
          // IPv4-mapped IPv6 address
          // (12 bytes 00 00 00 00 00 00 00 00 00 00 FF FF, followed by the 4 bytes of the IPv4 address).
          // byte[16] is the maximum length
          ipv6_4 : byte []
          // port number, network byte order
          port : uint16 }
    
    type InvVecObjectType = 
        | ERROR = 0x0u // Any data of with this number may be ignored
        | MSG_TX = 0x1u // Hash is related to a transaction
        | MSG_BLOCK = 0x2u // Hash is related to a data block
        // Hash of a block header; identical to MSG_BLOCK. 
        // When used in a getdata message, this indicates the reply should be 
        // a merkleblock message rather than a block message; 
        // this only works if a bloom filter has been set.
        | MSG_FILTERED_BLOCK = 0x3u
    
    type InvVec = 
        { // Identifies the object type linked to this inventory 
          objectType : InvVecObjectType
          // Hash of the object
          // byte[32] is the length
          hash : byte [] }
    
    type BlockHeader = 
        { // Block version information (note, this is signed)
          version : int32
          // The hash value of the previous block this particular block references
          // char[32] is the length
          prevBlock : byte []
          // The reference to a Merkle tree collection which is a hash of 
          // all transactions related to this block
          // char[32] is the length
          merkleRoot : byte []
          // A timestamp recording when this block was created (Will overflow in 2106)
          timestamp : uint32
          // The calculated difficulty target being used for this block
          bits : uint32
          // The nonce used to generate this block… to allow variations of 
          // the header and compute different hashes
          nonce : uint32
          // Number of transaction entries, this value is always 0
          txnCount : VarInt }
    
    // Message header magic number
    type MagicNumber = 
        | MainNetwork       = 0xd9b4bef9u // main
        | TestNetNetwork    = 0xdab5bffau // testnet
        | TestNet3Network   = 0x0709110bu // testnet3
        | NamecoinNetwork   = 0xfeb4bef9u // namecoin
    
    type MessageHeader = 
        { // Magic value indicating message origin network, 
          // and used to seek to next message when stream state is unknown
          // Enum underlying type must be uint32
          magic : MagicNumber
          // ASCII string identifying the packet content, 
          // NULL padded (non-NULL padding results in packet rejected)         
          // char[12] is the maximum length
          command : char []
          // Length of payload in number of bytes
          length : uint32
          // First 4 bytes of sha256(sha256(payload))
          checksum : uint32 }
    
    [<Flags>]
    type ServiceType = 
        | NODE_NETWORK = 0x1ul // 64 bits
    
    // When a node creates an outgoing connection, it will immediately 
    // advertise its version. The remote node will respond with its version. 
    // No further communication is possible until both peers have exchanged 
    // their version.
    type MessageVersion = 
        { // Identifies protocol version being used by the node
          version : int32
          // bitfield of features to be enabled for this connection
          // uint64 is the underlying type
          services : ServiceType
          // standard UNIX timestamp in seconds
          timestamp : int64
          // The network address of the node receiving this message
          addrRecv : NetAddr
          //
          // Fields below require version ≥ 106
          //
          // The network address of the node emitting this message
          addrFrom : NetAddr
          // Node random nonce, randomly generated every time a version 
          // packet is sent. This nonce is used to detect connections to self
          nonce : uint64
          // User Agent (0x00 if string is 0 bytes long)
          UserAgent : VarString
          // The last block received by the emitting node
          startHeight : int32
          //
          // Fields below require version ≥ 70001
          //
          // Whether the remote peer should announce relayed transactions 
          // or not, see BIP 0037
          relay : bool }
    
    // The verack message is sent in reply to version. This message consists of 
    // only a message header with the command string "verack".
    // Define it as an empty structure
    type MessageVerack = 
        struct
        end
    
    // Provide information on known nodes of the network. 
    // Non-advertised nodes should be forgotten after typically 3 hours
    type MessageAddr = 
        { // Number of address entries (min: 1, max: 1000)
          count : VarInt
          // Address of other nodes on the network. 
          // Version < 209 will only read the first one.
          addrList : NetAddr [] }
    
    // Allows a node to advertise its knowledge of one or more objects. 
    // It can be received unsolicited, or in reply to getblocks.
    // Payload (maximum 50,000 entries, which is just over 1.8 megabytes):
    type MessageInv = 
        { // Number of inventory entries
          count : VarInt
          // Inventory vectors
          inventory : InvVec [] }
    
    // Used in response to inv, to retrieve the content of a 
    // specific object, and is usually sent after receiving an inv packet, 
    // after filtering known elements. It can be used to retrieve transactions, 
    // but only if they are in the memory pool or relay set - arbitrary access 
    // to transactions in the chain is not allowed to avoid having clients 
    // start to depend on nodes having full transaction indexes 
    // (which modern nodes do not).
    // Payload (maximum 50,000 entries, which is just over 1.8 megabytes):
    type MessageGetData = 
        { // Number of inventory entries
          count : VarInt
          // Inventory vectors
          inventory : InvVec }
    
    // Response to a MessageGetData, sent if any requested data items could not be relayed, for example, 
    // because the requested transaction was not in the memory pool or relay set.
    type MessageNotFound = 
        { // Number of inventory entries
          count : VarInt
          // Inventory vectors
          inventory : InvVec }
    
    // Return an inv packet containing the list of blocks starting right after 
    // the last known hash in the block locator object, up to hash_stop or 
    // 500 blocks, whichever comes first.
    type MessageGetBlocks = 
        { // the protocol version
          version : uint32
          // number of block locator hash entries
          hashCount : VarInt
          // block locator object; newest back to genesis block 
          // (dense to start, but then sparse)
          // Every hash is byte[32] long
          blockLocatorHashes : byte []
          // hash of the last desired block; set to zero to get as many blocks 
          // as possible (500)
          // byte[32] is the length of the array
          hashStop : byte [] }
    
    // Return a headers packet containing the headers of blocks starting right 
    // after the last known hash in the block locator object, up to hash_stop 
    // or 2000 blocks, whichever comes first. To receive the next block headers, 
    // one needs to issue getheaders again with a new block locator object. 
    // The getheaders command is used by thin clients to quickly download 
    // the block chain where the contents of the transactions would be 
    // irrelevant (because they are not ours). 
    // Keep in mind that some clients may provide headers of blocks which are 
    // invalid if the block locator object contains a hash on the invalid branch.
    type MessageGetHeaders = 
        { // the protocol version
          version : uint32
          // number of block locator hash entries
          hashCount : VarInt
          // block locator object; newest back to genesis block 
          // (dense to start, but then sparse)
          // Each hash is a byte[32]
          blockLocatorHashes : byte []
          // hash of the last desired block header; set to zero to get as many 
          // blocks as possible (2000)
          // The lenght is byte[32]
          hashStop : byte [] }
    
    type OutPoint = 
        { // The hash of the referenced transaction.
          // The length is byte[32]
          hash : byte []
          // The index of the specific output in the transaction. 
          // The first output is 0, etc.
          index : uint32 }
    
    type TxIn = 
        { // The previous output transaction reference, as an OutPoint structure
          previousOutput : OutPoint
          // The length of the signature script
          scriptLength : VarInt
          // Computational Script for confirming transaction authorization
          signatureScript : char [] // uchar[] in the protocol specification??
          // Transaction version as defined by the sender. Intended for 
          // "replacement" of transactions when information is updated before 
          // inclusion into a block.
          sequence : uint32 }
    
    type TxOut = 
        { // Transaction Value
          value : int64
          // Length of the pk_script
          pkScriptLength : VarInt
          // Usually contains the public key as a Bitcoin script setting up 
          // conditions to claim this output.
          pkScript : char [] // uchar[] in the protocol specification
                             }
    
    // tx describes a bitcoin transaction, in reply to getdata
    type MessageTx = 
        { // Transaction data format version
          version : uint32
          // Number of Transaction inputs
          txInCount : VarInt
          // A list of 1 or more transaction inputs or sources for coins
          txIn : TxIn []
          // Number of Transaction outputs
          txOutCount : VarInt
          // A list of 1 or more transaction outputs or destinations for coins
          txOut : TxOut []
          // The block number or timestamp at which this transaction is locked:
          //      0	            Not locked
          //      < 500000000	    Block number at which this transaction is locked
          //      >= 500000000    UNIX timestamp at which this transaction is locked
          lockTime : uint32 }
    
    // The block message is sent in response to a getdata message which 
    // requests transaction information from a block hash.
    type MessageBlock = 
        { // Block version information (note, this is signed)
          version : uint32
          // The hash value of the previous block this particular block references
          // The length is byte[32]
          prevBlock : byte []
          // The reference to a Merkle tree collection which is a hash of all 
          // transactions related to this block
          // The length is char[32]
          merkleRoot : byte []
          // A Unix timestamp recording when this block was created 
          // (Currently limited to dates before the year 2106!)
          timestamp : uint32
          // The calculated difficulty target being used for this block
          bits : uint32
          // The nonce used to generate this block… to allow variations of the 
          // header and compute different hashes
          nonce : uint32
          // Number of transaction entries
          txnCount : VarInt
          // Block transactions, in format of "tx" command
          txns : MessageTx [] }
    
    // The headers packet returns block headers in response to a getheaders packet
    type MessageHeaders = 
        { // Number of block headers
          count : VarInt
          // Block headers
          headers : BlockHeader [] }
    
    type MessageGetAddr = 
        struct
        end
    
    type MessageMemPool = 
        struct
        end
    
    type MessagePing = 
        { // random nonce
          nonce : uint64 }
    
    type MessagePong = 
        { // nonce from ping
          nonce : uint64 }
    
    // The underlying type for enum is char
    type CCodes = 
        | RejectMalformed       = 0x1uy
        | RejectInvalid         = 0x10uy
        | RejectObsolete        = 0x11uy
        | RejectDuplicate       = 0x12uy
        | RejectNonstandard     = 0x40uy
        | RejectDust            = 0x41uy
        | RejectInsufficientFee = 0x42uy
        | RejectCheckpoint      = 0x43uy
    
    // The reject message is sent when messages are rejected.
    type MessageReject = 
        { // type of message rejected
          message : VarString
          // code relating to rejected message
          // The underlying type for enum is char
          ccode : CCodes
          // text version of reason for rejection
          reason : VarString
          // Optional extra data provided by some errors. Currently, all errors 
          // which provide this field fill it with the TXID or block header hash 
          // of the object being rejected, so the field is 32 bytes.
          data : byte [] }
    
    type MessageFilterLoad = 
        { // The filter itself is simply a bit field of arbitrary byte-aligned size. 
          // The maximum size is 36,000 bytes
          filter : uint8 []
          // The number of hash functions to use in this filter. 
          // The maximum value allowed in this field is 50.
          nHashFunc : uint32
          // A random value to add to the seed value in the hash function used by 
          // the bloom filter.
          nTweak : uint32
          // A set of flags that control how matched items are added to the filter.
          nFlags : uint8 }
    
    type MessageFilterAdd = 
        { // The data element to add to the current filter.
          data : uint8 [] }
    
    type MessageFilterClear = 
        struct
        end
    
    type MessageMerkleBlock = 
        { // Block version information, based upon the software version creating this block
          version : uint32
          // The hash value of the previous block this particular block references
          // The length is byte[32]
          prevBlock : byte []
          // The reference to a Merkle tree collection which is a hash of all 
          // transactions related to this block
          // The length is byte[32]
          merkleRoot : byte []
          // A timestamp recording when this block was created (Limited to 2106!)
          timestamp : uint32
          // The calculated difficulty target being used for this block
          bits : uint32
          // The nonce used to generate this block… to allow variations of 
          // the header and compute different hashes
          nonce : uint32
          // Number of transactions in the block (including unmatched ones)
          totalTransactions : uint32
          // hashes in depth-first order (including standard varint size prefix)
          // Each hash is byte[32]
          hashes : byte []
          // flag bits, packed per 8 in a byte, least significant bit first 
          // (including standard varint size prefix)
          flags : byte [] }
    
    // The payload is serialized into a uchar[] to ensure that versions 
    // using incompatible alert formats can still relay alerts among one another. 
    // The current alert payload format is
    type AlertPayload = 
        { // Alert format version
          version : int32
          // The timestamp beyond which nodes should stop relaying this alert
          realyUntil : int64
          // The timestamp beyond which this alert is no longer in effect 
          // and should be ignored        
          expiration : int64
          // A unique ID number for this alert
          id : int32
          // All alerts with an ID number less than or equal to this number 
          // should be canceled: deleted and not accepted in the future
          cancel : int32
          // All alert IDs contained in this set should be canceled as above
          setCancelLength : VarInt
          setCancel : int32 []
          // This alert only applies to versions greater than or equal to 
          // this version. Other versions should still relay it.
          minVer : int32
          // This alert only applies to versions less than or equal to 
          // this version. Other versions should still relay it.
          maxVer : int32
          // If this set contains any elements, then only nodes that have 
          // their subVer contained in this set are affected by the alert. 
          // Other versions should still relay it.
          setSubVerLength : VarInt
          setSubVer : VarString
          // Relative priority compared to other alerts
          priority : int32
          // A comment on the alert that is not displayed
          comment : VarString
          // The alert message that is displayed to the user
          statusBar : VarString
          // Reserved
          reserver : VarString }
    
    // An alert is sent between nodes to send a general notification 
    // message throughout the network.
    type MessageAlert = 
        { // Serialized alert payload
          payload : AlertPayload
          // An ECDSA signature of the message
          signature : char [] // uchar[] in the protocol specification
                              }
    
    type MessagePayload = 
        | MessageAddr of MessageAddr
        | MessageAlert of MessageAlert
        | MessageBlock of MessageBlock
        | MessageFilterAdd of MessageFilterAdd
        | MessageFilterClear of MessageFilterClear
        | MessageFilterLoad of MessageFilterLoad
        | MessageGetAddr of MessageGetAddr
        | MessageGetBlocks of MessageGetBlocks
        | MessageGetData of MessageGetData
        | MessageGetHeaders of MessageGetHeaders
        | MessageHeaders of MessageHeaders
        | MessageInv of MessageInv
        | MessageMemPool of MessageMemPool
        | MessageMerkleBlock of MessageMerkleBlock
        | MessageNotFound of MessageNotFound
        | MessagePing of MessagePing
        | MessagePong of MessagePong
        | MessageReject of MessageReject
        | MessageTx of MessageTx
        | MessageVerack of MessageVerack
        | MessageVersion of MessageVersion
    
    // Bitcoin protocol message
    type BitcoinMessage = 
        { header : MessageHeader
          payload : MessagePayload }
