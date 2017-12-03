using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AddSampleData
{
    class Program
    {
        private string connectionString;
        private long presentation;
        private DateTime from;
        private int dateRange;
        private int count;
        private Location[] locations;
        private Random rng;
        private long[] users;
        private NpgsqlConnection connection;

        private class Location
        {
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string Continent { get; set; }
        }

        private class Session : Location
        {
            public DateTime CreatedAt { get; set; }
            public long CreatedBy { get; set; }
            public long PresentationId { get; set; }

            public Session(Location location)
            {
                Latitude = location.Latitude;
                Longitude = location.Longitude;
                City = location.City;
                Country = location.Country;
                Continent = location.Continent;
            }

            public override string ToString()
            {
                return $"Latitude: {Latitude}, Longitude: {Longitude}, City: {City}, Country: {Country}, Continent: {Continent}, CreatedAt {CreatedAt}, CreatedBy {CreatedBy}";
            }
        }

        static int Main(string[] args)
        {
            if (!File.Exists("appsettings.json"))
            {
                Console.WriteLine("Create an appsettings.json");
                Console.WriteLine("See appsettings.sample.json for an example");
                return 1;
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            new Program().Run(configuration);

            return 0;
        }

        void Run(IConfigurationRoot configuration)
        {
            rng = new Random();
            LoadConfiguration(configuration);

            users = LoadUsers();

            var sessions = Enumerable.Range(0, count).Select(GenerateSession);

            foreach (var session in sessions)
            {
                connection.Execute(
                    @"INSERT INTO user_analytics_sessions
                    (presentation_id, created_at, created_by, longitude, latitude, country, city, continent)
                    VALUES
                    (@PresentationId, @CreatedAt, @CreatedBy, @Longitude, @Latitude, @Country, @City, @Continent)",
                    session
                );
            }

            Console.WriteLine($"Added {count} sessions");
        }

        private void LoadConfiguration(IConfigurationRoot configuration)
        {
            connectionString = configuration["connectionString"];
            presentation = long.Parse(configuration["presentation"]);
            from = DateTime.Parse(configuration["from"]);
            var to = DateTime.Parse(configuration["to"]);
            dateRange = (to - from).Days;
            locations = LoadLocations(configuration["locationsFile"]);
            count = int.Parse(configuration["count"]);
        }

        private Location[] LoadLocations(string file)
        {
            return File
                .ReadAllLines(file)
                .Select(line => line.Split(","))
                .Select(line => new Location
                {
                    City = line[0],
                    Country = line[1],
                    Continent = line[2],
                    Latitude = float.Parse(line[3]),
                    Longitude = float.Parse(line[4])
                }).ToArray();
        }

        private long[] LoadUsers()
        {
            connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection
                .Query("SELECT id FROM meetaroo_shared.users")
                .Select(result => (long)result.id)
                .ToArray();
        }

        private Session GenerateSession(int sessionN)
        {
            var location = locations[rng.Next(locations.Length)];

            var session = new Session(location) {
                CreatedAt = from.AddDays(rng.Next(dateRange) + rng.NextDouble()),
                CreatedBy = users[rng.Next(users.Length)],
                PresentationId = presentation
            };

            session.Latitude += (float)(rng.NextDouble() -.5) * .1f;
            session.Longitude += (float)(rng.NextDouble() -.5) * .1f;

            return session;
        }
    }
}
