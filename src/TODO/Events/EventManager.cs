using System;

namespace TODO.Events;

public static class EventManager
{
    /// <summary>
    /// Program-wide events
    /// </summary>
    public static event EventHandler<ApplicationEventArgs> ApplicationEvent;

    public static void RaiseEvent(EventType eventType, bool isSuccessful, MessageBase? message)
    {
        var args = new ApplicationEventArgs(eventType, isSuccessful, message);
        ApplicationEvent?.Invoke(null, args);
    }

    public static void RaiseEvent(ApplicationEventArgs e)
    {
        ApplicationEvent?.Invoke(null, e);
    }
}