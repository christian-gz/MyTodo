using System;

namespace TODO.Configuration;

public class SettingsChangedEventArgs : EventArgs
{
    public string? Setting { get; set; }
    public string? Value { get; set; }
}