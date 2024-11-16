using AgroGestor360Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AgroGestor360App.ViewModels;

[QueryProperty(nameof(Concepts), "concepts")]
public partial class PgDeletedInvoiceViewModel : ObservableObject
{
    [ObservableProperty]
    ConceptForDeletedSaleRecord[]? concepts;

    [ObservableProperty]
    ConceptForDeletedSaleRecord? selectedConcept;

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

        ConceptForDeletedSaleRecord send = SelectedConcept ?? new() { Concept = AnotherConcept };

        _ = WeakReferenceMessenger.Default.Send(send, "deletedinvoice");

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", "A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
        await Shell.Current.GoToAsync("..", true);
    }
}
