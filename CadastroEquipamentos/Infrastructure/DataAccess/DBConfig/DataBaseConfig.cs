using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;


namespace EquipmentManagement.Infrastructure.DataAccess.DBConfig
{
    public class DataBaseConfig
    {
        public static class DatabaseConfig
        {
            private const string ConnectionString = "Server=localhost;Database=EquipmentsDB;User Id=root;Password=Oei##$1122;";

            public static async Task<MySqlConnection> GetConnectionAsync()
            {
                return await Task.FromResult(new MySqlConnection(ConnectionString));
            }
        }
    }
}
