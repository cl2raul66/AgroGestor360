using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentSeller), nameof(CurrentSeller))]
public partial class PgAddEditSellerViewModel : ObservableValidator
{
    public PgAddEditSellerViewModel()
    {
        Date = DateTime.Now;
        Birthday = new(Date.Ticks);
    }

    [ObservableProperty]
    DTO6_2? currentSeller;

    [ObservableProperty]
    DateTime date;

    [ObservableProperty]
    DateTime birthday;

    [ObservableProperty]
    [Required]
    [MinLength(3)]
    string? name;

    [ObservableProperty]
    [Required]
    [MinLength(5)]
    string? nIT;

    [ObservableProperty]
    [Required]
    [MinLength(5)]
    string? dPI;

    [ObservableProperty]
    [Required]
    [Phone]
    string? phone;

    [ObservableProperty]
    [EmailAddress]
    string? email;

    [ObservableProperty]
    string? address;

    [ObservableProperty]
    bool isVisisbleInfo;

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            IsVisisbleInfo = true;
            await Task.Delay(4000);
            IsVisisbleInfo = false;

            return;
        }

        switch (CurrentSeller is null)
        {
            case false:
                DTO6_2 editSeller = new()
                {
                    Id = CurrentSeller.Id,
                    FullName = Name?.Trim().ToUpper(),
                    NIT = NIT?.Trim().ToUpper(),
                    NIP = DPI?.Trim().ToUpper(),
                    Phone = Phone?.Trim(),
                    Mail = Email?.Trim().ToLower(),
                    Address = Address?.Trim().ToUpper(),
                    Birthday = Birthday.Date <= DateTime.Now.AddYears(-18) ? Birthday.Date : null
                };
                WeakReferenceMessenger.Default.Send(editSeller, "editSeller");
                break;
            default:
                DTO6_1 newSeller = new()
                {
                    FullName = Name?.Trim().ToUpper(),
                    NIT = NIT?.Trim().ToUpper(),
                    NIP = DPI?.Trim().ToUpper(),
                    Phone = Phone?.Trim(),
                    Mail = Email?.Trim().ToLower(),
                    Address = Address?.Trim().ToUpper(),
                    Birthday = Birthday.Date <= DateTime.Now.AddYears(-18) ? Birthday.Date : null
                };
                WeakReferenceMessenger.Default.Send(newSeller, "newSeller");
                break;
        }

        await Cancel();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(CurrentSeller))
        {
            Name = CurrentSeller?.FullName;
            NIT = CurrentSeller?.NIT;
            DPI = CurrentSeller?.NIP;
            Phone = CurrentSeller?.Phone;
            Birthday = CurrentSeller?.Birthday ?? DateTime.Now;
            Email = CurrentSeller?.Mail;
            Address = CurrentSeller?.Address;
        }
    }
}
//TODO: poner en el dto de inserccion y modificacion una propiedad para la fecha de inserccion y de modificacion
