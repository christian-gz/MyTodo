using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Styling;
using ReactiveUI;
using TODO.Configuration;
using TODO.Events;

namespace TODO.ViewModels.Settings;

public class SettingsViewModel : SwappableViewModelBase
{
    public SettingsViewModel() { }
    public SettingsViewModel(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;

        _darkModeEnabled = _settingsManager.GetDarkModeEnabled();
        _selectedFileTodoCsv = _settingsManager.GetTodoCsvPath();
        _selectedFileArchiveCsv = _settingsManager.GetArchiveCsvPath();
        _selectedFolderTodoCsv = Path.GetDirectoryName(_selectedFileTodoCsv);
        _selectedFolderArchiveCsv = Path.GetDirectoryName(_selectedFileArchiveCsv);

        EventManager.ApplicationEvent += OnApplicationEvent;

        _selectFolderInteraction = new Interaction<string, string>();
        _selectFileInteraction = new Interaction<string, string>();

        SelectFolderTodoCsvCommand = ReactiveCommand.CreateFromTask(SelectFolderTodoCsv);
        SelectFolderArchiveCsvCommand = ReactiveCommand.CreateFromTask(SelectFolderArchiveCsv);
        SelectFileTodoCsvCommand = ReactiveCommand.CreateFromTask(SelectFileTodoCsv);
        SelectFileArchiveCsvCommand = ReactiveCommand.CreateFromTask(SelectFileArchiveCsv);

        SwitchShareCsvLocationCommand = ReactiveCommand.Create(SwitchShareCsvLocation);

        // use the private fields to not call another UpdateTodoCsv and UpdateArchiveCsv
        this.WhenAnyValue(o => o.SelectedFileTodoCsv)
            .Subscribe(new Action<string?>(o => this.RaisePropertyChanged(nameof(SelectedFolderTodoCsv))));
        this.WhenAnyValue(o => o.SelectedFileArchiveCsv)
            .Subscribe(new Action<string?>(o => this.RaisePropertyChanged(nameof(SelectedFolderArchiveCsv))));
        this.WhenAnyValue(o => o.SelectedFolderTodoCsv)
            .Subscribe(new Action<string?>(o => this.RaisePropertyChanged(nameof(SelectedFileTodoCsv))));
        this.WhenAnyValue(o => o.SelectedFolderArchiveCsv)
            .Subscribe(new Action<string?>(o => this.RaisePropertyChanged(nameof(SelectedFileArchiveCsv))));
    }

    private readonly SettingsManager _settingsManager;
    private readonly Interaction<string?, string?> _selectFolderInteraction;
    private readonly Interaction<string?, string?> _selectFileInteraction;

    private bool _darkModeEnabled;

    private bool _shareCsvLocation;

    private string? _selectedFolderTodoCsv;
    private string? _selectedFolderArchiveCsv;
    private string? _selectedFileTodoCsv;
    private string? _selectedFileArchiveCsv;

    private string? _selectedFolderTodoCsvInfo;
    private string? _selectedFolderArchiveCsvInfo;
    private string? _selectedFileTodoCsvInfo;
    private string? _selectedFileArchiveCsvInfo;

    private bool _firstVisit = true;

    private string? _generalSettingsInfo;
    private string? _generalSettingsError;

    public Interaction<string?, string?> SelectFolderInteraction => this._selectFolderInteraction;
    public Interaction<string?, string?> SelectFileInteraction => this._selectFileInteraction;

    public ICommand SwitchShareCsvLocationCommand { get; }
    public ICommand SelectFolderTodoCsvCommand { get; }
    public ICommand SelectFolderArchiveCsvCommand { get; }
    public ICommand SelectFileTodoCsvCommand { get; }
    public ICommand SelectFileArchiveCsvCommand { get; }

