using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquipmentCommon.CommonEntities;
using EquipmentCommon.DTOs;
using EquipmentCommon.Mapping;
using EquipmentCommon.CommonInterfaces.Repositories;
using EquipmentCommon.CommonInterfaces.Services;

namespace EquipmentManagement.Application.Services
{
    public class EquipmentService 
    {
        private readonly IEquipmentRepository _repository;
        private readonly LoggerService _logger;

        public EquipmentService(IEquipmentRepository repository, LoggerService logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllEquipments()
        {
            var equipments = await _repository.GetAllAsync();
            return equipments.Select(e => e.ToDto());
        }

        public async Task<IEnumerable<EquipmentDto>> GetPartialEquipments()
        {
            var equipments = await _repository.GetPartialAsync();
            return equipments.Select(e => e.ToDto());
        }

        public async Task<EquipmentDto> GetEquipment(int id)
        {
            var equipment = await _repository.GetByIdAsync(id);
            return equipment?.ToDto();
        }

        public async Task<EquipmentDto> AddEquipment(EquipmentDto equipmentDto)
        {
            var equipment = equipmentDto.ToEntity();
            var addedEquipment = await _repository.AddAsync(equipment);
            await _logger.LogAsync("Add", $"Added equipment - Id: {equipment.Id} and Instalation: {equipment.Installation}");

            return addedEquipment.ToDto();
        }

        public async Task UpdateEquipment(EquipmentDto equipmentDto)
        {
            try
            {
                var equipment = equipmentDto.ToEntity();
                await _repository.UpdateAsync(equipment);

                await _logger.LogAsync("Update", $"Updated equipment: {equipment.Id} - {equipment.Installation}");
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("UpdateEquipment", ex.ToString() + "\n");
            }
        }

        public async Task RemoveEquipment(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                await _logger.LogAsync("Remove", $"Removed equipment with ID: {id}");
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("RemoveEquipment", ex.ToString() + "\n");
            }
            
        }

        public async Task ImportFromCsv(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            var importedCount = 0;

            foreach (var line in lines)
            {
                var values = line.Split(';');
                if (values.Length > 4)
                {
                    var equipmentDto = new EquipmentDto
                    {
                        Installation = values[0],
                        Batch = int.Parse(values[1]),
                        Operator = values[2],
                        Manufacturer = values[3],
                        Model = int.Parse(values[4]),
                        Version = int.Parse(values[5])
                    };

                    await AddEquipment(equipmentDto);
                    importedCount++;
                }
            }
            await _logger.LogAsync("Import CSV", $"Imported {importedCount} equipments from CSV: {filePath}");
        }
    }
}
