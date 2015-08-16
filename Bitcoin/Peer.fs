namespace Bitcoin

open System.Net

module Peer =

    type private Message = string

    type T = {
        addr: IPAddress
        mailbox_: MailboxProcessor<Message>
    }

    let clientMessageHandler (inbox: MailboxProcessor<Message>) =
        let rec loop() = 
            async {
                let! message = inbox.Receive()
                do! loop()
            }

        loop()

    let create(address : IPAddress) =
        { addr = address; mailbox_ = MailboxProcessor.Start(clientMessageHandler); }