    public bool DarkModeEnabled
    {
        get => _darkModeEnabled;
        set
        {
            if (_darkModeEnabled != value)
            {
                this.RaiseAndSetIfChanged(ref _darkModeEnabled, value);

                Application? app = Application.Current;

                if (app != null)
                {
                    if (DarkModeEnabled)
                    {
                        app.RequestedThemeVariant = ThemeVariant.Dark;
                    }
                    else
                    {
                        app.RequestedThemeVariant = ThemeVariant.Light;
                    }

                    _settingsManager.UpdateDarkModeEnabled(DarkModeEnabled);
                }
            }
        }
    }

    public bool ShareCsvLocation
    {
        get => _shareCsvLocation;
        set => this.RaiseAndSetIfChanged(ref _shareCsvLocation, value);
    }
    public string? SelectedFolderTodoCsv
    {
        get => _selectedFolderTodoCsv;
        set
        {
            if (_selectedFolderTodoCsv != value)
            {
                 _settingsManager.UpdateTodoCsvLocation(value);

                string currentFileTodoCsvPath = _settingsManager.GetTodoCsvPath();
                // use the private field to not call another UpdateTodoCsv
                _selectedFileTodoCsv = currentFileTodoCsvPath;
                this.RaiseAndSetIfChanged(ref _selectedFolderTodoCsv, Path.GetDirectoryName(currentFileTodoCsvPath));

                if (ShareCsvLocation)
                {
                    SelectedFolderArchiveCsv = value;
                }
            }
        }
    }
    public string? SelectedFolderArchiveCsv
    {
        get => _selectedFolderArchiveCsv;
        set
        {
            if (_selectedFolderArchiveCsv != value)
            {
                Console.WriteLine("test");
                 _settingsManager.UpdateArchiveCsvLocation(value);

                string currentFileArchiveCsvPath = _settingsManager.GetArchiveCsvPath();
                // use the private field to not call another UpdateArchiveCsv
                _selectedFileArchiveCsv = currentFileArchiveCsvPath;
                this.RaiseAndSetIfChanged(ref _selectedFolderArchiveCsv, Path.GetDirectoryName(currentFileArchiveCsvPath));
            }
        }
    }
    public string? SelectedFileTodoCsv
    {
        get => _selectedFileTodoCsv;
        set
        {
            if (_selectedFileTodoCsv != value)
            {
                _settingsManager.UpdateTodoCsvPath(value);

                string currentFileTodoCsvPath = _settingsManager.GetTodoCsvPath();
                // use the private field to not call another UpdateTodoCsv
                _selectedFolderTodoCsv = Path.GetDirectoryName(currentFileTodoCsvPath);
                this.RaiseAndSetIfChanged(ref _selectedFileTodoCsv, currentFileTodoCsvPath);

                if (ShareCsvLocation && _selectedFolderTodoCsv != _selectedFolderArchiveCsv)
                    ShareCsvLocation = false;
            }
        }
    }
    public string? SelectedFileArchiveCsv
    {
        get => _selectedFileArchiveCsv;
        set
        {
            if (_selectedFileArchiveCsv != value)
            {
                _settingsManager.UpdateArchiveCsvPath(value);

                string currentFileArchiveCsvPath = _settingsManager.GetArchiveCsvPath();
                // use the private field to not call another UpdateArchiveCsv
                _selectedFolderArchiveCsv = Path.GetDirectoryName(currentFileArchiveCsvPath);
                this.RaiseAndSetIfChanged(ref _selectedFileArchiveCsv, currentFileArchiveCsvPath);

                if (ShareCsvLocation && _selectedFolderTodoCsv != _selectedFolderArchiveCsv)
                    ShareCsvLocation = false;
            }
        }
    }

