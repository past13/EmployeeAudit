using System;
using System.Threading.Tasks;

namespace EmployeeAudit.Entities
{
    public class EmployeeSnapshot {
        private readonly EmployeeEntity _baseEmployeeEntity;
        private readonly DateTime? _validDate;

        public EmployeeSnapshot(EmployeeEntity employee, DateTime? date) {
            _baseEmployeeEntity = employee;
            _validDate = date;
        }

        public async Task<EmployeeViewModel> GetEmployee() {
            return await _baseEmployeeEntity.GetEmployee(_validDate);
        }
    }
}