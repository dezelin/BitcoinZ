namespace Bitcoin

open Bitcoin.Agent
open System.Collections.Generic

module BlockingQueueAgent =

    type internal BlockingQueueAgentMessage<'T> =
        | Get of AsyncReplyChannel<'T>
        | Add of 'T * AsyncReplyChannel<unit>

    type T<'Msg>(maxLength) =
        let agent = Agent.T.Start(fun agent ->
            let queue = new Queue<_>()

            let rec emptyQueue() = 
                agent.Scan(fun msg ->
                    match msg with
                    | Add(value, reply) -> Some(enqueueAndContinue(value, reply))
                    | _ -> None
                )
            and fullQueue() = 
                agent.Scan(fun msg ->
                    match msg with
                    | Get(reply) -> Some(dequeueAndContinue(reply))
                    | _ -> None
                )
            and runningQueue() = async {
                let! msg = agent.Receive()
                match msg with
                | Add(value, reply) -> return! enqueueAndContinue(value, reply)
                | Get(reply) -> return! dequeueAndContinue(reply)
                }
            and enqueueAndContinue(value, reply) = async {
                queue.Enqueue(value)
                reply.Reply()
                return! chooseState()        
                }
            and dequeueAndContinue(reply) = async {
                reply.Reply(queue.Dequeue())
                return! chooseState()
                }
            and chooseState() =
                if queue.Count = 0 then emptyQueue()
                elif queue.Count < maxLength then runningQueue()
                else fullQueue()

            emptyQueue()
        )

        // Asynchronously add message to the queue. If queue is full block.
        member x.AsyncAdd(value: 'Msg) =
            agent.PostAndAsyncReply(fun channel -> Add(value, channel))

        // Asynchronously get message from the queue. If queue is empty block.
        member x.AsyncGet() =
            agent.PostAndAsyncReply(Get)

