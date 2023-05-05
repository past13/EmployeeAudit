using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmployeeAudit.Entities.Enums;
using EmployeeAudit.Services;

namespace EmployeeAudit.Entities
{
    public class EmployeeEntity 
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeEntity(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        
        [Range(1, int.MaxValue)]
        public int EmployeeId { get; private set; }

        [StringLength(50, MinimumLength = 3)] public string Name { get; private set; } = string.Empty;
        
        [Range(1, int.MaxValue)]
        public int Salary { get; private set; }
        public EntityAction EntityAction { get; private set; }
        public DateTime ExistenceStartUtc { get; private set; }
        public DateTime? ExistenceEndUtc { get;  set; }
        public void SetEmployeeName(string name) => Name = name;
        public void SetEmployeeSalary(int salary) => Salary = salary;
        public void SetEmployeeStartDate(DateTime starDate) => ExistenceStartUtc = starDate;
        public void SetEmployeeId(int id) => EmployeeId = id;
        public void SetEntityAction(EntityAction type) => EntityAction = type;
        
        public EmployeeSnapshot CreateSnapshot(DateTime? validDate) {
            return new EmployeeSnapshot(this, validDate);
        }
        
        public async Task PutEmployee()
        {
            await _employeeService.SaveEmployee(this); 
        }
        
        public async Task<EmployeeViewModel> GetEmployee(DateTime? date)
        {
            return await _employeeService.GetEmployee(EmployeeId, date);
        }
    }
}