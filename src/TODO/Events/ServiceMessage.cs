namespace TODO.Events;

public class ServiceMessage : MessageBase
{
    public ServiceMessage(string? propertyName, string? message)
    {
        PropertyName = propertyName;
        Message = message;
    }

    public string? PropertyName { get; set; }
}