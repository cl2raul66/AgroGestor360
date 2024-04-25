﻿using AgroGestor360.Server.Models;
using LiteDB;
using System.Globalization;
using vCardLib.Models;

namespace AgroGestor360.Server.Tools.Extensions;

public static class CustomersDTOExtension
{
    public static DTO5_1 ToDTO5_1(this Customer entity)
    {
        return new DTO5_1
        {
            CustomerId = entity.Id!.ToString(),
            CustomerName = entity.Contact!.FormattedName,
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
                Organization = string.IsNullOrEmpty(dTO.CustomerOrganizationName) ? new vCardLib.Models.Organization(dTO.CustomerOrganizationName!, null, null) : null
            },
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
                Organization = string.IsNullOrEmpty(dTO.CustomerOrganizationName) ? new vCardLib.Models.Organization(dTO.CustomerOrganizationName!, null, null) : null
            },
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
            Birthday = entity.Contact!.BirthDay,
            CustomerPhone = entity.Contact?.PhoneNumbers.FirstOrDefault().Number,
            CustomerMail = entity.Contact?.EmailAddresses.FirstOrDefault().Value,
            CustomerNIP = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "NIP").Value,
            CustomerNIT = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "NIT").Value,
            CustomerAddress = entity.Contact?.CustomFields.FirstOrDefault(x => x.Key == "ADDRESS").Value,
            Discount = entity.Discount
        };
    }
}
