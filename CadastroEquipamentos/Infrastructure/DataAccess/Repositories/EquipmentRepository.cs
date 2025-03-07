using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using EquipmentManagement.Application.Services;
using EquipmentManagement.Infrastructure.DataAccess.DBConfig;
using EquipmentCommon.CommonEntities;
using static EquipmentManagement.Infrastructure.DataAccess.DBConfig.DataBaseConfig;
using System.Data.Common;
using EquipmentCommon.CommonInterfaces.Repositories;

namespace EquipmentManagement.Infrastructure.DataAccess.Repositories
{
    public class EquipmentRepository : DBContext, IEquipmentRepository
    {
        private readonly LoggerService _logger;

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            const string query = "SELECT Id, Installation, Batch, Operator, Manufacturer, Model, Version FROM Equipment";
            using var conn = await GetOpenConnectionAsync();
            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            return await ReadEquipmentsAsync(reader);
        }

        public async Task<IEnumerable<Equipment>> GetPartialAsync()
        {
            const string query = "SELECT Id, Installation, Batch, Operator, Manufacturer, Model, Version FROM Equipment";
            using var conn = await GetOpenConnectionAsync();
            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            return await ReadEquipmentsAsync(reader);
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            const string query = "SELECT Id, Installation, Batch, Operator, Manufacturer, Model, Version FROM Equipment WHERE Id = @Id";
            using var conn = await GetOpenConnectionAsync();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            return await reader.ReadAsync().ConfigureAwait(false) ? MapEquipment(reader) : null;
        }

        public async Task<Equipment> AddAsync(Equipment equipment)
        {
            const string query = "INSERT INTO Equipment (Installation, Batch, Operator, Manufacturer, Model, Version) " +
                                 "VALUES (@Installation, @Batch, @Operator, @Manufacturer, @Model, @Version)";
            using var conn = await GetOpenConnectionAsync();
            using var cmd = CreateCommand(conn, query, equipment);
            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            return equipment;
        }

        public async Task<bool> UpdateAsync(Equipment equipment)
        {
            const string query = "UPDATE Equipment SET Installation = @Installation, Batch = @Batch, Operator = @Operator, " +
                                 "Manufacturer = @Manufacturer, Model = @Model, Version = @Version WHERE Id = @Id";
            using var conn = await GetOpenConnectionAsync();
            using var cmd = CreateCommand(conn, query, equipment);
            cmd.Parameters.AddWithValue("@Id", equipment.Id);
            return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Equipment WHERE Id = @Id";
            using var conn = await GetOpenConnectionAsync();
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
        }

