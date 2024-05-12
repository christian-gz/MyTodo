using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using TODO.Models;
using TODO.Services;

namespace TODO.ViewModels.Stats;

public class StatsViewModel : SwappableViewModelBase
{
    public StatsViewModel()
    {
        _todosDoneTotal = 10;
        _highPriorityTodosTotal = 5;
        _highPriorityTodosDoneTotal = 3;
        _todosWithDeadlineTotal = 8;
        _todosDoneAfterDeadline = 2;
        _todosDoneBeforeDeadline = 4;
    }

    public StatsViewModel(IService todoService)
    {
        _todoService = todoService;
    }

    private readonly IService _todoService;
    private ObservableCollection<TodoItem> _archive;
    private int _todosDoneTotal;
    private int _highPriorityTodosTotal;
    private int _highPriorityTodosDoneTotal;
    private int _mediumPriorityTodosTotal;
    private int _mediumPriorityTodosDoneTotal;
    private int _lowPriorityTodosTotal;
    private int _lowPriorityTodosDoneTotal;
    private int _todosWithDeadlineTotal;
    private int _todosDoneBeforeDeadline;
    private int _todosDoneAfterDeadline;

    public int TodosTotal => _archive.Count;
    public int TodosDoneTotal
    {
        get => _todosDoneTotal;
        set => this.RaiseAndSetIfChanged(ref _todosDoneTotal, value);
    }
    public int HighPriorityTodosTotal
    {
        get => _highPriorityTodosTotal;
        set => this.RaiseAndSetIfChanged(ref _highPriorityTodosTotal, value);
    }
    public int HighPriorityTodosDoneTotal
    {
        get => _highPriorityTodosDoneTotal;
        set => this.RaiseAndSetIfChanged(ref _highPriorityTodosDoneTotal, value);
    }
    public int MediumPriorityTodosTotal
    {
        get => _mediumPriorityTodosTotal;
        set => this.RaiseAndSetIfChanged(ref _mediumPriorityTodosTotal, value);
    }
    public int MediumPriorityTodosDoneTotal
    {
        get => _mediumPriorityTodosDoneTotal;
        set => this.RaiseAndSetIfChanged(ref _mediumPriorityTodosDoneTotal, value);
    }
    public int LowPriorityTodosTotal
    {
        get => _lowPriorityTodosTotal;
        set => this.RaiseAndSetIfChanged(ref _lowPriorityTodosTotal, value);
    }
    public int LowPriorityTodosDoneTotal
    {
        get => _lowPriorityTodosDoneTotal;
        set => this.RaiseAndSetIfChanged(ref _lowPriorityTodosDoneTotal, value);
    }
    public int TodosWithDeadlineTotal
    {
        get => _todosWithDeadlineTotal;
        set => this.RaiseAndSetIfChanged(ref _todosWithDeadlineTotal, value);
    }
    public int TodosDoneBeforeDeadline
    {
        get => _todosDoneBeforeDeadline;
        set => this.RaiseAndSetIfChanged(ref _todosDoneBeforeDeadline, value);
    }
    public int TodosDoneAfterDeadline
    {
        get => _todosDoneAfterDeadline;
        set => this.RaiseAndSetIfChanged(ref _todosDoneAfterDeadline, value);
    }

    private void CalcTodosDoneTotal()
    {
        TodosDoneTotal = 0;
        
        foreach (var todo in _archive)
        {
            if (todo.IsChecked)
            {
                TodosDoneTotal++;
            }
        }
    }
    private void CalcPriorityTodosTotal(Priority priority)
    {
        if (priority == Priority.High)
        {
            HighPriorityTodosTotal = 0;
            foreach (var todo in _archive)
            {
                if (todo.Priority == priority) HighPriorityTodosTotal++;
            }
        }
        else if (priority == Priority.Medium)
        {
            MediumPriorityTodosTotal = 0;
            foreach (var todo in _archive)
            {
                if (todo.Priority == priority) MediumPriorityTodosTotal++;
            }
        }
        else if (priority == Priority.Low)
        {
            LowPriorityTodosTotal = 0;
            foreach (var todo in _archive)
            {
                if (todo.Priority == priority) LowPriorityTodosTotal++;
            }
        }
    }
    private void CalcPriorityTodosDoneTotal(Priority priority)
    {
        if (priority == Priority.High)
        {
            HighPriorityTodosDoneTotal = 0;
            foreach (var todo in _archive)
            {
                if (todo.IsChecked && todo.Priority == priority) HighPriorityTodosDoneTotal++;
            }
        }
        else if (priority == Priority.Medium)
        {
            MediumPriorityTodosDoneTotal = 0;
            foreach (var todo in _archive)
            {
                if (todo.IsChecked && todo.Priority == priority) MediumPriorityTodosDoneTotal++;
            }
        }
        else if (priority == Priority.Low)
        {
            LowPriorityTodosDoneTotal = 0;
            foreach (var todo in _archive)
            {
                if (todo.IsChecked && todo.Priority == priority) LowPriorityTodosDoneTotal++;
            }
        }
    }

    private void CalcTodosWithDeadlineTotal()
    {
        TodosWithDeadlineTotal = 0;

        foreach (var todo in _archive)
        {
            if (todo.DateDeadline == null)
            {
                continue;
            }

            TodosWithDeadlineTotal++;
        }
    }

    private void CalcTodosDoneBeforeAndAfterDeadline()
    {
        TodosDoneBeforeDeadline = 0;
        TodosDoneAfterDeadline = 0;

        foreach (var todo in _archive)
        {
            DateTime? deadline = todo.DateDeadline;
            DateTime? finishedAt = todo.DateFinishedAt;

            if (deadline == null || !todo.IsChecked || finishedAt == null ) continue;

            TimeSpan delta = ((DateTime)deadline).Subtract((DateTime)finishedAt);

            if (delta.Days >= 0)
            {
                TodosDoneBeforeDeadline++;
            }
            else
            {
                TodosDoneAfterDeadline++;
            }
        }
    }

    public override void LoadViewModel()
    {
        _archive = new ObservableCollection<TodoItem>(_todoService.GetArchive());

        CalcTodosDoneTotal();
        this.RaisePropertyChanged(nameof(TodosTotal));
        CalcPriorityTodosTotal(Priority.High);
        CalcPriorityTodosDoneTotal(Priority.High);
        CalcPriorityTodosTotal(Priority.Medium);
        CalcPriorityTodosDoneTotal(Priority.Medium);
        CalcPriorityTodosTotal(Priority.Low);
        CalcPriorityTodosDoneTotal(Priority.Low);
        CalcTodosWithDeadlineTotal();
        CalcTodosDoneBeforeAndAfterDeadline();
    }
}