using AgroGestor360Server.Models;
using LiteDB;
using System.Globalization;
using vCardLib.Models;

namespace AgroGestor360Server.Tools.Extensions;

public static class CustomersDTOExtension
{
    public static DTO5_1 ToDTO5_1(this Customer entity)
    {
        return new DTO5_1
        {
            CustomerId = entity.Id!.ToString(),
            CustomerName = string.IsNullOrEmpty(entity.Contact?.Organization?.Name)
                ? entity.Contact?.FormattedName
                : entity.Contact?.Organization?.Name,
            Credit = entity.Credit,
            Discount = entity.Discount
        };
    }

    public static Customer FromDTO5_2(this DTO5_2 dTO)
    {
        DateTime Date = DateTime.Now;

        Dictionary<string, string> customFields = string.IsNullOrEmpty(dTO.CustomerAddress)
            ? new() { { "NIP", dTO.CustomerNIP! }, { "NIT", dTO.CustomerNIT! }, { "REGISTRATIONDATE", Date.ToString()! } }
            : new() { { "NIP", dTO.CustomerNIP! }, { "NIT", dTO.CustomerNIT! }, { "REGISTRATIONDATE", Date.ToString()! }, { "ADDRESS", dTO.CustomerAddress!.Trim().ToUpper() } };
        return new Customer
        {
            Contact = new vCard(vCardLib.Enums.vCardVersion.v4)
            {
                Language = new Language(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
                FormattedName = dTO.CustomerFullName,
                PhoneNumbers = string.IsNullOrEmpty(dTO.CustomerPhone) ? new() : [new() { Number = dTO.CustomerPhone }],
                EmailAddresses = string.IsNullOrEmpty(dTO.CustomerMail) ? new() : [new() { Value = dTO.CustomerMail }],
                CustomFields = [.. customFields],
                Title = dTO.CustomerOccupation,
                BirthDay = dTO.Birthday,
                Organization = string.IsNullOrEmpty(dTO.CustomerOrganizationName) 
                    ? null
                    : new vCardLib.Models.Organization(dTO.CustomerOrganizationName!, null, null)
            },
            Credit = dTO.Credit,
            Discount = dTO.Discount
        };
    }
    
    public static Customer FromDTO5_3(this DTO5_3 dTO)
    {
        DateTime Date = DateTime.Now;

        Dictionary<string, string> customFields = string.IsNullOrEmpty(dTO.CustomerAddress)
            ? new() { { "NIP", dTO.CustomerNIP! }, { "NIT", dTO.CustomerNIT! }, { "REGISTRATIONDATE", Date.ToString()! } }
            : new() { { "NIP", dTO.CustomerNIP! }, { "NIT", dTO.CustomerNIT! }, { "REGISTRATIONDATE", Date.ToString()! }, { "ADDRESS", dTO.CustomerAddress!.Trim().ToUpper() } };
        return new Customer
        {
            Id = new ObjectId(dTO.CustomerId),
            Contact = new vCard(vCardLib.Enums.vCardVersion.v4)
            {
                Language = new Language(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
                FormattedName = dTO.CustomerFullName,
                PhoneNumbers = string.IsNullOrEmpty(dTO.CustomerPhone) ? new() : [new() { Number = dTO.CustomerPhone }],
                EmailAddresses = string.IsNullOrEmpty(dTO.CustomerMail) ? new() : [new() { Value = dTO.CustomerMail }],
                CustomFields = [.. customFields],
                Title = dTO.CustomerOccupation,
                BirthDay = dTO.Birthday,
                Organization = string.IsNullOrEmpty(dTO.CustomerOrganizationName)
                    ? null
                    : new vCardLib.Models.Organization(dTO.CustomerOrganizationName!, null, null)
            },
            Credit = dTO.Credit,
            Discount = dTO.Discount
        };
    }

    public static DTO5_3 ToDTO5_3(this Customer entity)
    {
        return new DTO5_3
        {
            CustomerId = entity.Id!.ToString(),
            CustomerFullName = entity.Contact?.FormattedName,
            CustomerOccupation = entity.Contact?.Title,
            CustomerOrganizationName = entity.Contact?.Organization?.Name,
            Birthday = entity.Contact!.BirthDay,
            CustomerPhone = entity.Contact?.PhoneNumbers.FirstOrDefault().Number,
            CustomerMail = entity.Contact?.EmailAddresses.FirstOrDefault().Value,
            CustomerNIP = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "NIP").Value,
            CustomerNIT = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "NIT").Value,
            CustomerAddress = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "ADDRESS").Value,
            Credit = entity.Credit,
            Discount = entity.Discount
        };
    }
}
