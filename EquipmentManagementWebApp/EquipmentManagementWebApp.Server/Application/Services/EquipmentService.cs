using AutoMapper;
using EquipmentCommon.CommonEntities;
using EquipmentCommon.CommonInterfaces.Repositories;
using EquipmentCommon.CommonInterfaces.Services;
using EquipmentManagementWebApp.Server.Domain.Interfaces;
using EquipmentManagementWebApp.Server.Presentation.ViewModels;
using Org.BouncyCastle.Crypto;


namespace EquipmentManagementWebApp.Server.Application.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IMapper _mapper; 

        public EquipmentService(IEquipmentRepository equipmentRepository, IMapper mapper)
        {
            _equipmentRepository = equipmentRepository;
            _mapper = mapper; 
        }

        
        public async Task<IEnumerable<EquipmentViewModel>> GetAllAsync()
        {
            var equipments = await _equipmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EquipmentViewModel>>(equipments); 
        }

        public async Task<EquipmentViewModel> GetByIdAsync(int id)
        {
            var equipment = await _equipmentRepository.GetByIdAsync(id);
            return _mapper.Map<EquipmentViewModel>(equipment);
        }

       
        public async Task<EquipmentViewModel> AddAsync(EquipmentViewModel equipmentViewModel)
        {
            var equipment = _mapper.Map<Equipment>(equipmentViewModel); 
            var addedEquipment =  await _equipmentRepository.AddAsync(equipment);
            var addedEquipmentViewModel = _mapper.Map<EquipmentViewModel>(addedEquipment);

            return addedEquipmentViewModel;
        }

       
        public async Task<bool> UpdateAsync(EquipmentViewModel equipmentViewModel)
        {
            var equipment = _mapper.Map<Equipment>(equipmentViewModel);
            return await _equipmentRepository.UpdateAsync(equipment);
        }

        
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var equipment = await GetByIdAsync(id);

                if (equipment == null)
                {
                    
                    return false;
                }

                
                return await _equipmentRepository.DeleteAsync(id);

            }
            catch (Exception)
            {
                return true;
            }
        }

        public async Task<bool> ImportEquipmentsAsync(IEnumerable<EquipmentViewModel> equipmentViewModels)
        {
            foreach (var equipmentViewModel in equipmentViewModels)
            {
                var equipment = _mapper.Map<Equipment>(equipmentViewModel);
                var result = await _equipmentRepository.AddAsync(equipment);
                if (result == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}