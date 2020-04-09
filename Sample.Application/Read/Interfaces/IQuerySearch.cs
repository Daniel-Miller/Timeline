using System;

namespace Sample.Application.Read
{
    public interface IQuerySearch
    {
        AccountSummary SelectAccountSummary(Guid account);
        PersonSummary SelectPersonSummary(Guid person);
        TransferSummary SelectTransferSummary(Guid transaction);
        UserSummary SelectUserSummary(Guid user);

        bool IsUserRegistrationCompleted(Guid user);
        bool UserExists(Func<UserSummary, bool> predicate);
    }
}
