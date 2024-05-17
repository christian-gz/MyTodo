using System;
using System.IO;
using NetCoreAudio;
using TODO.ViewModels.Todo;

namespace TODO.Services;

/// <summary>
/// Plays a sound after a TodoItem is completed.
/// </summary>
public class SoundService
{
    private readonly Player _player;
    private readonly string _filePath;

    public bool CompletionSoundEnabled { get; set; }

    public SoundService()
    {
        _player = new Player();
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _filePath = Path.Combine(baseDirectory, "Assets", "completion_sound.wav");

        TodoItemViewModel.TodoEdited += OnTodoEdited;
    }

    private void OnTodoEdited(object? o, TodoItemEventArgs e)
    {
        if (CompletionSoundEnabled && e.Todo.IsChecked)
            PlaySound();
    }

    private void PlaySound()
    {
        _player.Play(_filePath);
    }
}