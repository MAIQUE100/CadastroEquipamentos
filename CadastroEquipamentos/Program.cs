using EquipmentManagement.Application.Services;
using EquipmentManagement.Infrastructure.DataAccess.Repositories;
using EquipmentManagement.Presentation;

class Program
{
    [STAThread]
    static async Task Main(string[] args)
    {
        var equipmentService = new EquipmentService(new EquipmentRepository(), new LoggerService());
        var ui = new ConsoleUI(equipmentService);
        await ui.Run();
    }
}