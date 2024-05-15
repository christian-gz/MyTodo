namespace TODO.Events;

public class SettingsMessage : MessageBase
{
    public SettingsMessage (string? propertyName, string? message)
    {
        PropertyName = propertyName;
        Message = message;
    }

    public string? PropertyName { get; set; }
}