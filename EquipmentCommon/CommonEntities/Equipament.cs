using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentCommon.CommonEntities
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Installation { get; set; }
        public int Batch { get; set; }
        public string Operator { get; set; }
        public string Manufacturer { get; set; }
        public int Model { get; set; }
        public int Version { get; set; }
    }
}
