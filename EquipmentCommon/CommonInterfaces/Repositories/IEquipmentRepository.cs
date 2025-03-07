using EquipmentCommon.CommonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentCommon.CommonInterfaces.Repositories
{
    public interface IEquipmentRepository : IRepositoryBase<Equipment>
    {
        Task<IEnumerable<Equipment>> GetPartialAsync();
    }
}