using System;

namespace Sample.Application.Read
{
    public interface IQueryStore
    {
        void DeletePerson(Guid personId);
        void InsertPerson(Guid tenantId, Guid personId, string personName, DateTimeOffset personRegistered);
        void UpdatePersonName(Guid personId, string personName);

        void DecreaseAccountBalance(Guid account, decimal amount);
        void IncreaseAccountBalance(Guid account, decimal amount);
        void InsertAccount(Guid tenantId, Guid accountId, string accountCode, string accountStatus, Guid personId);
        void UpdateAccountStatus(Guid account, string status);
        
        void InsertTransfer(Guid tenant, Guid transfer, string status, Guid fromAccount, Guid toAccount, decimal amount);
        void UpdateTransfer(Guid aggregateIdentifier, string status, string activity);

        void InsertUser(Guid user, string name, string password, string status);
        void UpdateUserStatus(Guid user, string status);
    }
}
