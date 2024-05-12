using System;
using TODO.Models;

namespace TODO.ViewModels.Todo;

public class TodoItemEventArgs : EventArgs
{
    public TodoItemEventArgs(TodoItem todo)
    {
        Todo = todo;
    }

    public TodoItem Todo { get; }
}
