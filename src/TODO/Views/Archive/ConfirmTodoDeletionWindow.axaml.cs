using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TODO.ViewModels.Archive;

namespace TODO.Views.Archive;

public partial class ConfirmTodoDeletionWindow : ReactiveWindow<ConfirmTodoDeletionViewModel>
{
    public ConfirmTodoDeletionWindow()
    {
        InitializeComponent();

        if (Design.IsDesignMode) return;

        this.WhenActivated(disposables =>
        {
            ViewModel!.ConfirmDeletionCommand.Subscribe(result =>
                {
                    Close(result);
                })
                .DisposeWith(disposables);
        });
    }
}