using System;

namespace EmployeeAudit.Entities
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Salary { get; set; }
        public DateTime? ExistenceStartUtc { get; set; }
        public DateTime? ExistenceEndUtc { get; set; }
    }
}