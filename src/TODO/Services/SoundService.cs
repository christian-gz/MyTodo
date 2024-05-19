using System;
using System.IO;
using NetCoreAudio;
using TODO.Configuration;
using TODO.ViewModels.Todo;

namespace TODO.Services;

/// <summary>
/// Plays a sound after a TodoItem is completed.
/// </summary>
public class SoundService
{
    private readonly SettingsManager _settingsManager;
    private bool _completionSoundEnabled;

    private readonly Player _player;
    private readonly string _filePath;

    public SoundService(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _completionSoundEnabled = _settingsManager.GetCompletionSoundEnabled();

        _player = new Player();
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _filePath = Path.Combine(baseDirectory, "Assets", "completion_sound.wav");

        TodoItemViewModel.TodoEdited += OnTodoEdited;
        _settingsManager.SettingsChange += HandleSettingsChange;
    }

    private void HandleSettingsChange(object? o, SettingsChangedEventArgs e)
    {
        if (e.Setting == "CompletionSoundEnabled")
        {
            bool value = bool.Parse(e.Value);
            _completionSoundEnabled = value;
        }
    }

    private void OnTodoEdited(object? o, TodoItemEventArgs e)
    {
        if (_completionSoundEnabled && e.Todo.IsChecked)
            PlaySound();
    }

    private void PlaySound()
    {
        _player.Play(_filePath);
    }
}