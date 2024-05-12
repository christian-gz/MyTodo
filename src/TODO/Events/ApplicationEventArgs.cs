using System;

namespace TODO.Events;

public class ApplicationEventArgs : EventArgs
{
    public EventType EventType { get; private set; }
    public bool IsSuccessful { get; private set; }
    public MessageBase? Message { get; private set; }

    public ApplicationEventArgs(EventType eventType, bool isSuccessful, MessageBase? message)
    {
        EventType = eventType;
        IsSuccessful = isSuccessful;
        Message = message;
    }
}