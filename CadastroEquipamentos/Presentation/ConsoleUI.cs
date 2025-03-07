using EquipmentCommon.CommonEntities;
using EquipmentCommon.CommonInterfaces;
using EquipmentCommon.DTOs;
using EquipmentManagement.Application.Services;
using Mysqlx.Expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EquipmentManagement.Presentation
{
    public class ConsoleUI
    {
        private readonly EquipmentService _service;

        public ConsoleUI(EquipmentService service)
        {
            _service = service;
        }

        public async Task Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("   GERENCIAMENTO DE EQUIPAMENTOS    ");
                Console.WriteLine("====================================");
                Console.WriteLine(" I. Adicionar Equipamento");
                Console.WriteLine(" L. Listar Todos os Equipamentos");
                Console.WriteLine(" C. Consultar Simples");
                Console.WriteLine(" E. Atualizar Equipamento");
                Console.WriteLine(" D. Remover Equipamento");
                Console.WriteLine(" M. Importar de CSV");
                Console.WriteLine(" X. Sair");
                Console.WriteLine("====================================");
                Console.Write(" Escolha uma opção: ");

                var choice = Console.ReadLine().ToUpper();
                Console.Clear();

                if (choice == "X") break;

                switch (choice)
                {
                    case "I":
                        await AddEquipment();
                        break;
                    case "L":
                        await ViewAllEquipment();
                        break;
                    case "C":
                        await ViewOneEquipment();
                        break;
                    case "D":
                        await RemoveEquipment();
                        break;
                    case "E":
                        await UpdateEquipment();
                        break;
                    case "M":
                        await ImportCsv();
                        break;
                    default:
                        Console.WriteLine("\n❌ Opção inválida. Tente novamente.");
                        break;
                }
                Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
                Console.ReadKey();
            }
        }

        private async Task AddEquipment()
        {
            Console.WriteLine("--- Adicionar Novo Equipamento ---");
            Console.Write("Instalação: ");
            var installation = Console.ReadLine();
            int batch = ReadIntInput("Lote");
            Console.Write("Operador: ");
            var operatorName = Console.ReadLine();
            Console.Write("Fabricante: ");
            var manufacturer = Console.ReadLine();
            int model = ReadIntInput("Modelo");
            int version = ReadIntInput("Versão");

            var equipmentDto = new EquipmentDto
            {
                Installation = installation,
                Batch = batch,
                Operator = operatorName,
                Manufacturer = manufacturer,
                Model = model,
                Version = version
            };

            await _service.AddEquipment(equipmentDto);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("\n✔ Equipamento adicionado com sucesso!");
        }

        private int ReadIntInput(string fieldName)
        {
            int value;
            while (true)
            {
                Console.Write($"{fieldName}: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out value))
                    return value;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine("❌ Entrada inválida. Digite um número inteiro válido.");
                Console.ResetColor();
            }
        }

        private async Task ViewAllEquipment()
        {
            var equipments = await _service.GetAllEquipments();

            if (!equipments.Any())
            {
                Console.WriteLine("\nNenhum equipamento encontrado.");
                return;
            }

            Console.WriteLine("\n--- Lista de Equipamentos ---\n");
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("| ID  | Instalação  | Lote | Operador | Fabricante | Modelo | Versão |");
            Console.WriteLine("----------------------------------------------------------------------");

            foreach (var equipment in equipments)
            {
                Console.WriteLine($"| {equipment.Id,-3} | {equipment.Installation,-12} | {equipment.Batch,-5} | {equipment.Operator,-8} | {equipment.Manufacturer,-12} | {equipment.Model,-7} | {equipment.Version,-7} |");
                Console.WriteLine("------------------------------------------------------------------");
            }
        }

        private async Task ViewOneEquipment()
        {
            Console.Write("Consulta simples: ");
            
            var equipments = await _service.GetAllEquipments();
            if (equipments != null)
            {
                Console.WriteLine("\n--- Equipment List ---\n\n");
                Console.WriteLine("-----------------------");
                Console.WriteLine("| Instalação  | Lote |");
                Console.WriteLine("----------------------");
                foreach (var equipment in equipments)
                {
                   Console.WriteLine($"| {equipment.Installation,-12} | {equipment.Batch.ToString(),-5} |");
                   Console.WriteLine("-------------------------");
                }    
            }
            else
            {
                Console.WriteLine("\n Equipment not found.");
            }
       
        }

        private async Task UpdateEquipment()
        {
            Console.Write("Enter Equipment ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var equipment = await _service.GetEquipment(id);
                if (equipment == null)
                {
                    Console.WriteLine("\n Equipment not found.");
                    return;
                }

                Console.Write($"New Installation (current: {equipment.Installation}): ");
                var newInstallation = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newInstallation))
                {
                    equipment.Installation = newInstallation;
                }

                equipment.Batch = ReadIntInput($"New Batch (current: {equipment.Batch}): ");
                

                Console.Write($"New Operator (current: {equipment.Operator}): ");
                var newOperator = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newOperator))
                {
                    equipment.Operator = newOperator;
                }

                Console.Write($"New Manufacturer (current: {equipment.Manufacturer}): ");
                var newManufacturer = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newManufacturer))
                {
                    equipment.Manufacturer = newManufacturer;
                }

                //Console.Write($"New Model (current: {equipment.Model}): ");
                //var newModel = Console.ReadLine();
                int model = ReadIntInput($"New Model (current: {equipment.Model}): ");
                equipment.Model = model;
                

                //Console.Write($"New Version (current: {equipment.Version}): ");
                //var newVersion = Console.ReadLine();
                int version = ReadIntInput($"New Version (current: {equipment.Version}): ");
                
                equipment.Version = version;
                

                await _service.UpdateEquipment(equipment);
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine($"\n✔  Equipment {equipment.Id} updated successfully.");
            }
            else
            {
                Console.WriteLine("\n Invalid ID.");
            }
        }

        private async Task RemoveEquipment()
        {
            Console.Write("Informe o ID do equipamento a ser removido: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var equipment = await _service.GetEquipment(id);
                if (equipment == null)
                {
                    Console.WriteLine("\nEquipamento não encontrado.");
                    return;
                }
                await _service.RemoveEquipment(id);
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine($"\n✔  Equipamento {equipment.Id} removido com sucesso.");
            }
            else
            {
                Console.WriteLine("\n❌ ID inválido.");
            }
        }

        private async Task ImportCsv()
        {

            string filePath = await Task.Run(() =>
            {
                string result = string.Empty;

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    openFileDialog.Title = "Select a CSV File";
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    Thread staThread = new Thread(() =>
                    {
                        var resultDialog = openFileDialog.ShowDialog();
                        if (resultDialog == DialogResult.OK)
                        {
                            result = openFileDialog.FileName;
                        }
                    });

                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    staThread.Join();
                }
                return result;
            });

            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("\nInvalid file path or operation cancelled.");
            }
            else
            {
                Console.WriteLine($"Selected file: {filePath}");
                await _service.ImportFromCsv(filePath);
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine($"\n✔  Equipments .CSV imported successfully.");
            }
        }
    }
}
