using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using vCardLib.Models;

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
    vCard? currentSeller;

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

        Dictionary<string, string> customFields = string.IsNullOrEmpty(Address)
            ? new() { { nameof(NIT), NIT! }, { nameof(DPI), DPI! }, { "REGISTRATIONDATE", Date.ToString()! } }
            : new() { { nameof(NIT), NIT! }, { nameof(DPI), DPI! }, { "REGISTRATIONDATE", Date.ToString()! }, { "ADDRESS", Address!.Trim().ToUpper() } };

        vCard theSeller = new(vCardLib.Enums.vCardVersion.v4)
        {
            Uid = CurrentSeller?.Uid,
            Language = new Language(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
            FormattedName = Name!.Trim().ToUpper(),
            BirthDay = Birthday.Date >= Date.Date ? null : Birthday.Date,
            CustomFields = [.. customFields],
            PhoneNumbers = [new TelephoneNumber(Phone!.Trim(), vCardLib.Enums.TelephoneNumberType.None)],
            EmailAddresses = string.IsNullOrEmpty(Email) ? new() : [new EmailAddress(Email!.Trim().ToLower(), vCardLib.Enums.EmailAddressType.None)]
        };

        string token = CurrentSeller is null ? "newSeller" : "editSeller";

        WeakReferenceMessenger.Default.Send(theSeller, token);

        await Cancel();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(CurrentSeller))
        {
            Name = CurrentSeller!.FormattedName;
            NIT = CurrentSeller.CustomFields.First(x => x.Key == nameof(NIT)).Value;
            DPI = CurrentSeller.CustomFields.First(x => x.Key == nameof(DPI)).Value;
            Phone = CurrentSeller.PhoneNumbers.First().Number;
            Birthday = CurrentSeller!.BirthDay ?? DateTime.Now;
            Email = CurrentSeller.EmailAddresses.FirstOrDefault().Value;
            Address = CurrentSeller.CustomFields.FirstOrDefault(x => x.Key == "ADDRESS").Value;
            Date = DateTime.Parse(CurrentSeller.CustomFields.FirstOrDefault(x => x.Key == "REGISTRATIONDATE").Value);
        }
    }
}
