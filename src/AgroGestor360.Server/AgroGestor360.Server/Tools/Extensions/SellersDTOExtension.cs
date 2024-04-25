using AgroGestor360.Server.Models;
using LiteDB;
using System.Globalization;
using vCardLib.Models;

namespace AgroGestor360.Server.Tools.Extensions;

public static class SellersDTOExtension
{
    public static DTO6 ToDTO6(this Seller entity)
    {
        return new DTO6
        {
            Id = entity.Id?.ToString(),
            FullName = entity.Contact?.FormattedName,
        };
    }

    public static Seller FromDTO6_1(this DTO6_1 dTO)
    {
        DateTime Date = DateTime.Now;

        Dictionary<string, string> sellerFields = string.IsNullOrEmpty(dTO.Address)
            ? new() { { nameof(dTO.NIP), dTO.NIP! }, { nameof(dTO.NIT), dTO.NIT! }, { "REGISTRATIONDATE", Date.ToString()! } }
            : new() { { nameof(dTO.NIP), dTO.NIP! }, { nameof(dTO.NIT), dTO.NIT! }, { "REGISTRATIONDATE", Date.ToString()! }, { "ADDRESS", dTO.Address!.Trim().ToUpper() } };
        return new Seller
        {
            Contact = new vCard(vCardLib.Enums.vCardVersion.v4)
            {
                Language = new Language(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
                FormattedName = dTO.FullName,
                PhoneNumbers = string.IsNullOrEmpty(dTO.Phone) ? new() : [new() { Number = dTO.Phone }],
                EmailAddresses = string.IsNullOrEmpty(dTO.Mail) ? new() : [new() { Value = dTO.Mail }],
                CustomFields = [.. sellerFields],
                Title = dTO.Occupation,
                BirthDay = dTO.Birthday
            }
        };
    }

    public static Seller FromDTO6_2(this DTO6_2 dTO)
    {
        DateTime Date = DateTime.Now;

        Dictionary<string, string> sellerFields = string.IsNullOrEmpty(dTO.Address)
            ? new() { { nameof(dTO.NIP), dTO.NIP! }, { nameof(dTO.NIT), dTO.NIT! }, { "REGISTRATIONDATE", Date.ToString()! } }
            : new() { { nameof(dTO.NIP), dTO.NIP! }, { nameof(dTO.NIT), dTO.NIT! }, { "REGISTRATIONDATE", Date.ToString()! }, { "ADDRESS", dTO.Address!.Trim().ToUpper() } };
        return new Seller
        {
            Id = string.IsNullOrEmpty(dTO.Id) ? ObjectId.NewObjectId() : new(dTO.Id),
            Contact = new vCard(vCardLib.Enums.vCardVersion.v4)
            {
                Language = new Language(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
                FormattedName = dTO.FullName,
                PhoneNumbers = string.IsNullOrEmpty(dTO.Phone) ? new() : [new() { Number = dTO.Phone }],
                EmailAddresses = string.IsNullOrEmpty(dTO.Mail) ? new() : [new() { Value = dTO.Mail }],
                CustomFields = [.. sellerFields],
                Title = dTO.Occupation,
                BirthDay = dTO.Birthday
            }
        };
    }

    public static DTO6_2 ToDTO6_2(this Seller entity)
    {
        return new DTO6_2
        {
            Id = entity.Id?.ToString(),
            FullName = entity.Contact?.FormattedName,
            Birthday = entity.Contact?.BirthDay,
            Occupation = entity.Contact?.Title,
            Phone = entity.Contact?.PhoneNumbers?.FirstOrDefault().Number,
            Mail = entity.Contact?.EmailAddresses?.FirstOrDefault().Value,
            NIP = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "NIP").Value,
            NIT = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "NIT").Value,
            Address = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "ADDRESS").Value
        };
    }
}
