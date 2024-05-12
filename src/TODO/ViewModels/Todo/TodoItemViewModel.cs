using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;
using TODO.Models;

namespace TODO.ViewModels.Todo;

public class TodoItemViewModel : ViewModelBase
{
    public TodoItemViewModel()
    {
        TodoList = new ObservableCollection<TodoItem>()
        {
            new TodoItem() { Title = "Do something", Priority = Priority.Medium },
            new TodoItem() { Title = "Buy some milk", Priority = Priority.Low, IsChecked = true },
            new TodoItem() { Title = "Do something else", Priority = Priority.High },
            new TodoItem() { Title = "Go on a walk", Priority = Priority.Low, IsEdited = true}
        };
    }

    public TodoItemViewModel(ObservableCollection<TodoItem> todoList, Priority[] priorities)
    {
        TodoList = todoList;
        Priorities = priorities;

        IObservable<bool> canEditTodoCommand =
            this.WhenAnyValue(item => item.TodoIsEdited, edited => !edited);
        IObservable<bool> canConfirmTodoCommand =
            this.WhenAnyValue(item => item.TodoIsEdited, edited => !!edited);

        EditTodoCommand = ReactiveCommand.Create<TodoItem>(entry => EditTodo(entry), canEditTodoCommand);
        DeleteTodoCommand = ReactiveCommand.Create<TodoItem>(entry => DeleteTodo(entry), canConfirmTodoCommand);
        ConfirmEditTodoCommand = ReactiveCommand.Create<TodoItem>(entry => EditTodo(entry), canConfirmTodoCommand);
        CompleteTodoCommand = ReactiveCommand.Create<TodoItem>(entry => CompleteTodo(entry), canEditTodoCommand);
    }

    private bool _todoIsEdited;

    public ObservableCollection<TodoItem> TodoList { get; set; }
    public Priority[] Priorities { get; }
    public ICommand EditTodoCommand { get; set; }
    public ICommand DeleteTodoCommand { get; set; }
    public ICommand ConfirmEditTodoCommand { get; set; }
    public ICommand CompleteTodoCommand { get; set; }

    public string DateTimeWatermark { get; set; } = DateTime.Now.ToString("d MMM yyyy");

    /// <summary>
    /// Event to signal, that an TodoItem has been edited.
    /// </summary>
    public static event EventHandler TodoEdited;

    /// <summary>
    /// Event to signal, that an TodoItem has been deleted.
    /// </summary>
    public static event EventHandler<TodoItemEventArgs> TodoDeleted;

    private bool TodoIsEdited
    {
        get => _todoIsEdited;
        set => this.RaiseAndSetIfChanged(ref _todoIsEdited, value);
    }

    /// <summary>
    /// Start or stop editing todoItem, if stopped raise an event for the ListViewModel to sort and filter.
    /// Only one todoItem can be edited at once.
    /// </summary>
    /// <param name="todoItem"> The item to set the IsEdited property. </param>
    private void EditTodo(TodoItem todoItem)
    {
        if (!TodoIsEdited && !todoItem.IsEdited)
        {
            TodoIsEdited = true;
            todoItem.IsEdited = !todoItem.IsEdited;
        }
        else if (TodoIsEdited && todoItem.IsEdited)
        {
            TodoIsEdited = false;
            todoItem.IsEdited = !todoItem.IsEdited;

            TodoEdited?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// The todoItem has to be deleted. Raise a ToDdoDeleted for the ListViewModel to delete it.
    /// </summary>
    /// <param name="todoItem"> TodoItem to be deleted. </param>
    private void DeleteTodo(TodoItem todoItem)
    {
        TodoDeleted?.Invoke(this, new TodoItemEventArgs(todoItem));
        TodoIsEdited = false;
    }

    /// <summary>
    /// Switch the IsChecked property of the todoItem, if checked set the datetime.
    /// Raise a TodoEdited event for the ListViewModel to sort and filter.
    /// </summary>
    /// <param name="todoItem"></param>
    private void CompleteTodo(TodoItem todoItem)
    {
        if (!todoItem.IsChecked)
        {
            todoItem.IsChecked = true;
            todoItem.DateFinishedAt = DateTime.Now;
        }
        else
        {
            todoItem.IsChecked = false;
            todoItem.DateFinishedAt = null;
        }

        TodoEdited?.Invoke(this, EventArgs.Empty);
    }
}