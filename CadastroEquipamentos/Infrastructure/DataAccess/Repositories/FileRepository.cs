using EquipmentCommon.CommonEntities;
using EquipmentCommon.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Infrastructure.DataAccess.Repositories
{
    public class FileRepository
    {
        //private readonly string _filePath = "data.txt";

        //public void Add(Equipment equipment)
        //{
        //    File.AppendAllText(_filePath, FormatEquipment(equipment) + "\n");
        //}

        //public List<Equipment> GetAll()
        //{
        //    if (!File.Exists(_filePath)) return new List<Equipment>();
        //    return File.ReadAllLines(_filePath).Select(ParseEquipment).ToList();
        //}

        //public Equipment Get(string installation)
        //{
        //    return GetAll().FirstOrDefault(e => e.Installation == installation);
        //}

        //public void Remove(string installation)
        //{
        //    var equipments = GetAll().Where(e => e.Installation != installation).ToList();
        //    File.WriteAllLines(_filePath, equipments.Select(FormatEquipment));
        //}

        //public void Update(string installation, Equipment updatedEquipment)
        //{
        //    Remove(installation);
        //    Add(updatedEquipment);
        //}

        //private string FormatEquipment(Equipment equipment) =>
        //    $"{equipment.Installation};{equipment.Batch};{equipment.Operator};{equipment.Manufacturer};{equipment.Model};{equipment.Version}";

        //private Equipment ParseEquipment(string line)
        //{
        //    var values = line.Split(';');
        //    return new Equipment { Installation = values[0], Batch = values[1], Operator = values[2], Manufacturer = values[3], Model = values[4], Version = values[5] };
        //}
    }
}
