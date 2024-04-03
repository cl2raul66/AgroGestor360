﻿using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class BanksDTOExtension
{
    public static Bank ToBank(this BankDTO dto)
    {
        return new Bank
        {
            Id = string.IsNullOrEmpty(dto.Id) ? null : new ObjectId(dto.Id),
            Name = dto.Name
        };
    }

    public static BankDTO ToBankDTO(this Bank bank)
    {
        return new BankDTO
        {
            Id = bank.Id!.ToString(),
            Name = bank.Name
        };
    }

    public static BankAccountDTO ToBankAccountDTO(this BankAccount account)
    {
        return new BankAccountDTO
        {
            Number = account.Number,
            Alias = account.Alias,
            Beneficiary = account.Beneficiary,
            InstrumentType = account.InstrumentType
        };
    }

    public static BankAccount ToBankAccount(this BankAccountDTO dto)
    {
        return new BankAccount
        {
            Number = dto.Number,
            Alias = dto.Alias,
            Beneficiary = dto.Beneficiary,
            InstrumentType = dto.InstrumentType
        };
    }
}
