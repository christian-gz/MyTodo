using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using TODO.Models;
using TODO.Services;

namespace TODO.ViewModels.Todo;

public class TodoListViewModel : SwappableViewModelBase
{
    public TodoListViewModel()
    {
        TodoList = new ObservableCollection<TodoItem>()
        {
            new TodoItem() { Title = "Do something", Priority = Priority.Medium },
            new TodoItem() { Title = "Buy some milk", Priority = Priority.Low, IsChecked = true },
            new TodoItem() { Title = "Do something else", Priority = Priority.High },
            new TodoItem() { Title = "Go on a walk", Priority = Priority.Low, IsEdited = true}
        };
    }

    public TodoListViewModel(IService todoService)
    {
        _todoService = todoService;
        TodoList = new ObservableCollection<TodoItem>(_todoService.GetTodoList());
        TodoItemViewModel = new TodoItemViewModel(TodoList, Priorities);

        TodoItemViewModel.TodoEdited += OnTodoEdited;
        TodoItemViewModel.TodoDeleted += OnTodoDeleted;

        _todoService.TodoListChanged += OnTodoListChanged;

        this.WhenAnyValue(o => o.SortingSelected)
            .Subscribe(_ => ChangeSorting());
        this.WhenAnyValue(o => o.FilterSelected)
            .Subscribe(_ => ChangeFilter());

        IObservable<bool> canAddTodo =
            this.WhenAnyValue(o => o.TitleNewTodo, title => !string.IsNullOrWhiteSpace(title));

        AddTodoCommand = ReactiveCommand.Create(AddTodo, canAddTodo);
        RemoveCheckedTodosCommand = ReactiveCommand.Create(RemoveCheckedTodos);
        RemoveAllTodosCommand = ReactiveCommand.Create(RemoveAllTodos);
    }

    private ObservableCollection<TodoItem> _todoList;
    private string? _titleNewTodo = string.Empty;
    private DateTime? _dateDeadlineNewTodo;
    private Priority _priorityNewTodo = Priority.Medium;
    private string _sortingSelected = "Select Sorting Method...";
    private string _filterSelected = "Select Filter...";
    private readonly IService _todoService;


    public TodoItemViewModel TodoItemViewModel { get; set; }
    public ICommand? AddTodoCommand { get; set; }
    public ICommand? RemoveCheckedTodosCommand { get; set; }
    public ICommand? RemoveAllTodosCommand { get; set; }
    public Priority[] Priorities { get; } = Enum.GetValues(typeof(Priority)).Cast<Priority>().ToArray();
    public string DateTimeWatermark { get; set; } = DateTime.Now.ToString("d MMM yyyy");

    private ObservableCollection<TodoItem> TodoList
    {
        get => _todoList;
        set => this.RaiseAndSetIfChanged(ref _todoList, value);
    }
    public string? TitleNewTodo
    {
        get => _titleNewTodo;
        set => this.RaiseAndSetIfChanged(ref _titleNewTodo, value);
    }

    public DateTime? DateDeadlineNewTodo
    {
        get => _dateDeadlineNewTodo;
        set => this.RaiseAndSetIfChanged(ref _dateDeadlineNewTodo, value);
    }

    public Priority PriorityNewTodo
    {
        get => _priorityNewTodo;
        set => this.RaiseAndSetIfChanged(ref _priorityNewTodo, value);
    }

    public string SortingSelected
    {
        get => _sortingSelected;
        set => this.RaiseAndSetIfChanged(ref _sortingSelected, value);
    }

    public string FilterSelected
    {
        get => _filterSelected;
        set => this.RaiseAndSetIfChanged(ref _filterSelected, value);
    }

    public ObservableCollection<string> SortingOptions { get; }
        =
        [
            "Priority Ascending", "Priority Descending", "Alphabetic Ascending", "Alphabetic Descending", "Completed Todos Last", "Completed Todos First",
            "Deadline Ascending", "Deadline Descending", "Select Sorting Method..."
        ];

