using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using EquipmentCommon.CommonEntities;
using EquipmentCommon.CommonInterfaces.Repositories;
//using MySqlConnector;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly string _connectionString;

    public EquipmentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Connection");
    }

    public Task<IEnumerable<Equipment>> GetAllAsync()
    {
        var equipment = new List<Equipment>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Id, Installation, Batch, Operator, Manufacturer, Model, Version FROM Equipment";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    equipment.Add(new Equipment
                    {
                        Id = reader.GetInt32("Id"),
                        Installation = reader.GetString("Installation"),
                        Batch = reader.GetInt32("Batch"),
                        Operator = reader.GetString("Operator"),
                        Manufacturer = reader.GetString("Manufacturer"),
                        Model = reader.GetInt32("Model"),
                        Version = reader.GetInt32("Version"),

                    });
                }
            }
        }

        return Task.FromResult<IEnumerable<Equipment>>(equipment);
    }

    public Task<Equipment?> GetByIdAsync(int id)
    {
        Equipment? equipment = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT Id, Installation, Batch, Operator, Manufacturer, Model, Version FROM Equipment WHERE Id = @Id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        equipment = new Equipment
                        {
                            Id = reader.GetInt32("Id"),
                            Installation = reader.GetString("Installation"),
                            Batch = reader.GetInt32("Batch"),
                            Operator = reader.GetString("Operator"),
                            Manufacturer = reader.GetString("Manufacturer"),
                            Model = reader.GetInt32("Model"),
                            Version = reader.GetInt32("Version"),
                        };
                    }
                }
            }
        }

        return Task.FromResult(equipment);
    }

    public Task<Equipment> AddAsync(Equipment equipment)
    {
        try
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Equipment (Installation, Batch, Operator, Manufacturer, Model, Version) VALUES (@Installation, @Batch, @Operator, @Manufacturer, @Model, @Version)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Installation", equipment.Installation);
                    command.Parameters.AddWithValue("@Batch", equipment.Batch);
                    command.Parameters.AddWithValue("@Operator", equipment.Operator);
                    command.Parameters.AddWithValue("@Manufacturer", equipment.Manufacturer);
                    command.Parameters.AddWithValue("@Model", equipment.Model);
                    command.Parameters.AddWithValue("@Version", equipment.Version);
                    command.ExecuteNonQuery();
                }
            }
            return Task.FromResult(equipment);
        }
        catch (Exception ex)
        {

            throw;
        }
       
    }

    public Task<bool> UpdateAsync(Equipment entity)
    {
        using (var conn = new MySqlConnection(_connectionString))
        {
            conn.Open();
            string query = "UPDATE Equipment SET Installation = @Installation, Batch = @Batch, Operator = @Operator, Manufacturer = @Manufacturer, Model = @Model, Version = @Version WHERE Id = @Id";
            using (var command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@Installation", entity.Installation);
                command.Parameters.AddWithValue("@Batch", entity.Batch);
                command.Parameters.AddWithValue("@Operator", entity.Operator);
                command.Parameters.AddWithValue("@Manufacturer", entity.Manufacturer);
                command.Parameters.AddWithValue("@Model", entity.Model);
                command.Parameters.AddWithValue("@Version", entity.Version);
                command.Parameters.AddWithValue("@Id", entity.Id);
                int rowsAffected = command.ExecuteNonQuery();

                return Task.FromResult(rowsAffected > 0);
            }
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "DELETE FROM Equipment WHERE Id = @Id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }
    }

    public void Dispose()
    {

    }

    public Task<IEnumerable<Equipment>> GetPartialAsync()
    {
        throw new NotImplementedException();
    }
}