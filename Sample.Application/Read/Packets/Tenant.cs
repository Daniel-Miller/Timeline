using System;

using Timeline.Identities;

namespace Sample.Application.Read
{
    public class Tenant : ITenant
    {
        public Tenant(Guid id, string name, int key) { Identifier = id; Name = name; Key = key; }

        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public string Code => Name.Substring(0, Name.IndexOf(' ')).ToLower();
        public int Key { get; set; }
    }
}
