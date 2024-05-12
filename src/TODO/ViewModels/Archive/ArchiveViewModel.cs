using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using TODO.Models;
using TODO.Services;

namespace TODO.ViewModels.Archive;

public class ArchiveViewModel : SwappableViewModelBase
{
    public ArchiveViewModel()
    {
        Archive = new ObservableCollection<TodoItem>()
        {
            new TodoItem() { Title = "Do something", Priority = Priority.Medium, IsChecked = true,
                             DateDeadline = DateTime.Now.Add(new TimeSpan(1, 1, 1)),
                             DateFinishedAt = DateTime.Now},
            new TodoItem() { Title = "Go on a walk", Priority = Priority.Low, IsChecked = true },
            new TodoItem() { Title = "Do something else", Priority = Priority.High }
        };
    }
    public ArchiveViewModel(IService todoService)
    {
        _todoService = todoService;

        IObservable<bool> canDeleteSelectedTodoCommand = this
            .WhenAnyValue(o => o.SelectedTodo)
            .Select(todo => todo != null);

        ShowDialog = new Interaction<ConfirmTodoDeletionViewModel, bool>();
        DeleteSelectedTodoCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var deleteDialog = new ConfirmTodoDeletionViewModel();
            var result = await ShowDialog.Handle(deleteDialog);
            if (result)
            {
                DeleteSelectedTodo();
            }
        }, canDeleteSelectedTodoCommand);
    }

    private readonly IService _todoService;
    private ObservableCollection<TodoItem> _archive;
    private TodoItem? _selectedTodo;

    public Interaction<ConfirmTodoDeletionViewModel, bool> ShowDialog { get; }
    public ICommand DeleteSelectedTodoCommand { get; set; }
    public TodoItem? SelectedTodo
    {
        get => _selectedTodo;
        set => this.RaiseAndSetIfChanged(ref _selectedTodo, value);
    }
    public ObservableCollection<TodoItem> Archive
    {
        get => _archive;
        set => this.RaiseAndSetIfChanged(ref _archive, value);
    }

    public void DeleteSelectedTodo()
    {
        if (SelectedTodo != null)
        {
            _todoService.RemoveFromArchive(SelectedTodo);
            Archive.Remove(SelectedTodo);
            SelectedTodo = null;
        }
    }
    public override void LoadViewModel()
    {
        Archive = new ObservableCollection<TodoItem>(_todoService.GetArchive());
    }
}