    public string? SelectedFolderTodoCsvInfo
    {
        get => _selectedFolderTodoCsvInfo;
        set => this.RaiseAndSetIfChanged(ref _selectedFolderTodoCsvInfo, value);
    }
    public string? SelectedFolderArchiveCsvInfo
    {
        get => _selectedFolderArchiveCsvInfo;
        set => this.RaiseAndSetIfChanged(ref _selectedFolderArchiveCsvInfo, value);
    }
    public string? SelectedFileTodoCsvInfo
    {
        get => _selectedFileTodoCsvInfo;
        set => this.RaiseAndSetIfChanged(ref _selectedFileTodoCsvInfo, value);
    }
    public string? SelectedFileArchiveCsvInfo
    {
        get => _selectedFileArchiveCsvInfo;
        set => this.RaiseAndSetIfChanged(ref _selectedFileArchiveCsvInfo, value);
    }

    public string? GeneralSettingsInfo
    {
        get => _generalSettingsInfo;
        set => this.RaiseAndSetIfChanged(ref _generalSettingsInfo, value);
    }
    public string? GeneralSettingsError
    {
        get => _generalSettingsError;
        set => this.RaiseAndSetIfChanged(ref _generalSettingsError, value);
    }

    private void SwitchShareCsvLocation()
    {
        if (ShareCsvLocation)
        {
            ShareCsvLocation = false;
        }
        else
        {
            SelectedFolderArchiveCsv = SelectedFolderTodoCsv;

            ShareCsvLocation = true;
        }
    }
    private async Task SelectFolderTodoCsv()
    {
        SelectedFolderTodoCsv = await _selectFolderInteraction.Handle("Please select a location to store the todos.");
    }
    private async Task SelectFolderArchiveCsv()
    {
        SelectedFolderArchiveCsv = await _selectFolderInteraction.Handle("Please select a location to store the archive.");
    }
    private async Task SelectFileTodoCsv()
    {
        SelectedFileTodoCsv = await _selectFileInteraction.Handle("Please select a CSV file to store the todos.");
    }
    private async Task SelectFileArchiveCsv()
    {
        SelectedFileArchiveCsv = await _selectFileInteraction.Handle("Please select a CSV file to store the archive.");
    }

    private void OnApplicationEvent(object? o, ApplicationEventArgs e)
    {
        switch (e.EventType)
        {
            case EventType.Service:
                if (e.Message == null)
                    return;

                ServiceMessage serviceMessage = (ServiceMessage)e.Message;

                if (e.IsSuccessful)
                {
                    GeneralSettingsInfo += "\n" + serviceMessage.Message;
                }
                else
                {
                    GeneralSettingsError += "\n" + serviceMessage.Message;
                }
                break;
            case EventType.Settings:
                if (e.Message == null)
                    return;

                SettingsMessage settingsMessage = (SettingsMessage)e.Message;

                if (!string.IsNullOrWhiteSpace(settingsMessage.PropertyName))
                {
                    switch (settingsMessage.PropertyName)
                    {
                        case "TodoCsvPath":
                            SelectedFileTodoCsvInfo = settingsMessage.Message;
                            SelectedFolderTodoCsvInfo = settingsMessage.Message;

                            break;
                        case "ArchiveCsvPath":
                            SelectedFileArchiveCsvInfo = settingsMessage.Message;
                            SelectedFolderArchiveCsvInfo = settingsMessage.Message;

                            break;
                    }

                    return;
                }

                if (e.IsSuccessful)
                {
                    GeneralSettingsInfo += "\n" + settingsMessage.Message;
                }
                else
                {
                    GeneralSettingsError += "\n" + settingsMessage.Message;
                }
                break;
        }
    }

    public override void LoadViewModel()
    {
        if (!_firstVisit)
        {
            SelectedFolderTodoCsvInfo = "";
            SelectedFolderArchiveCsvInfo = "";
            SelectedFileTodoCsvInfo = "";
            SelectedFileArchiveCsvInfo = "";

            GeneralSettingsError = "";
            GeneralSettingsInfo = "";

            _settingsManager.CheckSettingsStatus();
        }
        else
        {
            _firstVisit = false;
        }
    }
}