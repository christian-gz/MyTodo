using System;
using ReactiveUI;

namespace TODO.Models;

public class TodoItem : ReactiveObject
{
    private string? _title = string.Empty;
    private bool _isChecked;
    private Priority _priority;
    private DateTime? _dateDeadline;
    private DateTime? _dateFinishedAt;
    private bool _isEdited;
    private bool _isShown = true;

    public DateTime DateTimeAdded { get; } = DateTime.Now;

    public string? Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public bool IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }

    public Priority Priority
    {
        get => _priority;
        set => this.RaiseAndSetIfChanged(ref _priority, value);
    }

    public DateTime? DateDeadline
    {
        get => _dateDeadline;
        set => this.RaiseAndSetIfChanged(ref _dateDeadline, value);
    }

    public DateTime? DateFinishedAt
    {
        get => _dateFinishedAt;
        set => this.RaiseAndSetIfChanged(ref _dateFinishedAt, value);
    }

    public bool IsEdited
    {
        get => _isEdited;
        set => this.RaiseAndSetIfChanged(ref _isEdited, value);
    }

    public bool IsShown
    {
        get => _isShown;
        set => this.RaiseAndSetIfChanged(ref _isShown, value);
    }
}