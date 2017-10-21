using System;

namespace Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running migrations...");

            var connector = new Connector();
            var migrator = new DatabaseMigrator();
            new SharedSchemaMigrator(connector, migrator).Migrate();
            // • Connect to shared schema
            // • Migrate and get list of orgs
            // • For each org schema
            //   ○ Connect and migrate
        }
    }
}
