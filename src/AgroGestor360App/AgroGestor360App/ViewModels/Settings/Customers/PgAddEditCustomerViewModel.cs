using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using AgroGestor360Client.Models;
using AgroGestor360App.Views.Settings;

namespace AgroGestor360App.ViewModels;

[QueryProperty(nameof(Discounts), nameof(Discounts))]
[QueryProperty(nameof(CurrentCustomer), "CurrentCustomer")]
[QueryProperty(nameof(Credits), "credits")]
public partial class PgAddEditCustomerViewModel : ObservableValidator
{
    public PgAddEditCustomerViewModel()
    {
        Date = DateTime.Now;
        Birthday = new(Date.Ticks);
    }

    [ObservableProperty]
    LineCreditItem[]? credits;

    [ObservableProperty]
    LineCreditItem? selectedCredit;

    [ObservableProperty]
    DiscountForCustomer[]? discounts;

    [ObservableProperty]
    DiscountForCustomer? selectedDiscount;

    [ObservableProperty]
    DTO5_3? currentCustomer;

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

    [ObservableProperty]
    string? title = "Agregar cliente";

    [ObservableProperty]
    string? titleBtn = "Agregar";

    [RelayCommand]
    async Task Cancel()
    {
        WeakReferenceMessenger.Default.Send("cancel", nameof(PgAddEditCustomer));
        await Shell.Current.GoToAsync("..", true);
    }

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

        switch (CurrentCustomer is null)
        {
            case false:
                DTO5_3 editCustomer = new()
                {
                    CustomerId = CurrentCustomer.CustomerId,
                    CustomerFullName = Name?.Trim().ToUpper(),
                    CustomerNIT = NIT!.Trim().ToUpper(),
                    CustomerOrganizationName = BusinessName?.Trim().ToUpper(),
                    CustomerPhone = Phone!.Trim(),
                    CustomerMail = Email?.Trim().ToLower(),
                    CustomerAddress = Address?.Trim().ToUpper(),
                    Birthday = Birthday.Date <= DateTime.Now.AddYears(-18) ? Birthday.Date : null,
                    Discount = CurrentCustomer!.Discount,
                };
                WeakReferenceMessenger.Default.Send(editCustomer, "editCustomer");
                break;
            default:
                DTO5_2 newCustomer = new()
                {
                    CustomerFullName = Name?.Trim().ToUpper(),
                    CustomerNIT = NIT!.Trim().ToUpper(),
                    CustomerOrganizationName = BusinessName?.Trim().ToUpper(),
                    CustomerPhone = Phone!.Trim(),
                    CustomerMail = Email?.Trim().ToLower(),
                    CustomerAddress = Address?.Trim().ToUpper(),
                    Birthday = Birthday.Date <= DateTime.Now.AddYears(-18) ? Birthday.Date : null,
                    Discount = SelectedDiscount,
                };
                WeakReferenceMessenger.Default.Send(newCustomer, "newCustomer");
                break;
        }

        await Shell.Current.GoToAsync("..", true);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(CurrentCustomer))
        {
            if (CurrentCustomer is not null)
            {
                Name = CurrentCustomer?.CustomerFullName;
                NIT = CurrentCustomer?.CustomerNIT;
                Phone = CurrentCustomer?.CustomerPhone;
                Birthday = CurrentCustomer?.Birthday ?? DateTime.Now;
                Email = CurrentCustomer?.CustomerMail;
                Address = CurrentCustomer?.CustomerAddress;
                BusinessName = CurrentCustomer?.CustomerOrganizationName;
                IsBusiness = !string.IsNullOrEmpty(CurrentCustomer?.CustomerOrganizationName);

                Title = "Editar cliente";
                TitleBtn = "Editar";
            }
        }
    }
}
