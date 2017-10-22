using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dapper;

namespace Migrator
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Running migrations...");

            if (!WaitForDatabaseToComeOnline())
                return 42;

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

            return 0;
        }

        private static bool WaitForDatabaseToComeOnline()
        {
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    var connector = new Connector();
                    using (var connection = connector.Connect("public"))
                        connection.Execute("SELECT 1 FROM pg_catalog.pg_database");
                    Console.WriteLine("Connected to database");
                    return true;
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine($"Connection attempt {i+1} failed. Retrying...");
                    Thread.Sleep(1000);
                }
            }

            return false;
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
