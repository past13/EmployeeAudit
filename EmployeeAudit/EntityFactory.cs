using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EmployeeAudit.Entities;
using EmployeeAudit.Entities.Enums;
using EmployeeAudit.Services;

namespace EmployeeAudit
{
    public interface IEmployeeFactory
    {
        EmployeeEntity CreateEntity(Dictionary<string,string> properties);
    }

    public class GetEmployeeFactory : IEmployeeFactory
    {
        private readonly IEmployeeService _employeeService;
        public GetEmployeeFactory(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        
        public EmployeeEntity CreateEntity(Dictionary<string,string> properties)
        {
            var employee = new EmployeeEntity(_employeeService);
            
            var id = int.TryParse(
                properties.FirstOrDefault(kvp => 
                    kvp.Key.Equals("--employeeId", StringComparison.OrdinalIgnoreCase)).Value, out var employeeId)
                ? employeeId
                : (int?)null;

            var date = properties.FirstOrDefault(kvp =>
                kvp.Key.Equals("--simulatedTimeUtc", StringComparison.OrdinalIgnoreCase)).Value;

            var result = DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate)
                ? startDate
                : (DateTime?)null;
            
            if (id > 0) employee.SetEmployeeId((int)id);

            if (result is null)
            {
                employee.SetEmployeeStartDate(DateTime.UtcNow);
            }
            else 
            {
                employee.SetEmployeeStartDate((DateTime)result);
            }
            
            employee.SetEntityAction(EntityAction.GetEntity);
            
            return employee;
        }
    }

    public class CreateEmployeeFactory : IEmployeeFactory
    {
        private readonly IEmployeeService _employeeService;
        public CreateEmployeeFactory(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        
        public EmployeeEntity CreateEntity(Dictionary<string,string> properties)
        {
            var employee = new EmployeeEntity(_employeeService);
            
            var id = int.TryParse(
                properties.FirstOrDefault(kvp => 
                    kvp.Key.Equals("--employeeId", StringComparison.OrdinalIgnoreCase)).Value, out var employeeId)
                ? employeeId
                : (int?)null;

            var name = properties.FirstOrDefault(kvp =>
                kvp.Key.Equals("--employeeName", StringComparison.OrdinalIgnoreCase)).Value;

            var salary = int.TryParse(properties.FirstOrDefault(kvp =>
                kvp.Key.Equals("--employeeSalary", StringComparison.OrdinalIgnoreCase)).Value, out var employeeSalary)
                ? employeeSalary
                : (int?)null;
            
            if (id != null) employee.SetEmployeeId((int)id);
            if (name != null) employee.SetEmployeeName(name);
            if (salary != null) employee.SetEmployeeSalary((int)salary);
            
            employee.SetEntityAction(EntityAction.CreateEntity);
            
            return employee;
        }
    }

    public interface IEntityFactory
    {
        EmployeeEntity CreateEntity(string type, Dictionary<string,string> properties);
    }
    
    public class EntityFactory : IEntityFactory
    {
        private readonly IEmployeeService _employeeService;
        
        private readonly Dictionary<string, IEmployeeFactory> _factories;
        public EntityFactory(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _factories = new Dictionary<string, IEmployeeFactory>
            {
                { "set-employee", new CreateEmployeeFactory(_employeeService) },
                { "get-employee", new GetEmployeeFactory(_employeeService) }
            };
        }

        public EmployeeEntity CreateEntity(string type, Dictionary<string,string> properties)
        {
            if (_factories.TryGetValue(type, out IEmployeeFactory factory))
            {
                return factory.CreateEntity(properties);
            }

            throw new ArgumentException("Invalid command type.");
        }
    }
}