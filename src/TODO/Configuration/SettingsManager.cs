using System;
using System.IO;
using System.Text.Json;
using TODO.Events;

namespace TODO.Configuration;

public class SettingsManager
{
    public SettingsManager()
    {
        LoadSettings();
    }

    private const string _filePath = "appsettings.json";
    private AppSettings _appSettings;
    private AppSettings _appSettingsToApply = new AppSettings();

    public event EventHandler<SettingsChangedEventArgs> SettingsChange;
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
                new SettingsMessage(
                    null,
                    $"Can't write to the settings file. Settings can't be saved.\n" +
                    $"{ ex.Message }"
                )
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
        SettingsLoaded = false;

        if (File.Exists(_filePath))
        {
            try
            {
                string json = File.ReadAllText(_filePath);
                _appSettings = JsonSerializer.Deserialize<AppSettings>(json);

                EventManager.RaiseEvent(
                    EventType.Settings,
                    true,
                    new SettingsMessage(
                        null,
                        $"Successfully loaded the settings."
                    )
                );

                SettingsLoaded = true;
                return;
            }
            catch (UnauthorizedAccessException)
            {
                EventManager.RaiseEvent(
                    EventType.Settings,
                    false,
                    new SettingsMessage(
                        null,
                        $"Could not access the settings file in {Environment.CurrentDirectory}. " +
                        $"Please make sure the application has the rights to access the {_filePath} file. " +
                        $"The default settings will be used, but changes to them can't be saved."
                    )
                );
            }
            catch (Exception ex)
            {
                EventManager.RaiseEvent(
                    EventType.Settings,
                    false,
                    new SettingsMessage(
                        null,
                        $"The settings could not be loaded: { ex.Message } " +
                        $"The default settings will be used, but changes to them can't be saved."
                    )
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
                    new SettingsMessage(
                        null,
                        $"Successfully created the default settings file."
                    )
                );
            }
        }
    }

    /// <summary>
    /// Checks if the settings can be saved to the appsettings.json file.
    /// </summary>
    public void CheckSettingsStatus()
    {
        if (!SettingsLoaded)
        {
            LoadSettings();
            if (SettingsLoaded)
            {
                EventManager.RaiseEvent(
                    EventType.Settings,
                    true,
                    new SettingsMessage(
                        null,
                        "The settings are successfully loaded. " +
                        "Please restart the application to apply the settings."
                    )
                );
            }
        }
        else
        {
            EventManager.RaiseEvent(
                EventType.Settings,
                true,
                new SettingsMessage(
                    null,
                    "The settings are successfully loaded."
                )
            );
        }
    }

    /// <summary>
    /// Finalises the settings change if the Service could apply the change and
    /// if the SettingsManager can write to the settings file.
    /// </summary>
    /// <param name="setting"> Which setting got changed </param>
    /// <param name="isSuccess"> True if the Service could apply the change. </param>
    public void ConfirmSettingsChange(string setting, bool isSuccess)
    {
        switch (setting)
        {
            case "TodoCsvPath":
                if (isSuccess)
                {
                    var savePath = _appSettings.TodoCsvPath;
                    _appSettings.TodoCsvPath = _appSettingsToApply.TodoCsvPath;

                    if (WriteSettingsToFile())
                    {
                        EventManager.RaiseEvent(
                            EventType.Settings,
                            true,
                            new SettingsMessage(
                                "TodoCsvPath",
                                "Successfully changed the CSV file of the todos."
                            )
                        );
                    }
                    else
                    {
                        // restore old path if it is not possible to write to the settings file
                        _appSettings.TodoCsvPath = savePath;
                    }
                }
                else
                {
                    EventManager.RaiseEvent(
                        EventType.Settings,
                        false,
                        new SettingsMessage(
                            "TodoCsvPath",
                            $"Could not change the CSV file to { _appSettingsToApply.TodoCsvPath  }"
                        )
                    );
                }
                break;
            case "ArchiveCsvPath":
                if (isSuccess)
                {
                    var savePath = _appSettings.ArchiveCsvPath;
                    _appSettings.ArchiveCsvPath = _appSettingsToApply.ArchiveCsvPath;

                    if (WriteSettingsToFile())
                    {
                        EventManager.RaiseEvent(
                            EventType.Settings,
                            true,
                            new SettingsMessage(
                                "ArchiveCsvPath",
                                "Successfully changed the CSV file of the archive."
                            )
                        );
                    }
                    else
                    {
                        // restore old path if it is not possible to write to the settings file
                        _appSettings.ArchiveCsvPath = savePath;
                    }
                }
                else
                {
                    EventManager.RaiseEvent(
                        EventType.Settings,
                        false,
                        new SettingsMessage(
                            "ArchiveCsvPath",
                            $"Could not change the CSV file to { _appSettingsToApply.ArchiveCsvPath  }"
                        )
                    );
                }
                break;
        }
    }

    /// <summary>
    /// Update the CSV file path for the TodoList. Calls UpdateTodoCsvPath() with the default file name.
    /// </summary>
    /// <param name="newLocation"> The directory path to be used for the new CSV file </param>
    public void UpdateTodoCsvLocation(string? newLocation)
    {
        if (!string.IsNullOrWhiteSpace(newLocation))
        {
            string todoCsvPath = Path.Combine(newLocation, AppSettings.DefaultTodoCsvFileName);
            UpdateTodoCsvPath(todoCsvPath);
        }
    }

    /// <summary>
    /// Update the CSV file path for the todos. If the specified file does not exist, create it.
    /// The file path is only updated if the CSV is successfully loaded or created.
    /// </summary>
    /// <param name="newPath"> The path to be used for the CSV file </param>
    public void UpdateTodoCsvPath(string? newPath)
    {
        if (!string.IsNullOrWhiteSpace(newPath))
        {
            _appSettingsToApply.TodoCsvPath = newPath;
            SettingsChange?.Invoke(this, new SettingsChangedEventArgs {
                Setting = "TodoCsvPath",
                Value = newPath
            });
        }
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
    public void UpdateArchiveCsvLocation(string? newLocation)
    {
        if (!string.IsNullOrWhiteSpace(newLocation))
        {
            string archiveCsvPath = Path.Combine(newLocation, AppSettings.DefaultArchiveCsvFileName);
            UpdateArchiveCsvPath(archiveCsvPath);
        }
    }

    /// <summary>
    /// Update the CSV file path for the Archive. If the specified file does not exist, create it.
    /// The file path is only updated if the CSV is successfully loaded or created.
    /// </summary>
    /// <param name="newPath"> The path to be used for the CSV file </param>
    public void UpdateArchiveCsvPath(string? newPath)
    {
        if (!string.IsNullOrWhiteSpace(newPath))
        {
            _appSettingsToApply.ArchiveCsvPath = newPath;
            SettingsChange?.Invoke(this, new SettingsChangedEventArgs {
                Setting = "ArchiveCsvPath",
                Value = newPath
            });
        }
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