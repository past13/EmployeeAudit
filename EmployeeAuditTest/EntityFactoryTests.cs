using System;
using System.Collections.Generic;
using EmployeeAudit;
using EmployeeAudit.Entities;
using EmployeeAudit.Entities.Enums;
using EmployeeAudit.Services;
using Moq;
using Xunit;

namespace EmployeeAuditTest
{
    public class EntityFactoryTests
    {
        private readonly IEntityFactory _factory;

        public EntityFactoryTests()
        {
            var employeeServiceMock = new Mock<IEmployeeService>();
            _factory = new EntityFactory(employeeServiceMock.Object);
        }

        [Fact]
        public void CreateEntity_ReturnsGetEmployeeEntity_WhenTypeIsGetEmployee()
        {
            var properties = new Dictionary<string, string>
            {
                { "--employeeId", "123" },
                { "--simulatedTimeUtc", "2023-05-04T00:00:00Z" }
            };

            var result = _factory.CreateEntity("get-employee", properties);

            Assert.IsType<EmployeeEntity>(result);
            Assert.Equal(EntityAction.GetEntity, result.EntityAction);
            Assert.Equal(123, result.EmployeeId);
            Assert.Equal(new DateTime(2023, 5, 4), result.ExistenceStartUtc.Date);
        }
        
        [Fact]
        public void CreateEntity_ReturnsEmployeeEntityWithDefaultStartDate_WhenSimulatedTimeUtcIsNotProvided()
        {
            var properties = new Dictionary<string, string>
            {
                { "--employeeId", "123" }
            };

            var result = _factory.CreateEntity("get-employee", properties);

            Assert.IsType<EmployeeEntity>(result);
            Assert.Equal(EntityAction.GetEntity, result.EntityAction);
            Assert.Equal(123, result.EmployeeId);
            Assert.Equal(DateTime.UtcNow.Date, result.ExistenceStartUtc.Date);
        }

        [Fact]
        public void CreateEntity_ReturnsCreateEmployeeEntity_WhenTypeIsSetEmployee()
        {
            var properties = new Dictionary<string, string>
            {
                { "--employeeId", "123" },
                { "--employeeName", "John Doe" },
                { "--employeeSalary", "50000" }
            };

            var result = _factory.CreateEntity("set-employee", properties);

            Assert.IsType<EmployeeEntity>(result);
            Assert.Equal(EntityAction.CreateEntity, result.EntityAction);
            Assert.Equal(123, result.EmployeeId);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal(50000, result.Salary);
        }
    }
}