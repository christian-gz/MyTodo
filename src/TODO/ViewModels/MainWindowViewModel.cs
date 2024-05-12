using System;
using System.Collections.Generic;
using System.Windows.Input;
using ReactiveUI;
using TODO.Configuration;
using TODO.Events;
using TODO.Services;
using TODO.ViewModels.Archive;
using TODO.ViewModels.Settings;
using TODO.ViewModels.Stats;
using TODO.ViewModels.Todo;

namespace TODO.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        // setup a list to save ApplicationEvents that are raised during initialisation
        List<ApplicationEventArgs> eventArgsList = new();
        EventHandler<ApplicationEventArgs> collectApplicationEvents = (o, e) => eventArgsList.Add(e);
        EventManager.ApplicationEvent += collectApplicationEvents;

        TodoListService service = new TodoListService();
        _todoListService = service;

        SettingsManager settingsManager = new SettingsManager(service);

        _todoListView = new TodoListViewModel(service);
        _archiveView = new ArchiveViewModel(service);
        _statsView = new StatsViewModel(service);
        _settingsView = new SettingsViewModel(settingsManager);

        _currentViewModel = _todoListView;

        NavigateTodoListCommand = ReactiveCommand.Create(NavigateTodoList);
        NavigateArchiveCommand = ReactiveCommand.Create(NavigateArchive);
        NavigateStatsCommand = ReactiveCommand.Create(NavigateStats);
        NavigateSettingsCommand = ReactiveCommand.Create(NavigateSettings);

        this.WhenAnyValue(o => o.CurrentViewModel)
            .Subscribe(o =>
            {
                this.RaisePropertyChanged(nameof(ViewOneActive));
                this.RaisePropertyChanged(nameof(ViewTwoActive));
                this.RaisePropertyChanged(nameof(ViewThreeActive));
                this.RaisePropertyChanged(nameof(ViewFourActive));
            });

        // re raise all ApplicationEvents now that the application is ready to handle them
        EventManager.ApplicationEvent -= collectApplicationEvents;
        foreach (var eventArg in eventArgsList)
        {
            EventManager.RaiseEvent(eventArg);
        }
        eventArgsList.Clear();
    }

    private readonly SwappableViewModelBase _todoListView;
    private readonly SwappableViewModelBase _archiveView;
    private readonly SwappableViewModelBase _statsView;
    private readonly SwappableViewModelBase _settingsView;
    private readonly TodoListService _todoListService;
    private SwappableViewModelBase _currentViewModel;
    public SwappableViewModelBase ArchiveView => _archiveView;

    public ICommand NavigateTodoListCommand { get; set; }
    public ICommand NavigateArchiveCommand { get; set; }
    public ICommand NavigateStatsCommand { get; set; }
    public ICommand NavigateSettingsCommand { get; set; }

    public SwappableViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    private void NavigateTodoList()
    {
        CurrentViewModel = _todoListView;
        _todoListView.LoadViewModel();
    }
    private void NavigateArchive()
    {
        CurrentViewModel = _archiveView;
        _archiveView.LoadViewModel();
    }
    private void NavigateStats()
    {
        CurrentViewModel = _statsView;
        _statsView.LoadViewModel();
    }
    private void NavigateSettings()
    {
        CurrentViewModel = _settingsView;
        _settingsView.LoadViewModel();
    }

    public void OnClosing()
    {
        _todoListService.OnClosing();
    }

    public bool ViewOneActive => CurrentViewModel == _todoListView;
    public bool ViewTwoActive => CurrentViewModel == _archiveView;
    public bool ViewThreeActive => CurrentViewModel == _statsView;
    public bool ViewFourActive => CurrentViewModel == _settingsView;
}