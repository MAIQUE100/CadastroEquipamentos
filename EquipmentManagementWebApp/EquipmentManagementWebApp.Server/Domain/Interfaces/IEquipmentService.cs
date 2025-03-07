using EquipmentCommon.CommonEntities;
using EquipmentManagementWebApp.Server.Presentation.ViewModels;

namespace EquipmentManagementWebApp.Server.Domain.Interfaces
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentViewModel>> GetAllAsync();
        Task<EquipmentViewModel> GetByIdAsync(int id);
        Task<EquipmentViewModel> AddAsync(EquipmentViewModel entity);
        Task<bool> UpdateAsync(EquipmentViewModel entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ImportEquipmentsAsync(IEnumerable<EquipmentViewModel> entity);
    }
}
