using CsvHelper.Configuration;
using CsvHelper;
using EquipmentManagementAsp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using EquipmentManagementAsp.Mappings;
using FluentValidation;
using System.Text;

namespace EquipmentManagementAsp.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IValidator<Equipment> _validator;

        public EquipmentService(IValidator<Equipment> validator)
        {
            _validator = validator;
        }

        public async Task<(bool success, string message, List<Equipment> equipments)> ImportCSVAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Por favor, selecione um arquivo CSV.", null);
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Por favor, selecione um arquivo CSV válido.", null);
            }

            try
            {
                using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using var csv = new CsvReader(reader, config);

                var records = csv.GetRecords<Equipment>().ToList();

                if (!records.Any())
                {
                    return (false, "O arquivo CSV está vazio ou os dados não estão corretos.", null);
                }

                var errors = new List<string>();
                foreach (var record in records)
                {
                    var validationResult = _validator.Validate(record);
                    if (!validationResult.IsValid)
                    {
                        return (false, "Erro na validação dos dados:\n" + validationResult.Errors.First().ErrorMessage, null);
                    }
                }

                if (errors.Any())
                {
                    return (false, "Erro na validação dos dados:\n" + string.Join("\n", errors), null);
                }

                return (true, "Arquivo CSV importado com sucesso!", records);
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao processar o arquivo: {ex.Message}", null);
            }
        }

        //public async Task<(string, List<Equipment>)> ImportCSVAsync(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return ("Por favor, selecione um arquivo CSV.", null);
        //    }

        //    if (!file.FileName.EndsWith(".csv"))
        //    {
        //        return ("Por favor, selecione um arquivo CSV válido.", null);
        //    }

        //    List<Equipment> records = new List<Equipment>();
        //    List<string> validColumns = new List<string>
        //    {
        //        "Installation", "Batch", "Operator", "Manufacturer", "Model", "Version"
        //    };

        //    try
        //    {
        //        using (var reader = new StreamReader(file.OpenReadStream()))
        //        {
        //            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        //            {
        //                Delimiter = ";",
        //                HasHeaderRecord = true, // Considera que pode ter cabeçalho
        //                HeaderValidated = null, // Desativa a validação automática de cabeçalho
        //                MissingFieldFound = null
        //            };

        //            using (var csv = new CsvReader(reader, config))
        //            {
        //                // Tenta ler o cabeçalho
        //                bool hasHeader = csv.Read();
        //                if (hasHeader)
        //                {
        //                    csv.ReadHeader();
        //                    var header = csv.HeaderRecord;

        //                    // Verifica se o cabeçalho contém as colunas válidas
        //                    if (header == null || !validColumns.All(validColumn => header.Contains(validColumn)))
        //                    {
        //                        return ("Cabeçalho inválido no arquivo CSV. As colunas esperadas são: Installation, Batch, Operator, Manufacturer, Model, Version.", null);
        //                    }
        //                }
        //                else
        //                {
        //                    // Se não houver cabeçalho, deve seguir a ordem das colunas no modelo
        //                    // Não é possível garantir a ordem das colunas sem um cabeçalho
        //                    return ("O arquivo CSV não contém cabeçalho e não pode ser processado sem um cabeçalho.", null);
        //                }

        //                // Lê os registros do CSV e mapeia para o objeto Equipment
        //                records = csv.GetRecords<Equipment>().ToList();

        //                // Verifica se algum campo obrigatório está vazio
        //                foreach (var record in records)
        //                {
        //                    if (string.IsNullOrEmpty(record.Installation) ||
        //                        string.IsNullOrEmpty(record.Batch) ||
        //                        string.IsNullOrEmpty(record.Operator) ||
        //                        string.IsNullOrEmpty(record.Manufacturer) ||
        //                        string.IsNullOrEmpty(record.Model) ||
        //                        string.IsNullOrEmpty(record.Version))
        //                    {
        //                        return ("Alguns campos obrigatórios estão vazios. Verifique os dados no arquivo CSV.", null);
        //                    }
        //                }
        //            }
        //        }

        //        return ("Arquivo CSV importado com sucesso!", records);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ($"Erro ao processar o arquivo: {ex.Message}", null);
        //    }
        //}

        //    public async Task<(string, List<Equipment>)> ImportCSVAsync(IFormFile file)
        //    {
        //        if (file == null || file.Length == 0)
        //        {
        //            return ("Por favor, selecione um arquivo CSV.", null);
        //        }

        //        if (!file.FileName.EndsWith(".csv"))
        //        {
        //            return ("Por favor, selecione um arquivo CSV válido.", null);
        //        }

        //        List<Equipment> records = new List<Equipment>();

        //        try
        //        {
        //            using (var reader = new StreamReader(file.OpenReadStream()))
        //            {
        //                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        //                {
        //                    Delimiter = ";", 
        //                    HasHeaderRecord = true,
        //                    HeaderValidated = null, 
        //                    MissingFieldFound = null
        //                };

        //                using (var csv = new CsvReader(reader, config))
        //                {
        //                    // Verifica o cabeçalho
        //                    var header = csv.Context.HeaderRecord;
        //                    var requiredColumns = new[] { "Instalação", "Lote", "Operador", "Fabricante", "Modelo", "Versão" };

        //                    if (header == null || !requiredColumns.All(col => header.Contains(col)))
        //                    {
        //                        return ("Erro: O arquivo CSV não contém as colunas necessárias ou tem colunas faltando.", null);
        //                    }

        //                    // Ignora colunas extras
        //                    var validColumns = header.Where(col => requiredColumns.Contains(col)).ToArray();
        //                    csv.Context.HeaderRecord = validColumns;

        //                    // Realiza o mapeamento das colunas
        //                    csv.Context.RegisterClassMap<EquipmentMap>();
        //                    records = csv.GetRecords<Equipment>().ToList();
        //                    //records = csv.GetRecords<Equipment>().ToList();
        //                }
        //            }

        //            return ("Arquivo CSV importado com sucesso!", records);
        //        }
        //        catch (Exception ex)
        //        {
        //            return ($"Erro ao processar o arquivo: {ex.Message}", null);
        //        }
        //    }
    }
}
