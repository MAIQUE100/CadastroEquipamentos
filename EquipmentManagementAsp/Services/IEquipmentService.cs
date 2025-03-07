using EquipmentManagementAsp.Models;
using System.Threading.Tasks;

namespace EquipmentManagementAsp.Services
{
    public interface IEquipmentService
    {
        Task<(bool success, string message, List<Equipment> equipments)> ImportCSVAsync(IFormFile file);
    }
}
