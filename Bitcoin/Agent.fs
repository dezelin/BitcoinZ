namespace Bitcoin

module Agent = 
    // Agent.T is a type alias for MailboxProcessor
    type T<'Msg> = MailboxProcessor<'Msg>
