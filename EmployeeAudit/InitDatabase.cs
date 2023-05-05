using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace EmployeeAudit
{
    public static class InitDatabase
    {
        private static string _connectionString = "";
        private static string _database = "";

        public static async Task SetupDatabase(IConfigurationRoot config, string projectDirectory)
        {
            _connectionString = config.GetSection("ConnectionStrings")["DefaultConnection"];
            _database = config.GetValue<string>("Database");
            
            Console.WriteLine("Waiting for database to start");
            await TestConnection();

            if (!await IsDatabaseExist())
            {
                await CreateDatabase();
                Console.WriteLine("Adding new database");
            }
            
            await ImportSchema(projectDirectory, config);
        } 
        
        private static async Task TestConnection()
        {
            Exception? latestException = null;
            var then = DateTime.UtcNow;
            while (DateTime.UtcNow - then < TimeSpan.FromMinutes(0.2))
            {
                try
                {
                    await using var connection = new NpgsqlConnection(_connectionString);
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
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE {_database}";

            await command.ExecuteNonQueryAsync();
        }

        private static async Task ImportSchema(string projectDirectory, IConfiguration config)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
           
            var schemaName = config.GetValue<string>("SchemaName");
            var tableName = config.GetValue<string>("TableName");

            var command = connection.CreateCommand();

            var tableExistsSql = $"SELECT EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = '{schemaName}' AND table_name = '{tableName}')";
            await using var tableExistsCommand = new NpgsqlCommand(tableExistsSql, connection);
            var tableExists = (bool)await tableExistsCommand.ExecuteScalarAsync();

            if (!tableExists)
            {
                Console.WriteLine("Adding database schema");
                Console.WriteLine("Creating table");
                command.CommandText = await File.ReadAllTextAsync($"{projectDirectory}\\Database\\dbSchema.sql");
                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task<bool> IsDatabaseExist()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var query = $"SELECT 1 FROM pg_database WHERE datname='{_database}'";

            await using var command = new NpgsqlCommand(query, connection);
            
            object result = command.ExecuteScalar();

            return result != null && result != DBNull.Value;
        }
    }
}