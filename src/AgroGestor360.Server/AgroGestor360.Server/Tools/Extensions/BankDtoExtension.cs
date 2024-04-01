using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class BankDTOExtension
{
    public static Bank ToBank(this BankDTO bankDto)
    {
        return new Bank
        {
            Id = string.IsNullOrEmpty(bankDto.Id) ? null : new ObjectId(bankDto.Id),
            Name = bankDto.Name
        };
    }
}
