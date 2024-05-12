using ReactiveUI;

namespace TODO.ViewModels.Archive;

public class ConfirmTodoDeletionViewModel : ViewModelBase
{
    public ConfirmTodoDeletionViewModel()
    {
        ConfirmDeletionCommand = ReactiveCommand.Create<bool, bool>(b => b);
    }

    public bool True => true;
    public bool False => false;

    public ReactiveCommand<bool, bool> ConfirmDeletionCommand { get; }

}