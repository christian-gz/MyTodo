using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TODO.ViewModels;
using TODO.ViewModels.Settings;

namespace TODO.Views.Settings;

public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        InitializeComponent();

        if (Design.IsDesignMode) return;

        this.WhenActivated(d =>
        {
            d(ViewModel.SelectFolderInteraction.RegisterHandler(this.InteractionHandlerFolderPicker));
        });

        this.WhenActivated(d =>
        {
            d(ViewModel.SelectFileInteraction.RegisterHandler(this.InteractionHandlerFilePicker));
        });
    }

    private async Task InteractionHandlerFolderPicker(InteractionContext<string?, string?> context)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(this);

            var storageFolders = await topLevel!.StorageProvider
                .OpenFolderPickerAsync(
                    new FolderPickerOpenOptions()
                    {
                        AllowMultiple = false,
                        Title = context.Input,
                        SuggestedStartLocation = await topLevel!.StorageProvider
                            .TryGetFolderFromPathAsync(ViewModel.SelectedFolderTodoCsv)
                    });

            if (storageFolders != null && storageFolders.Any())
            {
                string decodedPath = Uri.UnescapeDataString(storageFolders?.First().Path.AbsolutePath);
                var normalizedPath = Path.GetFullPath(decodedPath);

                context.SetOutput(normalizedPath);
            }
            else
            {
                context.SetOutput(null);
            }
        }
        catch (Exception ex)
        {
            context.SetOutput(null);
            Console.WriteLine($"Error occurred during folder selection: { ex.Message}");
        }
    }

    private async Task InteractionHandlerFilePicker(InteractionContext<string?, string?> context)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(this);

            var storageFiles = await topLevel!.StorageProvider
                .OpenFilePickerAsync(
                    new FilePickerOpenOptions()
                    {
                        AllowMultiple = false,
                        Title = context.Input,
                        SuggestedStartLocation = await topLevel!.StorageProvider
                            .TryGetFolderFromPathAsync(ViewModel.SelectedFolderTodoCsv)
                    });

            if (storageFiles != null && storageFiles.Any())
            {
                string decodedPath = Uri.UnescapeDataString(storageFiles?.First().Path.AbsolutePath);
                var normalizedPath = Path.GetFullPath(decodedPath);

                context.SetOutput(normalizedPath);
            }
            else
            {
                context.SetOutput(null);
            }
        }
        catch (Exception ex)
        {
            context.SetOutput(null);
            Console.WriteLine($"Error occurred during folder selection: { ex.Message}");
        }

    }
}