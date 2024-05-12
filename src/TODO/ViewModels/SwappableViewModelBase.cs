namespace TODO.ViewModels;

/// <summary>
/// Abstract base class for view models that support being swapped in and out of a view.
/// </summary>
public abstract class SwappableViewModelBase : ViewModelBase
{
    /// <summary>
    /// Gets called by MainWindowViewModel every time the user switches to the corresponding view.
    /// Used to refresh or initialize the view model state.
    /// </summary>
    public abstract void LoadViewModel();
}