    public ObservableCollection<string> FilterOptions { get; }
        =
        [
            "Not Checked", "Low Priority", "Medium Priority", "High Priority", "Deadline Today",
            "Deadline Within 3 Days", "Deadline Beyond 3 Days", "Select Filter..."
        ];

    private void AddTodo()
    {
        TodoItem todoItem = new TodoItem()
        {
            Title = TitleNewTodo, Priority = PriorityNewTodo, DateDeadline = DateDeadlineNewTodo
        };

        TodoList.Add(todoItem);
        _todoService.AddToTodoList(todoItem);

        TitleNewTodo = "";
        PriorityNewTodo = Priority.Medium;
        DateDeadlineNewTodo = null;

        ChangeFilter();
        ChangeSorting();
    }

    /// <summary>
    /// Remove all checked todoItems by first adding the item to the archive located in the service,
    /// then removing it from the TodoList in the service and then removing it from the TodoList.
    /// </summary>
    private void RemoveCheckedTodos()
    {
        // can't use foreach for deletion
        for (var i = TodoList.Count() - 1; i >= 0; i--)
        {
            if (TodoList[i].IsChecked)
            {
                _todoService.AddToArchive(TodoList[i]);
                _todoService.RemoveFromTodoList(TodoList[i]);
                TodoList.Remove(TodoList[i]);
            }
        }
    }

    /// <summary>
    /// Remove all todoItems from the TodoList.
    /// </summary>
    private void RemoveAllTodos()
    {
        _todoService.RemoveAllFromTodoList();
        TodoList.Clear();
    }

    /// <summary>
    /// Filter and sort on an TodoEdited event raised by the ItemViewModel.
    /// </summary>
    private void OnTodoEdited(object? o, EventArgs  e)
    {
        ChangeFilter();
        ChangeSorting();
    }

    /// <summary>
    /// Delete todoItem, filter and sort on an TodoDeleted event raised by the ItemViewModel.
    /// </summary>
    private void OnTodoDeleted(object? o, TodoItemEventArgs e)
    {
        _todoService.RemoveFromTodoList(e.Todo);
        TodoList.Remove(e.Todo);
        ChangeFilter();
        ChangeSorting();
    }

    /// <summary>
    /// If the Service raises a TodoListChanged event, reload the TodoList from the service
    /// and set the TodoList to the new TodoList for the ItemViewModel.
    /// </summary>
    private void OnTodoListChanged(object? o, EventArgs e)
    {
        TodoList = new ObservableCollection<TodoItem>(_todoService.GetTodoList());
        TodoItemViewModel.TodoList = _todoList;
    }

    private void ChangeFilter()
    {
        if (FilterSelected != "Select Filter...")
        {
            FilterOptions.Remove("Select Filter...");

            if (!FilterOptions.Contains("Clear Filter"))
            {
                FilterOptions.Add("Clear Filter");
            }
        }

        switch (FilterSelected)
        {
            case "Select Filter...":
                break;

            case "Not Checked":
                FilterChecked(false);
                break;

            case "Low Priority":
                FilterPriority(Priority.Low);
                break;

            case "Medium Priority":
                FilterPriority(Priority.Medium);
                break;

            case "High Priority":
                FilterPriority(Priority.High);
                break;

            case "Clear Filter":
                foreach (var item in TodoList)
                {
                    item.IsShown = true;
                }

                FilterOptions.Add("Select Filter...");
                FilterSelected = "Select Filter...";
                FilterOptions.Remove("Clear Filter");
                break;

            case "Deadline Today":
                FilterDeadline(true, false, false);
                break;

            case "Deadline Within 3 Days":
                FilterDeadline(false, true, false);
                break;

            case "Deadline Beyond 3 Days":
                FilterDeadline(false, false, true);
                break;

            default:
                Console.WriteLine($"Can't filter this: { FilterSelected }");
                break;
        }
    }

