using AutoMapper;
using EquipmentManagementWebApp.Server.Presentation.ViewModels;
using EquipmentCommon.CommonEntities;

namespace EquipmentManagementWebApp.Server.Application.Mappers
{
    public class EquipmentViewModelProfile : Profile
    {
        public EquipmentViewModelProfile()
        {
            CreateMap<Equipment, EquipmentViewModel>().ReverseMap();
        }
    }
}
