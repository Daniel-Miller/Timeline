using System;
using System.Net.Mail;

using Sample.Application.Read;
using Sample.Domain;

using Timeline.Commands;
using Timeline.Events;

namespace Sample.Application.Write
{
    public class UmbrellaProcessManager
    {
        private IQuerySearch _querySearch;

        public UmbrellaProcessManager(ICommandQueue commander, IEventQueue publisher, IQuerySearch querySearch)
        {
            _querySearch = querySearch;

            publisher.Subscribe<TransferStarted>(Handle);
            commander.Override<RenamePerson>(Handle, Tenants.Umbrella.Identifier);
        }

        public void Handle(TransferStarted e)
        {
            if (e.Amount > 10000)
            {
                var from = _querySearch.SelectAccountSummary(e.FromAccount);
                var to = _querySearch.SelectAccountSummary(e.ToAccount);
                var alert = $"Money transfer started: {e.Amount:c2} from {from.AccountCode} to {to.AccountCode}";
                
                // var smtp = new SmtpClient();
                // smtp.Send("system@example.com", "oswell@example.com", "Large Money Transfer", alert);
            }
        }

        public void Handle(RenamePerson c)
        {
            // Do nothing. Umbrella Corporation does not permit renaming any person in their tenant account.
            
            // Throw an exception to make the consequences more severe for any attempt to rename a person...
            // throw new DisallowRenamePersonException();
        }
    }
}