    private void FilterChecked(bool include)
    {
        foreach (var item in TodoList)
        {
            item.IsShown = item.IsChecked ? include : !include;
        }
    }

    private void FilterPriority(Priority priority)
    {
        foreach (var item in TodoList)
        {
            item.IsShown = item.Priority == priority;
        }
    }

    private void FilterDeadline(bool zeroDays, bool threeDays, bool overThreeDays)
    {

        DateTime now = (DateTime.Now).Date;
        TimeSpan delta;
        TimeSpan zeroDaysTime = new TimeSpan(0, 0, 0, 0);
        TimeSpan threeDaysTime = new TimeSpan(3, 0, 0, 0);

        foreach (var item in TodoList)
        {
            item.IsShown = false;

            if (item.DateDeadline == null)
            {
                continue;
            }

            delta = ((DateTime)item.DateDeadline).Date.Subtract(now);

            if (zeroDays && delta.Days == zeroDaysTime.Days)
            {
                item.IsShown = true;
            }
            else if (threeDays && delta.Days >= zeroDaysTime.Days && delta.Days <= threeDaysTime.Days)
            {
                item.IsShown = true;
            }
            else if (overThreeDays && delta.Days > threeDaysTime.Days)
            {
                item.IsShown = true;
            }
        }
    }

    private void ChangeSorting()
    {
        var sortedTodos = new List<TodoItem>(TodoList);

        if (SortingSelected != "Select Sorting Method...")
        {
                SortingOptions.Remove("Select Sorting Method...");

                if (!SortingOptions.Contains("Reset Sorting"))
                {
                    SortingOptions.Add("Reset Sorting");
                }
        }

        switch (SortingSelected)
        {
            case "Select Sorting Method...":
                SortingOptions.Remove("Reset Sorting");
                break;

            case "Priority Ascending":
                sortedTodos.Sort((x, y) => x.Priority.CompareTo(y.Priority));
                break;

            case "Priority Descending":
                sortedTodos.Sort((x, y) => y.Priority.CompareTo(x.Priority));
                break;

            case "Alphabetic Ascending":
                sortedTodos.Sort((x, y) => String.Compare(x.Title, y.Title, StringComparison.Ordinal));
                break;

            case "Alphabetic Descending":
                sortedTodos.Sort((x, y) => String.Compare(y.Title, x.Title, StringComparison.Ordinal));
                break;

            case "Completed Todos Last":
                sortedTodos.Sort((x ,y) => x.IsChecked.CompareTo(y.IsChecked));
                break;

            case "Completed Todos First":
                sortedTodos.Sort((x ,y) => y.IsChecked.CompareTo(x.IsChecked));
                break;

            case "Deadline Ascending":
                sortedTodos.Sort((x, y) => Nullable.Compare(x.DateDeadline, y.DateDeadline));
                break;

            case "Deadline Descending":
                sortedTodos.Sort((x, y) => Nullable.Compare(y.DateDeadline, x.DateDeadline));
                break;

            case "Reset Sorting":
                SortingOptions.Add("Select Sorting Method...");
                SortingSelected = "Select Sorting Method...";
                SortingOptions.Remove("Reset Sorting");

                sortedTodos.Sort((x, y) => x.DateTimeAdded.CompareTo(y.DateTimeAdded));
                break;

            default:
                Console.WriteLine($"Can't sort this: { SortingSelected }");
                break;
        }

        TodoList.Clear();

        foreach (var todo in sortedTodos)
        {
             TodoList.Add(todo);
        }
    }

    public override void LoadViewModel()
    {
        if (SortingSelected != "Select Sorting Method...")
        {
            SortingSelected = "Reset Sorting";
            ChangeSorting();
        }

        if (FilterSelected != "Select Filter...")
        {
            FilterSelected = "Clear Filter";
            ChangeFilter();
        }
    }
}