using CsvHelper.Configuration;
using EquipmentManagementAsp.Models;

namespace EquipmentManagementAsp.Mappings
{
    public class EquipmentMap : ClassMap<Equipment>
    {
        public EquipmentMap()
        {
            // Mapeamento das colunas do CSV para as propriedades da classe Equipment
            Map(m => m.Installation).Name("Instalacao");
            Map(m => m.Batch).Name("Lote");
            Map(m => m.Operator).Name("Operador");
            Map(m => m.Manufacturer).Name("Fabricante");
            Map(m => m.Model).Name("Modelo");
            Map(m => m.Version).Name("Versao");
        }
    }
}
