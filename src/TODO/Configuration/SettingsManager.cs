using System;
using System.IO;
using System.Text.Json;
using TODO.Events;
using TODO.Services;

namespace TODO.Configuration;

public class SettingsManager
{
    public SettingsManager(IService todoService)
    {
        _todoService = todoService;
        LoadSettings();

        UpdateTodoCsvPath(_appSettings!.TodoCsvPath, null);
        UpdateArchiveCsvPath(_appSettings!.ArchiveCsvPath, null);
    }

    private const string _filePath = "appsettings.json";
    private AppSettings _appSettings;
    private readonly IService _todoService;

    public bool SettingsLoaded { get; set; }

    /// <summary>
    /// Write the AppSettings to appsettings.json.
    /// </summary>
    private bool WriteSettingsToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_appSettings);
            File.WriteAllText(_filePath, json);

            return true;
        }
        catch (Exception ex)
        {
            EventManager.RaiseEvent(
                EventType.Settings,
                false,
                new SettingsMessage($"Can't write to the settings file. Settings can't be saved.\n" +
                                    $"{ ex.Message }")
            );

            return false;
        }
    }

    /// <summary>
    /// Load the AppSettings from the appsettings.json file.
    /// If no file exists, use the default settings and create the file.
    /// </summary>
    private void LoadSettings()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                string json = File.ReadAllText(_filePath);
                _appSettings = JsonSerializer.Deserialize<AppSettings>(json);

                EventManager.RaiseEvent(
                    EventType.Settings,
                    true,
                    new SettingsMessage($"Successfully loaded the settings.")
                );

                SettingsLoaded = true;
                return;
            }
            catch (UnauthorizedAccessException)
            {
                EventManager.RaiseEvent(
                    EventType.Settings,
                    false,
                    new SettingsMessage($"Could not access the settings file in {Environment.CurrentDirectory}. " +
                                        $"Please make sure the application has the rights to access the {_filePath} file. " +
                                        $"The default settings will be used, but changes to them can't be saved.")
                );
            }
            catch (Exception ex)
            {
                EventManager.RaiseEvent(
                    EventType.Settings,
                    false,
                    new SettingsMessage($"The settings could not be loaded: { ex.Message } " +
                                        $"The default settings will be used, but changes to them can't be saved.")
                );
            }

            // use default settings but don't try to save them
            _appSettings = new AppSettings();
        }
        else
        {
            // use default settings and create the file
            _appSettings = new AppSettings();

            if (WriteSettingsToFile())
            {
                EventManager.RaiseEvent(
                    EventType.Settings,
                    true,
                    new SettingsMessage($"Successfully created the default settings file.")
                );
            }
        }
    }

    public bool CheckSettingsStatus()
    {
        if (!SettingsLoaded)
        {
            EventManager.RaiseEvent(
                EventType.Settings,
                false,
                new SettingsMessage("The settings could not be loaded. " +
                                    "The default settings will be used, but changes to them can't be saved.")
            );
            return false;
        }

        return true;
    }

    /// <summary>
    /// Update the CSV file path for the TodoList. Calls UpdateTodoCsvPath() with the default file name.
    /// </summary>
    /// <param name="newLocation"> The directory path to be used for the new CSV file </param>
    /// <param name="propertyName"> Used to notify the user about the result of the update operation. (ApplicationEvent) </param>
    /// <returns>
    /// True if the settings update succeeded.
    /// False if the settings update didn't succeeded.
    /// </returns>
    public bool UpdateTodoCsvLocation(string? newLocation, string? propertyName)
    {
        if (!string.IsNullOrWhiteSpace(newLocation))
        {
            string todoCsvPath = Path.Combine(newLocation, AppSettings.DefaultTodoCsvFileName);
            return UpdateTodoCsvPath(todoCsvPath, propertyName);
        }

        return false;
    }

    /// <summary>
    /// Update the CSV file path for the todos. If the specified file does not exist, create it.
    /// The file path is only updated if the CSV is successfully loaded or created.
    /// </summary>
    /// <param name="newPath"> The path to be used for the CSV file </param>
    /// <param name="propertyName"> Used to notify the user about the result of the update operation. (ApplicationEvent) </param>
    /// <returns>
    /// True if the settings update succeeded.
    /// False if the settings update didn't succeeded.
    /// </returns>
    public bool UpdateTodoCsvPath(string? newPath, string? propertyName)
    {
        if (!string.IsNullOrWhiteSpace(newPath))
        {
            bool resultWriteSettings = false;
            bool serviceResultTodo;

            try
            {
                serviceResultTodo = _todoService.RefreshTodoCsvPath(newPath, propertyName);
            }
            catch (FileNotFoundException)
            {
                serviceResultTodo = _todoService.SaveTodoCsv(newPath, propertyName);
            }

            if (serviceResultTodo)
            {
                if (_appSettings.TodoCsvPath == newPath)
                    // no need to write the settings to file if the path is the same,
                    // only happens when the SettingsViewModel initially sets the property
                    return true;

                _appSettings.TodoCsvPath = newPath;
                resultWriteSettings = WriteSettingsToFile();
            }

            return resultWriteSettings && serviceResultTodo;
        }

        return false;
    }

    /// <summary>
    /// Returns the todoCSV Path.
    /// </summary>
    public string GetTodoCsvPath()
    {
        return _appSettings.TodoCsvPath;
    }

    /// <summary>
    /// Update the CSV location for the Archive. Calls UpdateArchiveCsvPath() with the default file name.
    /// </summary>
    /// <param name="newLocation"> The directory path to be used for the new CSV file </param>
    /// <param name="propertyName"> Used to notify the user about the result of the update operation. (ApplicationEvent) </param>
    /// <returns>
    /// True if the settings update succeeded.
    /// False if the settings update didn't succeeded.
    /// </returns>
    public bool UpdateArchiveCsvLocation(string? newLocation, string? propertyName)
    {
        if (!string.IsNullOrWhiteSpace(newLocation))
        {
            string archiveCsvPath = Path.Combine(newLocation, AppSettings.DefaultArchiveCsvFileName);
            return UpdateArchiveCsvPath(archiveCsvPath, propertyName);
        }

        return false;
    }

    /// <summary>
    /// Update the CSV file path for the Archive. If the specified file does not exist, create it.
    /// The file path is only updated if the CSV is successfully loaded or created.
    /// </summary>
    /// <param name="newPath"> The path to be used for the CSV file </param>
    /// <param name="propertyName"> Used to notify the user about the result of the update operation. (ApplicationEvent) </param>
    /// <returns>
    /// True if the settings update succeeded.
    /// False if the settings update didn't succeeded.
    /// </returns>
    public bool UpdateArchiveCsvPath(string? newPath, string? propertyName)
    {
        if (!string.IsNullOrWhiteSpace(newPath))
        {
            bool resultWriteSettings = false;
            bool serviceResultArchive;

            try
            {
                serviceResultArchive = _todoService.RefreshArchiveCsvPath(newPath, propertyName);
            }
            catch (FileNotFoundException ex)
            {
                serviceResultArchive = _todoService.SaveArchiveCsv(newPath, propertyName);
            }

            if (serviceResultArchive)
            {
                if (_appSettings.ArchiveCsvPath == newPath)
                    // no need to write if the path is the same, only happens when the
                    // SettingsViewModel initially sets the property
                    return true;

                _appSettings.ArchiveCsvPath = newPath;
                resultWriteSettings = WriteSettingsToFile();
            }

            return resultWriteSettings && serviceResultArchive;
        }

        return false;
    }

    /// <summary>
    /// Returns the ArchiveCSV Path.
    /// </summary>
    public string GetArchiveCsvPath()
    {
        return _appSettings.ArchiveCsvPath;
    }

    /// <summary>
    /// Updates the dark mode enabled property.
    /// </summary>
    public bool UpdateDarkModeEnabled(bool enabled)
    {
        _appSettings.DarkModeEnabled = enabled;
        WriteSettingsToFile();

        return true;
    }
}