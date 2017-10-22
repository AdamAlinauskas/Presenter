using System;
using System.Collections.Generic;
using System.Linq;

namespace Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running migrations...");

            IList<string> orgs;
            using (var sharedMigrator = GetSharedMigrator())
            {
                sharedMigrator.Migrate();
                orgs = sharedMigrator.GetOrgs().ToList();
            }
            
            foreach (var org in orgs)
            using (var orgMigrator = GetOrgMigrator(org)) {
                orgMigrator.Migrate();                
            }
        }

        private static SharedSchemaMigrator GetSharedMigrator()
        {
            var connector = new Connector();
            var dbAssistant = new DatabaseAssistant();
            return new SharedSchemaMigrator(connector, dbAssistant);
        }

        private static OrganizationMigrator GetOrgMigrator(string org)
        {
            var connector = new Connector();
            var dbAssistant = new DatabaseAssistant();
            return new OrganizationMigrator(connector, dbAssistant, org);
        }
    }
}
