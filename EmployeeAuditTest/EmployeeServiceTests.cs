using System;
using System.Threading.Tasks;
using EmployeeAudit.Database;
using EmployeeAudit.Entities;
using EmployeeAudit.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeAuditTest
{
    public class EmployeeServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "EmployeeTestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _employeeService = new EmployeeService(_dbContext);
        }

        [Fact]
        public async Task GetEmployee_ReturnsEmployeeWithMatchingIdAndStartDate()
        {
            var existingEmployee = new Employee
            {
                EmployeeId = 1,
                Name = "John Doe",
                Salary = 50000,
                ExistenceStartUtc = DateTime.UtcNow.AddDays(-7)
            };
            
            _dbContext.Employees.Add(existingEmployee);
            await _dbContext.SaveChangesAsync();

            var result = await _employeeService.GetEmployee(existingEmployee.EmployeeId, existingEmployee.ExistenceStartUtc);

            Assert.Equal(existingEmployee.EmployeeId, result.Id);
            Assert.Equal(existingEmployee.Name, result.Name);
            Assert.Equal(existingEmployee.Salary, result.Salary);
            Assert.Equal(existingEmployee.ExistenceStartUtc, result.ExistenceStartUtc);
        }
        
        [Fact]
        public async Task GetEmployee_ReturnsNull_WhenEmployeeNotFound()
        {
            var employee = await _employeeService.GetEmployee(1, null);

            var withDefaultDate = new EmployeeViewModel { ExistenceStartUtc = new DateTime() };

            var entityEmpty = HelperMethods.AreEqual(withDefaultDate, employee);
            Assert.True(entityEmpty);
        }
    }
}