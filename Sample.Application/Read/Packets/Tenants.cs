using System;
using System.Collections.Generic;

namespace Sample.Application.Read
{
    /// <summary>
    /// Implements a hard-coded list of tenants. Ordinarily this would be stored in a database table or in a file 
    /// external to the code, of course.
    /// </summary>
    public static class Tenants
    {
        private static readonly List<Tenant> _data;

        static Tenants()
        {
            var a = Guid.Parse("46bdba73-6c32-4d18-999c-ec0bc3ec7310");
            var b = Guid.Parse("b0180776-7f93-4db1-b452-f878bd046396");
            var c = Guid.Parse("9d3cc36b-31ba-4875-b1fb-71bc21c4c99b");

            _data = new List<Tenant>
            {
                new Tenant(a, "Acme Corporation", 1),
                new Tenant(b, "Damage, Inc.", 2),
                new Tenant(c, "Umbrella Corporation", 3)
            };
        }

        public static Tenant Acme => _data[0];
        public static Tenant Damage => _data[1];
        public static Tenant Umbrella => _data[2];
    }
}
