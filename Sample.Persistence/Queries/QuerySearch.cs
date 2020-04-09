using System;
using System.Linq;

using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class QuerySearch : IQuerySearch
    {
        private string DatabaseConnectionString { get; set; }

        public QuerySearch(string databaseConnectionString)
        {
            DatabaseConnectionString = databaseConnectionString;
        }

        public bool IsUserRegistrationCompleted(Guid user)
        {
            var summary = SelectUserSummary(user);
            if (summary != null)
                return summary.UserRegistrationStatus == "Succeeded"
                    || summary.UserRegistrationStatus == "Failed";

            return false;
        }

        public AccountSummary SelectAccountSummary(Guid accountId)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                return db.AccountSummaries.Single(x => x.AccountIdentifier == accountId);
            }
        }

        public PersonSummary SelectPersonSummary(Guid personId)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                return db.PersonSummaries.Single(x => x.PersonIdentifier == personId);
            }
        }

        public TransferSummary SelectTransferSummary(Guid transfer)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                return db.TransferSummaries.SingleOrDefault(x => x.TransferIdentifier == transfer);
            }
        }

        public UserSummary SelectUserSummary(Guid user)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                return db.UserSummaries.SingleOrDefault(x => x.UserIdentifier == user);
            }
        }

        public bool UserExists(Func<UserSummary, bool> predicate)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                return db.UserSummaries.Any(predicate);
            }
        }
    }
}
