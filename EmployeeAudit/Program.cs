using System;
using System.Collections.Generic;
// using System.CommandLine;
// using System.CommandLine.Invocation;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EmployeeAudit.Database;
using EmployeeAudit.Entities;
using EmployeeAudit.Entities.Enums;
using EmployeeAudit.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeAudit
{
    class Program
    {
        private static ServiceCollection AddServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IEntityFactory, EntityFactory>();

            const string connectionString = "Server=localhost;Database=analyticsproject;User ID=postgres;Password=guest;Port=7777";
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
            
            return services;
        }
        
        static async Task Main(string[] args)
        {
            await InitDatabase.SetupDatabase();

            var services = AddServices();
            var serviceProvider = services.BuildServiceProvider();
            
            var factoryService = serviceProvider.GetService<IEntityFactory>();
            
            var command = args.Take(1).FirstOrDefault();
            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine("Input can not be empty");
                return;
            }

            if (!command.Contains("set-employee") && !command.Contains("get-employee"))
            {
                Console.WriteLine("Command does not exist");
                return;
            }
            
            var dictionaryParams = Helper.ConvertStringArrayToDictionary(args.Skip(1).ToArray());
            if (dictionaryParams.Count == 0)
            {
                Console.WriteLine("Arguments can not be empty");
                return;
            }

            var employee = factoryService.CreateEntity(command, dictionaryParams);

            List<ValidationResult> validationResults;
            
            if (employee.EntityAction == EntityAction.CreateEntity)
            {
                string[] requiredFields =
                {
                    nameof(EmployeeEntity.EmployeeId), 
                    nameof(EmployeeEntity.Salary), 
                    nameof(EmployeeEntity.Name)
                };
                
                validationResults = Helper.ObjectValidator<EmployeeEntity>.Validate(employee, requiredFields);
                if (validationResults.Any())
                {
                    Helper.ErrorMessage(validationResults);
                }
                
                await employee.PutEmployee();
            }
            
            if (employee.EntityAction == EntityAction.GetEntity)
            {
                string[] requiredFields = { nameof(EmployeeEntity.EmployeeId) };
                validationResults = Helper.ObjectValidator<EmployeeEntity>.Validate(employee, requiredFields);
                if (validationResults.Any())
                {
                    Helper.ErrorMessage(validationResults);
                }
                
                EmployeeSnapshot snapshot = employee.CreateSnapshot(employee.ExistenceStartUtc);
            
                var result = await snapshot.GetEmployee();
                
                Console.WriteLine(
                    $"Employee: Id: {result.Id}, Name: {result.Name}, Salary: {result.Salary}, Start Date: {result.ExistenceStartUtc}, End Date: {result.ExistenceEndUtc} ");
            }
            
            Console.ReadLine();
        }
    }
}

