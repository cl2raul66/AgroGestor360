using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class BankDTOExtension
{
    public static Bank ToBank(this BankDTO bankDTO)
    {
        var r = string.IsNullOrEmpty(bankDTO.Id) ? null : new ObjectId(bankDTO.Id);
        return new Bank
        {
            Id = string.IsNullOrEmpty(bankDTO.Id) ? null : new ObjectId(bankDTO.Id),
            Name = bankDTO.Name
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
}
