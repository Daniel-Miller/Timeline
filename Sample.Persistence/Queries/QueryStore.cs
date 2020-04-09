using System;
using System.Linq;

using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class QueryStore : IQueryStore
    {
        private string DatabaseConnectionString { get; set; }

        public QueryStore(string databaseConnectionString)
        {
            DatabaseConnectionString = databaseConnectionString;
        }

        #region Methods (Account)

        public void DecreaseAccountBalance(Guid accountId, decimal amount)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var account = db.AccountSummaries.Single(x => x.AccountIdentifier == accountId);
                account.AccountBalance -= amount;

                var person = db.PersonSummaries.Single(x => x.PersonIdentifier == account.OwnerIdentifier);
                person.TotalAccountBalance -= amount;

                db.SaveChanges();
            }
        }

        public void IncreaseAccountBalance(Guid accountId, decimal amount)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var account = db.AccountSummaries.Single(x => x.AccountIdentifier == accountId);
                account.AccountBalance += amount;

                var person = db.PersonSummaries.Single(x => x.PersonIdentifier == account.OwnerIdentifier);
                person.TotalAccountBalance += amount;

                db.SaveChanges();
            }
        }

        public void InsertAccount(Guid tenantId, Guid accountId, string accountCode, string accountStatus, Guid personId)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var account = new AccountSummary
                {
                    TenantIdentifier = tenantId,
                    AccountIdentifier = accountId,
                    AccountCode = accountCode,
                    AccountStatus = accountStatus,
                    OwnerIdentifier = personId
                };
                db.AccountSummaries.Add(account);
                db.SaveChanges();

                if (accountStatus == "Open")
                {
                    var person = db.PersonSummaries.Single(x => x.PersonIdentifier == personId);
                    person.OpenAccountCount++;
                    db.SaveChanges();
                }

                Denormalize();
            }
        }

        public void UpdateAccountStatus(Guid accountId, string accountStatus)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var account = db.AccountSummaries.Single(x => x.AccountIdentifier == accountId);
                account.AccountStatus = accountStatus;

                if (accountStatus != "Open")
                {
                    var person = db.PersonSummaries.Single(x => x.PersonIdentifier == account.OwnerIdentifier);
                    person.OpenAccountCount--;
                }

                db.SaveChanges();
            }
        }

        #endregion

        #region Methods (Person)

        public void DeletePerson(Guid personId)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var p = db.PersonSummaries.Single(x => x.PersonIdentifier == personId);
                db.PersonSummaries.Remove(p);
                db.SaveChanges();
            }
        }

        public void InsertPerson(Guid tenantId, Guid personId, string personName, DateTimeOffset personRegistered)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var summary = new PersonSummary
                {
                    TenantIdentifier = tenantId,
                    PersonIdentifier = personId,
                    PersonName = personName,
                    PersonRegistered = personRegistered
                };
                db.PersonSummaries.Add(summary);
                db.SaveChanges();
            }
        }

        public void UpdatePersonName(Guid personId, string personName)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var person = db.PersonSummaries.Single(x => x.PersonIdentifier == personId);
                person.PersonName = personName;
                db.SaveChanges();

                Denormalize();
            }
        }

        #endregion

        #region Methods (Transfer)

        public void InsertTransfer(Guid tenant, Guid transfer, string status, Guid fromAccount, Guid toAccount, decimal amount)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var summary = new TransferSummary
                {
                    TenantIdentifier = tenant,
                    TransferIdentifier = transfer,
                    TransferStatus = status,
                    TransferAmount = amount,
                    FromAccountIdentifier = fromAccount,
                    ToAccountIdentifier = toAccount
                };
                db.TransferSummaries.Add(summary);
                db.SaveChanges();
            }

            Denormalize();
        }

        public void UpdateTransfer(Guid aggregateIdentifier, string status, string activity)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var summary = db.TransferSummaries.Single(x => x.TransferIdentifier == aggregateIdentifier);
                summary.TransferStatus = status;
                summary.TransferActivity = activity;
                db.SaveChanges();
            }
        }

        #endregion

        #region Methods (User)

        public void InsertUser(Guid user, string name, string password, string status)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var summary = new UserSummary
                {
                    UserIdentifier = user,
                    LoginName = name,
                    LoginPassword = password,
                    UserRegistrationStatus = status
                };
                db.UserSummaries.Add(summary);
                db.SaveChanges();
            }
        }

        public void UpdateUserStatus(Guid user, string status)
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                var summary = db.UserSummaries.Single(x => x.UserIdentifier == user);
                summary.UserRegistrationStatus = status;
                db.SaveChanges();
            }
        }

        #endregion

        #region Methods (denormalization)

        /// <summary>
        /// Executes a stored procedure responsible for denormalization of query table rows. We try to avoid joins in
        /// the query database tables so that projections are as simple as possible, and so that search functions are 
        /// as fast as possible. The Denormalized stored procedure is expected to fill in any blanks or mismatches. 
        /// </summary>
        /// <example>
        /// Suppose we have a query table storing Account records. It contains a ContactIdentifier column and a 
        /// ContactName column. The event that inserts new Account records has a ContactIdentifier but not a 
        /// ContactName. We rely on the Denormalize procedure to update the ContactName after the Account row is 
        /// inserted.
        /// </example>
        private void Denormalize()
        {
            using (var db = new QueryDbContext(DatabaseConnectionString))
            {
                db.Database.ExecuteSqlCommand("EXEC queries.Denormalize");
            }
        }

        #endregion
    }
}
