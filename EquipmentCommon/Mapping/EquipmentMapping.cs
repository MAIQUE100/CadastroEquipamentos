using EquipmentCommon.CommonEntities;
using EquipmentCommon.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentCommon.Mapping
{
    public static class EquipmentMapping
    {
        public static EquipmentDto ToDto(this Equipment equipment)
        {
            return new EquipmentDto
            {
                Id = equipment.Id,
                Installation = equipment.Installation,
                Batch = equipment.Batch,
                Operator = equipment.Operator,
                Manufacturer = equipment.Manufacturer,
                Model = equipment.Model,
                Version = equipment.Version
            };
        }

        public static Equipment ToEntity(this EquipmentDto dto)
        {
            return new Equipment
            {
                Id = dto.Id,
                Installation = dto.Installation,
                Batch = dto.Batch,
                Operator = dto.Operator,
                Manufacturer = dto.Manufacturer,
                Model = dto.Model,
                Version = dto.Version
            };
        }
    }
}
