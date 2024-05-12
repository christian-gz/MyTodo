using System.ComponentModel;

namespace TODO.Models;

public enum Priority
{
    [Description("Low Priority")]
    Low,
    [Description("Medium Priority")]
    Medium,
    [Description("High Priority")]
    High,
}