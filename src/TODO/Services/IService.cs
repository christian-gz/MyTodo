using System;
using System.Collections.Generic;
using System.ComponentModel;
using TODO.Configuration;
using TODO.Models;

namespace TODO.Services;

public interface IService
{
    /// <summary>
    /// Event to raise when the TodoList changed because of loading a new todos.csv.
    /// </summary>
    public event EventHandler TodoListChanged;

    /// <summary>
    /// Event to raise when the Archive changed because of loading a new archive.csv.
    /// </summary>
    public event EventHandler ArchiveChanged;

    /// <summary>
    /// Returns the TodoList.
    /// </summary>
    public IEnumerable<TodoItem> GetTodoList();

    /// <summary>
    /// Add todoItem from TodoList.
    /// </summary>
    public void AddToTodoList(TodoItem todoItem);

    /// <summary>
    /// Remove todoItem from TodoList.
    /// </summary>
    public void RemoveFromTodoList(TodoItem todoItem);

    /// <summary>
    /// Remove all todoItems from TodoList and add all to the Archive.
    /// </summary>
    public void RemoveAllFromTodoList();

    /// <summary>
    /// Returns the TodoList.
    /// </summary>
    public IEnumerable<TodoItem> GetArchive();

    /// <summary>
    /// Add todoItem to the Archive.
    /// </summary>
    public void AddToArchive(TodoItem todoItem);

    /// <summary>
    /// Remove todoItem to the Archive.
    /// </summary>
    public void RemoveFromArchive(TodoItem todoItem);

    /// <summary>
    /// Save the relevant Data when the application is closing.
    /// </summary>
    public void OnClosing();

    /// <summary>
    /// Try to save the TodoList in the CSV with the todoCsvPath.
    /// </summary>
    /// <param name="todoCsvPath"> TodoList CSV filepath to try to save the data. </param>
    /// <returns> True if successful, false otherwise </returns>
    public bool SaveTodoCsv(string todoCsvPath);

    /// <summary>
    /// Try to save the Archive in the CSV with the archiveCsvPath.
    /// </summary>
    /// <param name="archiveCsvPath"> Archive CSV filepath to try to save the data. </param>
    /// <returns> True if successful, false otherwise </returns>
    public bool SaveArchiveCsv(string archiveCsvPath);
}