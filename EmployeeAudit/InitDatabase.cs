using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

namespace EmployeeAudit
{
    public static class InitDatabase
    {
        // private static readonly string Password = Environment.GetEnvironmentVariable("AnalyticsPassword") 
        //                                           ?? throw new InvalidOperationException("You must set the AnalyticsPassword environment variable");
        // private static readonly string Database = Environment.GetEnvironmentVariable("AnalyticsDatabase") 
        //                                           ?? throw new InvalidOperationException("You must set the AnalyticsDatabase environment variable");
        // private static readonly string Port = Environment.GetEnvironmentVariable("AnalyticsPort") 
        //                                       ?? throw new InvalidOperationException("You must set the AnalyticsPort environment variable");
        // private static readonly string SchemaLocation = Environment.GetEnvironmentVariable("AnalyticsSchemaLocation") 
        //                                       ?? throw new InvalidOperationException("You must set the AnalyticsSchemaLocation environment variable");
        private const string Password = "guest";
        private const string Port = "7777";
        private const string Database = "analyticsproject";
        private const string SchemaLocation = @"C:\Users\vnol0096\Desktop\visma-tech-test-complex-domain-app-main\Test\DatabaseSchema\DatabaseSchema.csproj";

        public static async Task SetupDatabase()
        {
            Console.WriteLine("Waiting for database to start");
            await TestConnection();

            if (!await IsDatabaseExist())
            {
                await CreateDatabase();
                Console.WriteLine("Adding new database");
            }
            
            // Console.WriteLine("Adding database schema");
            // await ImportSchema();
        } 
        
        private static async Task TestConnection()
        {
            Exception? latestException = null;
            var then = DateTime.UtcNow;
            while (DateTime.UtcNow - then < TimeSpan.FromMinutes(0.2))
            {
                try
                {
                    await using var connection = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port};");
                    await connection.OpenAsync();
                    Console.WriteLine("Connection attempt succeeded");

                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Connection attempt failed");
                    latestException = e;
                    await Task.Delay(1000);
                }
            }

            throw new InvalidOperationException($"Could not connect to database", latestException);
        }
        
        private static async Task CreateDatabase()
        {
            await using var connection = new NpgsqlConnection($"Server=localhost;User ID=postgres; Password={Password}; Port={Port};");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE {Database}";

            await command.ExecuteNonQueryAsync();
        }

        private static async Task ImportSchema()
        {
            await using var connection = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port}; Database={Database};");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = await File.ReadAllTextAsync(SchemaLocation);

            await command.ExecuteNonQueryAsync();
        }

        private static async Task<bool> IsDatabaseExist()
        {
            var Password = "guest";
            var Port = "7777";
            var Database = "analyticsproject";

            await using var connection = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port}; Database={Database};");
            await connection.OpenAsync();
            
            var query = $"SELECT 1 FROM pg_database WHERE datname='{Database}'";

            await using var command = new NpgsqlCommand(query, connection);
            
            object result = command.ExecuteScalar();

            return result != null && result != DBNull.Value;
        }
    }
}