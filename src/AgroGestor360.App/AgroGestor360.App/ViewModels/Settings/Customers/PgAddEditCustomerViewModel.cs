using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Globalization;
using vCardLib.Models;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentCustomer), nameof(CurrentCustomer))]
public partial class PgAddEditCustomerViewModel : ObservableValidator
{
    public PgAddEditCustomerViewModel()
    {
        Date = DateTime.Now;
        Birthday = new(Date.Ticks);
    }

    [ObservableProperty]
    vCard? currentCustomer;

    [ObservableProperty]
    DateTime date;

    [ObservableProperty]
    DateTime birthday;

    [ObservableProperty]
    [Required]
    [MinLength(3)]
    string? name;

    [ObservableProperty]
    string? businessName;

    [ObservableProperty]
    [Required]
    [MinLength(5)]
    string? nIT;

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
    
    [ObservableProperty]
    bool isBusiness;

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors || (IsBusiness && string.IsNullOrEmpty(BusinessName)))
        {
            IsVisisbleInfo = true;
            await Task.Delay(4000);
            IsVisisbleInfo = false;

            return;
        }

        Dictionary<string, string> customFields = string.IsNullOrEmpty(Address)
            ? new() { { nameof(NIT), NIT! }, { "REGISTRATIONDATE", Date.ToString()! } }
            : new() { { nameof(NIT), NIT! }, { "REGISTRATIONDATE", Date.ToString()! }, { "ADDRESS", Address!.Trim().ToUpper() } };

        vCard theCustomer = new(vCardLib.Enums.vCardVersion.v4)
        {
            Uid = CurrentCustomer?.Uid,
            Language = new Language(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
            FormattedName = Name!.Trim().ToUpper(),
            BirthDay = Birthday.Date >= Date.Date ? null : Birthday.Date,
            CustomFields = [.. customFields],
            PhoneNumbers = [new TelephoneNumber(Phone!.Trim(), vCardLib.Enums.TelephoneNumberType.None)],
            EmailAddresses = string.IsNullOrEmpty(Email) ? new() : [new EmailAddress(Email!.Trim().ToLower(), vCardLib.Enums.EmailAddressType.None)],
            Organization = IsBusiness ? new Organization(BusinessName!, null, null) : null
        };

        string token = CurrentCustomer is null ? "newCustomer" : "editCustomer";

        WeakReferenceMessenger.Default.Send(theCustomer, token);

        await Cancel();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(CurrentCustomer))
        {
            Name = CurrentCustomer!.FormattedName;
            NIT = CurrentCustomer.CustomFields.First(x => x.Key == nameof(NIT)).Value;
            Phone = CurrentCustomer.PhoneNumbers.First().Number;
            Birthday = CurrentCustomer!.BirthDay ?? DateTime.Now;
            Email = CurrentCustomer.EmailAddresses.FirstOrDefault().Value;
            Address = CurrentCustomer.CustomFields.FirstOrDefault(x => x.Key == "ADDRESS").Value;
            BusinessName = CurrentCustomer!.Organization?.Name;
            Date = DateTime.Parse(CurrentCustomer.CustomFields.FirstOrDefault(x => x.Key == "REGISTRATIONDATE").Value);

            IsBusiness = CurrentCustomer!.Organization is not null;
        }
    }
}
