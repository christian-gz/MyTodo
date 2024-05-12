using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TODO.ViewModels;
using TODO.ViewModels.Archive;
using TODO.Views.Archive;

namespace TODO.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        // ConfirmTodoDeletion from Archive
        this.WhenActivated(disposableRegistration =>
        {
            var archiveModel = (ArchiveViewModel)ViewModel!.ArchiveView;
            var registration = archiveModel.ShowDialog.RegisterHandler(DoShowConfirmTodoDeletionAsync);
            disposableRegistration(registration);
        });

        Closing += OnClosing;
    }

    private async Task DoShowConfirmTodoDeletionAsync(InteractionContext<ConfirmTodoDeletionViewModel, bool> interaction)
    {
        var dialog = new ConfirmTodoDeletionWindow();
        dialog.DataContext = interaction.Input;

        var result = await dialog.ShowDialog<bool>(this);
        interaction.SetOutput(result);
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        ViewModel!.OnClosing();
    }
}