        private async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            var conn = await DatabaseConfig.GetConnectionAsync();
            await conn.OpenAsync().ConfigureAwait(false);
            return conn;
        }

        private async Task<List<Equipment>> ReadEquipmentsAsync(DbDataReader reader)
        {
            var equipments = new List<Equipment>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                equipments.Add(MapEquipment(reader));
            }
            return equipments;
        }

        private Equipment MapEquipment(DbDataReader reader)
        {
            return new Equipment
            {
                Id = reader.GetInt32("Id"),
                Installation = reader.GetString("Installation"),
                Batch = reader.GetInt32("Batch"),
                Operator = reader.GetString("Operator"),
                Manufacturer = reader.GetString("Manufacturer"),
                Model = reader.GetInt32("Model"),
                Version = reader.GetInt32("Version")
            };
        }

        private MySqlCommand CreateCommand(MySqlConnection conn, string query, Equipment equipment)
        {
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Installation", equipment.Installation);
            cmd.Parameters.AddWithValue("@Batch", equipment.Batch);
            cmd.Parameters.AddWithValue("@Operator", equipment.Operator);
            cmd.Parameters.AddWithValue("@Manufacturer", equipment.Manufacturer);
            cmd.Parameters.AddWithValue("@Model", equipment.Model);
            cmd.Parameters.AddWithValue("@Version", equipment.Version);
            return cmd;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    //public class EquipmentRepository : DBContext, IEquipmentRepository
    //{
    //    private readonly LoggerService _logger;

    //    public async Task<IEnumerable<Equipment>> GetAllAsync()
    //    {
    //        var equipments = new List<Equipment>();
    //        using (var conn = await DatabaseConfig.GetConnectionAsync())
    //        {
    //            await conn.OpenAsync().ConfigureAwait(false);
    //            var query = "SELECT Id, Installation, Batch, Operator, Manufacturer, Model, Version FROM Equipment";
    //            using (var cmd = new MySqlCommand(query, conn))
    //            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
    //            {
    //                while (await reader.ReadAsync().ConfigureAwait(false))
    //                {
    //                    equipments.Add(new Equipment
    //                    {
    //                        Id = reader.GetInt32("Id"),
    //                        Installation = reader.GetString("Installation"),
    //                        Batch = reader.GetInt32("Batch"),
    //                        Operator = reader.GetString("Operator"),
    //                        Manufacturer = reader.GetString("Manufacturer"),
    //                        Model = reader.GetInt32("Model"),
    //                        Version = reader.GetInt32("Version"),
    //                    });
    //                }
    //            }
    //        }
    //        return equipments;
    //    }

    //    public async Task<Equipment?> GetByIdAsync(int id)
    //    {
    //        using (var conn = await DatabaseConfig.GetConnectionAsync())
    //        {
    //            await conn.OpenAsync().ConfigureAwait(false);
    //            var query = "SELECT Id, Installation, Batch, Operator, Manufacturer, Model, Version FROM Equipment WHERE Id = @Id";
    //            using (var cmd = new MySqlCommand(query, conn))
    //            {
    //                cmd.Parameters.AddWithValue("@Id", id);
    //                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
    //                {
    //                    if (await reader.ReadAsync().ConfigureAwait(false))
    //                    {
    //                        return new Equipment
    //                        {
    //                            Id = reader.GetInt32("Id"),
    //                            Installation = reader.GetString("Installation"),
    //                            Batch = reader.GetInt32("Batch"),
    //                            Operator = reader.GetString("Operator"),
    //                            Manufacturer = reader.GetString("Manufacturer"),
    //                            Model = reader.GetInt32("Model"),
    //                            Version = reader.GetInt32("Version"),
    //                        };
    //                    }
    //                }
    //            }
    //        }
    //        return null!;
    //    }

    //    public async Task<Equipment> AddAsync(Equipment equipment)
    //    {
    //        using (var conn = await DatabaseConfig.GetConnectionAsync())
    //        {
    //            await conn.OpenAsync().ConfigureAwait(false);
    //            var query = "INSERT INTO Equipment (Installation, Batch, Operator, Manufacturer, Model, Version) " +
    //                        "VALUES (@Installation, @Batch, @Operator, @Manufacturer, @Model, @Version)";
    //            using (var cmd = new MySqlCommand(query, conn))
    //            {
    //                cmd.Parameters.AddWithValue("@Installation", equipment.Installation);
    //                cmd.Parameters.AddWithValue("@Batch", equipment.Batch);
    //                cmd.Parameters.AddWithValue("@Operator", equipment.Operator);
    //                cmd.Parameters.AddWithValue("@Manufacturer", equipment.Manufacturer);
    //                cmd.Parameters.AddWithValue("@Model", equipment.Model);
    //                cmd.Parameters.AddWithValue("@Version", equipment.Version);
    //                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    //                return equipment;
    //            }
    //        }
    //    }

    //    public async Task<bool> UpdateAsync(Equipment equipment)
    //    {
    //        using (var conn = await DatabaseConfig.GetConnectionAsync())
    //        {
    //            await conn.OpenAsync().ConfigureAwait(false);
    //            var query = "UPDATE Equipment SET Installation = @Installation, Batch = @Batch, Operator = @Operator, " +
    //                        "Manufacturer = @Manufacturer, Model = @Model, Version = @Version WHERE Id = @Id";
    //            using (var cmd = new MySqlCommand(query, conn))
    //            {
    //                cmd.Parameters.AddWithValue("@Installation", equipment.Installation);
    //                cmd.Parameters.AddWithValue("@Batch", equipment.Batch);
    //                cmd.Parameters.AddWithValue("@Operator", equipment.Operator);
    //                cmd.Parameters.AddWithValue("@Manufacturer", equipment.Manufacturer);
    //                cmd.Parameters.AddWithValue("@Model", equipment.Model);
    //                cmd.Parameters.AddWithValue("@Version", equipment.Version);
    //                cmd.Parameters.AddWithValue("@Id", equipment.Id);
    //                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
    //            }
    //        }
    //    }

    //    public async Task<bool> DeleteAsync(int id)
    //    {
    //        using (var conn = await DatabaseConfig.GetConnectionAsync())
    //        {
    //            await conn.OpenAsync().ConfigureAwait(false);
    //            var query = "DELETE FROM Equipment WHERE Id = @Id";
    //            using (var cmd = new MySqlCommand(query, conn))
    //            {
    //                cmd.Parameters.AddWithValue("@Id", id);
    //                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
    //            }
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
