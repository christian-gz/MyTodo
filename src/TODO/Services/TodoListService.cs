using System;
using System.Collections.Generic;
using System.IO;
using TODO.Configuration;
using TODO.Events;
using TODO.Models;

namespace TODO.Services;

public class TodoListService : IService
{
    public TodoListService(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _todoCsvPath = _settingsManager.GetTodoCsvPath();
        _archiveCsvPath = _settingsManager.GetArchiveCsvPath();

        _settingsManager.SettingsChange += HandleSettingsChange;

        TryLoadData(_todoCsvPath, TodoList, TodoListChanged, true);
        TryLoadData(_archiveCsvPath, Archive, ArchiveChanged, false);
    }

    private readonly SettingsManager _settingsManager;
    private readonly CsvFileManager _csvFileManager = new();
    private string _todoCsvPath;
    private string _archiveCsvPath;
    public event EventHandler TodoListChanged;
    public event EventHandler ArchiveChanged;

    private List<TodoItem> TodoList { get; set; } = new()
    {
        new TodoItem() { Title = "Thank you for trying out my app :)", Priority = Priority.Medium },
        new TodoItem() { Title = "You can edit each todo by using the button on the right.", Priority = Priority.Low, },
        new TodoItem() { Title = "Move the completed todos or all todos to the archive by using the buttons below.", Priority = Priority.Low, },
    };
    private List<TodoItem> Archive { get; set; } = new();

    public IEnumerable<TodoItem> GetTodoList()
    {
        return TodoList;
    }

    public void AddToTodoList(TodoItem todoItem)
    {
        TodoList.Add(todoItem);
    }

    public void RemoveFromTodoList(TodoItem todoItem)
    {
        TodoList.Remove(todoItem);
    }

    public void RemoveAllFromTodoList()
    {
        Archive.AddRange(TodoList);
        TodoList.Clear();
    }

    public IEnumerable<TodoItem> GetArchive()
    {
        return Archive;
    }

    public void AddToArchive(TodoItem todoItem)
    {
        Archive.Add(todoItem);
    }

    public void RemoveFromArchive(TodoItem todoItem)
    {
        Archive.Remove(todoItem);
    }

    public void OnClosing()
    {
        TrySaveData(_todoCsvPath, TodoList);
        TrySaveData(_archiveCsvPath, Archive);
    }

    /// <summary>
    /// Handles the SettingsChanged event by applying the new path.
    /// </summary>
    /// <param name="o"> Object that raised the event. </param>
    /// <param name="e"> Includes which setting got changed and the new path. </param>
    private void HandleSettingsChange(object? o, SettingsChangedEventArgs e)
    {
        bool resultTryLoadData;
        string? path = e.Value;
        string? setting = e.Setting;
        
        if (setting == null)
            return;
        
        if (path == null)
        {
            _settingsManager.ConfirmSettingsChange(setting, false);
            return;
        }

        switch (e.Setting)
        {
            case "TodoCsvPath":
                if (_todoCsvPath == path)
                {
                    _settingsManager.ConfirmSettingsChange(setting, true);
                    return;
                }

                resultTryLoadData = TryLoadData(path, TodoList, TodoListChanged, true);

                if (resultTryLoadData)
                    _todoCsvPath = path;

                _settingsManager.ConfirmSettingsChange(setting, resultTryLoadData);

                break;
            case "ArchiveCsvPath":
                if (_archiveCsvPath == path)
                {
                    _settingsManager.ConfirmSettingsChange(setting, true);
                    return;
                }

                resultTryLoadData = TryLoadData(path, Archive, ArchiveChanged, false);

                if (resultTryLoadData)
                    _archiveCsvPath = path;

                _settingsManager.ConfirmSettingsChange(setting, resultTryLoadData);

                break;
        }
    }

    public bool SaveTodoCsv(string todoCsvPath)
    {
        throw new NotImplementedException();
    }

    public bool SaveArchiveCsv(string archiveCsvPath)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Try to load the data from the specified path to the specified list.
    /// Raises an event that the used list has changed.
    /// </summary>
    /// <param name="path"> The file path where the TodoItems list should be loaded from. </param>
    /// <param name="list"> The list to save the loaded TodoItems at. </param>
    /// <param name="eventHandler"> The event to invoke if the data is successfully loaded. </param>
    /// <param name="isTodoList"> Used to differentiate the TodoList so the data can be reset. </param>
    /// <returns> True if successful, false otherwise </returns>
    private bool TryLoadData(string path, List<TodoItem> list, EventHandler eventHandler, bool isTodoList)
    {
        try
        {
            var tempList = _csvFileManager.LoadFromFile(path);

            // loading from the file was successful
            list.Clear();

            if (isTodoList)
            {
                // clean the data if it is a TodoList
                var cleanedData = ResetTodoItemStates(tempList);
                list.AddRange(cleanedData);
            }
            else
            {
                list.AddRange(tempList);
            }

            // Raise an event, that the List changed
            eventHandler?.Invoke(this, EventArgs.Empty);

            EventManager.RaiseEvent(
                EventType.Service,
                true,
                new ServiceMessage(
                    null,
                    $"Successfully loaded { path }"
                )
            );

            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            EventManager.RaiseEvent(
                EventType.Service,
                false,
                new ServiceMessage(
                    null,
                    $"Could not load the file with path { path }, " +
                    $"please make sure the application has the rights to access it or " +
                    $"select another file."
                )
            );
        }
        catch (FileNotFoundException)
        {
            TrySaveData(path, list);
        }
        catch (Exception ex)
        {
            EventManager.RaiseEvent(
                EventType.Service,
                false,
                new ServiceMessage(
                    null,
                    $"Could not read the data from { path }. " +
                    $"{ ex.Message }"
                )
            );
        }

        return false;
    }

    /// <summary>
    /// Try to save the list to the specified file path.
    /// </summary>
    /// <param name="path"> The file path where the TodoItems list should be saved. </param>
    /// <param name="list"> The list of TodoItems to save. </param>
    /// <returns> True if successful, false otherwise </returns>
    private bool TrySaveData(string path, List<TodoItem> list)
    {
        try
        {
            _csvFileManager.SaveToFile(path, list);

            EventManager.RaiseEvent(
                EventType.Service,
                true,
                new ServiceMessage(
                    null,
                    $"Successfully wrote to { path }"
                )
            );

            return true;
        }
        catch (UnauthorizedAccessException)
        {
            EventManager.RaiseEvent(
                EventType.Service,
                false,
                new ServiceMessage(
                    null,
                    $"Could not save the file with path { path }, " +
                    $"please make sure the application has the rights to access it or " +
                    $"select another file."
                )
            );
        }
        catch (Exception ex)
        {
            EventManager.RaiseEvent(
                EventType.Service,
                false,
                new ServiceMessage(
                    null,
                    $"Could not create the file with path { path }, " +
                    $"{ ex.Message }"
                )
            );
        }

        return false;
    }

    /// <summary>
    /// Reset the todoList to be in an initial state to be used by the ViewModel.
    /// </summary>
    /// <param name="todoList"> List to reset </param>
    private List<TodoItem> ResetTodoItemStates(List<TodoItem> todoList)
    {
        foreach (var todo in todoList)
        {
            todo.IsEdited = false;
            todo.IsShown = true;
        }

        return todoList;
    }
}