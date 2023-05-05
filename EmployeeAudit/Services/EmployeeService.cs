using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using EmployeeAudit.Database;
using EmployeeAudit.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAudit.Services
{
    public interface IEmployeeService
    {
        Task SaveEmployee(EmployeeEntity employee);
        Task<EmployeeViewModel> GetEmployee(int id, DateTime? date);
    }
    
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _dbContext;
        public EmployeeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<EmployeeViewModel> GetEmployee(int id, DateTime? date)
        {
            if (_dbContext.Employees == null)
            {
                throw new InvalidOperationException();
            };
            
            var employeeResponse = await _dbContext.Employees
                .OrderByDescending(item => item.ExistenceStartUtc)
                .FirstOrDefaultAsync(e => 
                    e.EmployeeId == id 
                    && date.HasValue && e.ExistenceStartUtc.Date == date.Value.Date); 

            return employeeResponse.MapEntityToDto();

        }        
        
        public async Task SaveEmployee(EmployeeEntity employee)
        {
            var entity = employee.MapDtoToEntity();
            
            try
            {
                if (_dbContext.Employees == null)
                {
                    throw new InvalidOperationException();
                };
                
                var employeeResponse = await _dbContext.Employees
                    .OrderByDescending(e => e.ExistenceStartUtc)
                    .FirstOrDefaultAsync(e => e.EmployeeId == entity.EmployeeId);
                
                if (employeeResponse != null && employeeResponse.Salary != entity.Salary)
                {
                    var newDate = DateTime.UtcNow;
                    
                    var copy = employeeResponse.CreateCopy();
                    
                    copy.Salary = entity.Salary;
                    copy.ExistenceStartUtc = newDate;

                    _dbContext.Employees?.Add(copy);

                    employeeResponse.ExistenceEndUtc = newDate;
                }
                else
                {
                    entity.ExistenceStartUtc = DateTime.UtcNow;
                    _dbContext.Employees?.Add(entity);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                Console.WriteLine($"Employee Saved {employee}");
            }
        }
    }
}