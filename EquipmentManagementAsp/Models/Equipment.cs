
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EquipmentManagementAsp.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Name("Instalacao")]
        public string Installation { get; set; }

        [Name("Lote")]
        public int Batch { get; set; }

        [Name("Operadora")]
        public string Operator { get; set; }

        [Name("Fabricante")]
        public string Manufacturer { get; set; }

        [Name("Modelo")]
        public int Model { get; set; }

        [Name("Versao")]
        public int Version { get; set; }
    }
}
