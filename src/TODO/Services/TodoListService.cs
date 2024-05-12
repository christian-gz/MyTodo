using System;
using System.Collections.Generic;
using System.IO;
using TODO.Events;
using TODO.Models;

namespace TODO.Services;

public class TodoListService : IService
{
    public TodoListService() { }

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
        TrySaveData(_todoCsvPath, TodoList, null);
        TrySaveData(_archiveCsvPath, Archive, null);
    }

    public bool RefreshTodoCsvPath(string todoCsvPath, string? propertyName)
    {
        if (_todoCsvPath == todoCsvPath)
            // no need to load if the path is the same, only happens when the
            // SettingsViewModel initially sets the property
            return true;

        bool resultTryLoadData = TryLoadData(todoCsvPath, TodoList, TodoListChanged, true, propertyName);

        if (resultTryLoadData)
            _todoCsvPath = todoCsvPath;

        return resultTryLoadData;
    }

    public bool SaveTodoCsv(string todoCsvPath, string? propertyName)
    {
        if (TrySaveData(todoCsvPath, TodoList, propertyName))
        {
            if (_todoCsvPath != todoCsvPath)
                // if the file is missing when first calling RefreshTodoCsvPath the path is not set
                _todoCsvPath = todoCsvPath;

            return true;
        }

        return false;
    }

    public bool RefreshArchiveCsvPath(string archiveCsvPath, string? propertyName)
    {
        if (_archiveCsvPath == archiveCsvPath)
            // no need to load if the path is the same, only happens when the
            // SettingsViewModel initially sets the property
            return true;

        bool resultTryLoadData = TryLoadData(archiveCsvPath, Archive, ArchiveChanged, false, propertyName);

        if (resultTryLoadData)
            _archiveCsvPath = archiveCsvPath;

        return resultTryLoadData;
    }

    public bool SaveArchiveCsv(string archiveCsvPath, string? propertyName)
    {
        if (TrySaveData(archiveCsvPath, Archive, propertyName))
        {
            if (_archiveCsvPath != archiveCsvPath)
                // if the file is missing when first calling RefreshTodoCsvPath the path is not set
                _archiveCsvPath = archiveCsvPath;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to load the data from the specified path to the specified list.
    /// Raises an event that the used list has changed.
    /// </summary>
    /// <param name="path"> The file path where the TodoItems list should be loaded from. </param>
    /// <param name="list"> The list to save the loaded TodoItems at. </param>
    /// <param name="eventHandler"> The event to invoke if the data is successfully loaded. </param>
    /// <param name="isTodoList"> Used to differentiate the TodoList so the data can be reset. </param>
    /// <param name="propertyName"> Used to notify the user about the result of the save operation. (ApplicationEvent) </param>
    /// <returns> True if successful, false otherwise </returns>
    private bool TryLoadData(string path, List<TodoItem> list, EventHandler eventHandler,
                             bool isTodoList, string? propertyName)
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
                    propertyName,
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
                    propertyName,
                    $"Could not load the file with path { path }, " +
                    $"please make sure the application has the rights to access it or " +
                    $"select another file."
                    )
                );
        }
        catch (FileNotFoundException)
        {
            // if the file is not found, the SettingsManager can create it
            throw;
        }
        catch (Exception ex)
        {
            EventManager.RaiseEvent(
                EventType.Service,
                false,
                new ServiceMessage(
                    propertyName,
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
    /// <param name="propertyName"> Used to notify the user about the result of the save operation. (ApplicationEvent) </param>
    /// <returns> True if successful, false otherwise </returns>
    private bool TrySaveData(string path, List<TodoItem> list, string? propertyName)
    {
        try
        {
            _csvFileManager.SaveToFile(path, list);

            EventManager.RaiseEvent(
                EventType.Service,
                true,
                new ServiceMessage(
                    propertyName,
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
                    propertyName,
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
                    propertyName,
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