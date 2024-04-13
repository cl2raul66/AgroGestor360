using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class BanksDTOExtension
{
    public static Bank ToBank(this BankDTO dTO)
    {
        return new Bank
        {
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            Name = dTO.Name
        };
    }

    public static BankDTO ToBankDTO(this Bank entity)
    {
        return new BankDTO
        {
            Id = entity.Id!.ToString(),
            Name = entity.Name
        };
    }

    public static BankAccountDTO ToBankAccountDTO(this BankAccount entity)
    {
        return new BankAccountDTO
        {
            Number = entity.Number,
            Alias = entity.Alias,
            Beneficiary = entity.Beneficiary,
            InstrumentType = entity.InstrumentType
        };
    }

    public static BankAccount ToBankAccount(this BankAccountDTO dTO)
    {
        return new BankAccount
        {
            Number = dTO.Number,
            Alias = dTO.Alias,
            Beneficiary = dTO.Beneficiary,
            InstrumentType = dTO.InstrumentType
        };
    }
}
