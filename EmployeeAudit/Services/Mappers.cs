using EmployeeAudit.Entities;

namespace EmployeeAudit.Services
{
    public static class Mappers
    {
        public static EmployeeViewModel MapEntityToDto(this Employee entity)
        {
            return new EmployeeViewModel
            {
                Id = entity.EmployeeId,
                Name = entity.Name,
                Salary = entity.Salary,
                ExistenceStartUtc = entity.ExistenceStartUtc,
                ExistenceEndUtc = entity.ExistenceEndUtc
            };
        }
        
        public static Employee MapDtoToEntity(this EmployeeEntity entity)
        {
            return new Employee
            {
                EmployeeId = entity.EmployeeId,
                Name = entity.Name,
                Salary = entity.Salary,
                ExistenceStartUtc = entity.ExistenceStartUtc,
                ExistenceEndUtc = entity.ExistenceEndUtc
            };
        }
    }
}