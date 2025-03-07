namespace EquipmentManagementWebApp.Server.Presentation.ViewModels
{
    public class EquipmentViewModel
    {
        public int Id { get; set; }
        public string Installation { get; set; }
        public int Batch { get; set; }
        public string Operator { get; set; }
        public string Manufacturer { get; set; }
        public int Model { get; set; }
        public int Version { get; set; }

        public string DisplayName => $"{Manufacturer} - Modelo {Model} (Versão {Version})";
    }
}
