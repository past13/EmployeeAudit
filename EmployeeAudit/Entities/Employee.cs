using System;

namespace EmployeeAudit.Entities
{
    public interface IPrototype<T>
    {
        T CreateCopy();
    }
    
    public class Employee : IPrototype<Employee>
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Salary { get; set; }
        public DateTime ExistenceStartUtc { get; set; }
        public DateTime? ExistenceEndUtc { get; set; }
        
        public Employee CreateCopy()
        {
            return new Employee
            {
                EmployeeId = EmployeeId,
                Name = Name,
                Salary = Salary,
                ExistenceStartUtc = ExistenceStartUtc,
                ExistenceEndUtc = ExistenceEndUtc
            };
        }
    }
}