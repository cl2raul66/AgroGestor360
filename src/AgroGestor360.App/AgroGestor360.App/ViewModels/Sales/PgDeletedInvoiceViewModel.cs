using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using AgroGestor360.App.Tools.Messages;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(Concepts), "concepts")]
public partial class PgDeletedInvoiceViewModel : ObservableObject
{
    [ObservableProperty]
    ConceptForDeletedInvoice[]? concepts;

    [ObservableProperty]
    ConceptForDeletedInvoice? selectedConcept;

    [ObservableProperty]
    string? anotherConcept;

    [ObservableProperty]
    bool dueOperatorError = true;

    [ObservableProperty]
    bool forAnotherConcept;

    [ObservableProperty]
    bool isVisibleInfo;

    [RelayCommand]
    void AddConcept()
    {
        ForAnotherConcept = !ForAnotherConcept;
        if (ForAnotherConcept)
        {
            SelectedConcept = null;
        }
        else
        {
            AnotherConcept = null;
        }
    }

    [RelayCommand]
    async Task Save()
    {
        if (!DueOperatorError && SelectedConcept is null && string.IsNullOrEmpty(AnotherConcept))
        {
            IsVisibleInfo = true;
            await Task.Delay(4000);
            IsVisibleInfo = false;

            return;
        }

        var send = SelectedConcept ?? new ConceptForDeletedInvoice { Concept = AnotherConcept };

        _ = WeakReferenceMessenger.Default.Send(send, "deletedinvoice");

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send(new CancelDialogForPgSalesRequestMessage(true));
        await Shell.Current.GoToAsync("..");
    }
}
