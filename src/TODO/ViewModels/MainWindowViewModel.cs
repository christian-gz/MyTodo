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
    public MainWindowViewModel(SettingsManager settingsManager, IService service)
    {
        _todoListService = service;

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
            .Subscribe(o => RaiseActiveViewPropertiesChanged());

        EventManager.ApplicationEvent += OnApplicationEvent;
    }

    private readonly SwappableViewModelBase _todoListView;
    private readonly SwappableViewModelBase _archiveView;
    private readonly SwappableViewModelBase _statsView;
    private readonly SwappableViewModelBase _settingsView;
    private readonly IService _todoListService;
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

    private void RaiseActiveViewPropertiesChanged()
    {
        this.RaisePropertyChanged(nameof(ViewOneActive));
        this.RaisePropertyChanged(nameof(ViewTwoActive));
        this.RaisePropertyChanged(nameof(ViewThreeActive));
        this.RaisePropertyChanged(nameof(ViewFourActive));
    }

    /// <summary>
    /// Checks if the ApplicationEvent is about the Theme of the application and
    /// in that case causes the converters for the menu bar colors to re-evaluate.
    /// </summary>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="e"> Contains event data including the type of the event
    /// and relevant messages. </param>
    private void OnApplicationEvent(object? sender, ApplicationEventArgs e)
    {
        switch (e.EventType)
        {
            case EventType.Settings:
                if (e.Message == null)
                    return;

                SettingsMessage settingsMessage = (SettingsMessage)e.Message;

                if (!string.IsNullOrWhiteSpace(settingsMessage.PropertyName))
                {
                    if (settingsMessage.PropertyName == "DarkModeEnabled")
                    {
                        RaiseActiveViewPropertiesChanged();
                    }
                }
                break;
        }
    }

    public bool ViewOneActive => CurrentViewModel == _todoListView;
    public bool ViewTwoActive => CurrentViewModel == _archiveView;
    public bool ViewThreeActive => CurrentViewModel == _statsView;
    public bool ViewFourActive => CurrentViewModel == _settingsView;
}