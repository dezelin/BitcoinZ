namespace Bitcoin

open System.Net

module DnsSeed =

    let private seedUrls = [ 
        "bitseed.xf2.org" 
        "dnsseed.bluematt.me" 
        "seed.bitcoin.sipa.be" 
        "dnsseed.bitcoin.dashjr.org" 
        "seed.bitcoinstats.com" 
        ]

    let fetchAsync (url : string) = 
        async { 
            try
                let! address = Async.FromBeginEnd
                                     (url, (fun (url, callback, state) -> Dns.BeginGetHostAddresses(url, callback, state)), 
                                      Dns.EndGetHostAddresses)
                return address
            with _ -> return [||]
        }

    let fetch() = 
        seedUrls
        |> Seq.map fetchAsync
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.concat

