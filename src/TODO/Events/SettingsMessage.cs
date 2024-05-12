namespace TODO.Events;

public class SettingsMessage : MessageBase
{
    public SettingsMessage(string? message)
    {
        Message = message;
    }
}