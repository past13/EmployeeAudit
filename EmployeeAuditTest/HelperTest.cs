using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EmployeeAudit;
using EmployeeAudit.Entities;
using EmployeeAudit.Services;
using Moq;
using Xunit;

namespace EmployeeAuditTest
{
    public class HelperTest
    {

        private readonly Mock<IEmployeeService> _employeeService;
        public HelperTest()
        {
            _employeeService = new Mock<IEmployeeService>();
        }
        
        [Fact]
        public void ConvertStringArrayToDictionary_ReturnsDictionary()
        {
            var args = new List<string> { "--name", "John", "--age", "30" };

            var result = Helper.ConvertStringArrayToDictionary(args);

            Assert.NotNull(result);
            Assert.Equal("John", result["--name"]);
            Assert.Equal("30", result["--age"]);
        }

        [Fact]
        public void ConvertStringArrayToDictionary_ThrowsException_WhenArgsContainDuplicateParams()
        {
            var args = new List<string> { "--name", "John", "--age", "30", "--name", "Doe" };
            Assert.Throws<ArgumentException>(() => Helper.ConvertStringArrayToDictionary(args));
        }

        [Fact]
        public void ConvertStringArrayToDictionary_ThrowsException_WhenArgsContainMissingParamValues()
        {
            var args = new List<string> { "--name", "John", "--age" };
            Assert.Throws<ArgumentException>(() => Helper.ConvertStringArrayToDictionary(args));
        }

        [Fact]
        public void ObjectValidator_ReturnsTrue_WhenObjectIsValid()
        {
            var obj = new EmployeeEntity(_employeeService.Object);

            string[] properties = { nameof(EmployeeEntity.EmployeeId), nameof(EmployeeEntity.Salary), nameof(EmployeeEntity.Name) };
            
            obj.SetEmployeeId(1);
            obj.SetEmployeeName("John");
            obj.SetEmployeeSalary(5000);

            var result = Helper.ObjectValidator<EmployeeEntity>.Validate(obj, properties);

            Assert.Empty(result);
        }
        
        [Fact]
        public void ObjectValidator_ReturnsFalse_WhenObjectNameTooShort()
        {
            var obj = new EmployeeEntity(_employeeService.Object);
            
            string[] properties = { nameof(EmployeeEntity.EmployeeId), nameof(EmployeeEntity.Name), nameof(EmployeeEntity.Salary) };
            
            var result = Helper.ObjectValidator<EmployeeEntity>.Validate(obj, properties);

            var employeeIdErrorMessage = result.FirstOrDefault(x => x.MemberNames.FirstOrDefault() == nameof(EmployeeEntity.EmployeeId))?.ErrorMessage;
            var salaryErrorMessage = result.FirstOrDefault(x => x.MemberNames.FirstOrDefault() == nameof(EmployeeEntity.Salary))?.ErrorMessage;
            var nameErrorMessage = result.FirstOrDefault(x => x.MemberNames.FirstOrDefault() == nameof(EmployeeEntity.Name))?.ErrorMessage;

            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("The field EmployeeId must be between 1 and 2147483647.", employeeIdErrorMessage);
            Assert.Equal("The field Salary must be between 1 and 2147483647.", salaryErrorMessage);
            Assert.Equal("Property Name cannot be null", nameErrorMessage);
        }

        [Fact]
        public void ObjectValidator_ReturnsFalse_WhenObjectEmployeeIdSalaryNameIsNotValid()
        {
            var obj = new EmployeeEntity(_employeeService.Object);

            obj.SetEmployeeId(-1);
            obj.SetEmployeeSalary(-1);
            obj.SetEmployeeName("Jo");

            string[] properties = { nameof(EmployeeEntity.EmployeeId), nameof(EmployeeEntity.Name), nameof(EmployeeEntity.Salary) };
            
            var result = Helper.ObjectValidator<EmployeeEntity>.Validate(obj, properties);

            var employeeIdErrorMessage = result.FirstOrDefault(x => x.MemberNames.FirstOrDefault() == nameof(EmployeeEntity.EmployeeId))?.ErrorMessage;
            var salaryErrorMessage = result.FirstOrDefault(x => x.MemberNames.FirstOrDefault() == nameof(EmployeeEntity.Salary))?.ErrorMessage;
            var nameErrorMessage = result.FirstOrDefault(x => x.MemberNames.FirstOrDefault() == nameof(EmployeeEntity.Name))?.ErrorMessage;

            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("The field EmployeeId must be between 1 and 2147483647.", employeeIdErrorMessage);
            Assert.Equal("The field Salary must be between 1 and 2147483647.", salaryErrorMessage);
            Assert.Equal("The field Name must be a string with a minimum length of 3 and a maximum length of 50.", nameErrorMessage);
        }
    }
}