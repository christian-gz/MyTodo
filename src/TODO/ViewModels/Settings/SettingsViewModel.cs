using System;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
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

        SelectedFileTodoCsv = _settingsManager.GetTodoCsvPath();
        SelectedFileArchiveCsv = _settingsManager.GetArchiveCsvPath();

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
    private bool _shareCsvLocation;
    private string? _selectedFolderTodoCsv;
    private string? _selectedFolderArchiveCsv;
    private string? _selectedFileTodoCsv;
    private string? _selectedFileArchiveCsv;

    private string? _selectedFolderTodoCsvInfo;
    private string? _selectedFolderArchiveCsvInfo;
    private string? _selectedFileTodoCsvInfo;
    private string? _selectedFileArchiveCsvInfo;

    private string? _generalSettingsInfo;
    private int _generalSettingsInfoDisplayTimes;
    private string? _generalSettingsError;
    private int _generalSettingsErrorDisplayTimes;

    public Interaction<string?, string?> SelectFolderInteraction => this._selectFolderInteraction;
    public Interaction<string?, string?> SelectFileInteraction => this._selectFileInteraction;

    public ICommand SwitchShareCsvLocationCommand { get; }
    public ICommand SelectFolderTodoCsvCommand { get; }
    public ICommand SelectFolderArchiveCsvCommand { get; }
    public ICommand SelectFileTodoCsvCommand { get; }
    public ICommand SelectFileArchiveCsvCommand { get; }

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
            if (_selectedFolderTodoCsv != value && _settingsManager.UpdateTodoCsvLocation(value, nameof(SelectedFolderTodoCsv)))
            {
                // use the private fields to not call another UpdateTodoCsv
                _selectedFileTodoCsv = _settingsManager.GetTodoCsvPath();
                this.RaiseAndSetIfChanged(ref _selectedFolderTodoCsv, value);

                if (ShareCsvLocation)
                {
                    SelectedFolderArchiveCsv = _selectedFolderTodoCsv;
                }
            }
        }
    }
    public string? SelectedFolderArchiveCsv
    {
        get => _selectedFolderArchiveCsv;
        set
        {
            if (_selectedFolderArchiveCsv != value && _settingsManager.UpdateArchiveCsvLocation(value, nameof(SelectedFolderArchiveCsv)))
            {
                // use the private fields to not call another UpdateArchiveCsv
                _selectedFileArchiveCsv = _settingsManager.GetArchiveCsvPath();
                this.RaiseAndSetIfChanged(ref _selectedFolderArchiveCsv, value);
            }
        }
    }
    public string? SelectedFileTodoCsv
    {
        get => _selectedFileTodoCsv;
        set
        {
            if (_settingsManager.UpdateTodoCsvPath(value, nameof(SelectedFileTodoCsv)))
            {
                _selectedFolderTodoCsv = Path.GetDirectoryName(value);

                this.RaiseAndSetIfChanged(ref _selectedFileTodoCsv, value);

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
            if (_settingsManager.UpdateArchiveCsvPath(value, nameof(SelectedFileArchiveCsv)))
            {
                _selectedFolderArchiveCsv = Path.GetDirectoryName(value);

                this.RaiseAndSetIfChanged(ref _selectedFileArchiveCsv, value);

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
        set
        {
            _generalSettingsInfoDisplayTimes = 1;
            this.RaiseAndSetIfChanged(ref _generalSettingsInfo, value);
        }
    }
    public string? GeneralSettingsError
    {
        get => _generalSettingsError;
        set
        {
            _generalSettingsErrorDisplayTimes = 1;
            this.RaiseAndSetIfChanged(ref _generalSettingsError, value);
        }
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
        if (e.EventType == EventType.Service)
        {
            if (e.Message == null)
                return;

            ServiceMessage message = (ServiceMessage)e.Message;

            if (string.IsNullOrEmpty(message.PropertyName))
            {
                if (e.IsSuccessful)
                {
                    GeneralSettingsInfo += "\n" + message.Message;
                }
                else
                {
                    GeneralSettingsError += "\n" + message.Message;
                }
                return;
            }

            string errorPropertyName = $"{message.PropertyName}Info";

            PropertyInfo? propertyInfo =
                GetType().GetProperty(errorPropertyName, BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(this, message.Message);
            }
            else
            {
                Console.WriteLine($"Cant find property { errorPropertyName } or can't write to it");
            }
        }
        else if (e.EventType == EventType.Settings)
        {
            if (e.Message == null)
                return;

            SettingsMessage message = (SettingsMessage)e.Message;

            if (e.IsSuccessful)
            {
                GeneralSettingsInfo += "\n" + message.Message;
            }
            else
            {
                GeneralSettingsError += "\n" + message.Message;
            }
            return;
        }
    }

    public override void LoadViewModel()
    {
        SelectedFolderTodoCsvInfo = "";
        SelectedFolderArchiveCsvInfo = "";
        SelectedFileTodoCsvInfo = "";
        SelectedFileArchiveCsvInfo = "";

        if (_generalSettingsInfoDisplayTimes > 0)
        {
            _generalSettingsInfoDisplayTimes--;
        }
        else
        {
            GeneralSettingsInfo = "";
        }

        if (_generalSettingsErrorDisplayTimes > 0)
        {
            _generalSettingsErrorDisplayTimes--;
        }
        else
        {
            GeneralSettingsError = "";
        }
    }
}