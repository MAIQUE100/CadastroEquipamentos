using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentManagement.Infrastructure.DataAccess.DBConfig
{
    public class DBContext
    {
        private MySqlConnection mySqlConnection = new MySqlConnection();

        public MySqlConnection GetConnection()
        {
            return mySqlConnection;
        }

        public void OpenConnection()
        {
            if (mySqlConnection.State != ConnectionState.Open)
            {
                mySqlConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (mySqlConnection.State == ConnectionState.Open)
            {
                mySqlConnection.Close();
            }
        }

        public MySqlDataReader ExecuteReader(string sql)
        {
            OpenConnection();
            using (MySqlCommand cmd = GetConnection().CreateCommand())
            {
                cmd.CommandText = sql;
                MySqlDataReader rdr = cmd.ExecuteReader();
                CloseConnection();
                return rdr;
            }
        }

        public async Task<DataSet> ExecuteQueryAsync(string sql)
        {
            DataSet dataSet = new DataSet();

            // Abrir a conexão de forma síncrona (se não houver método assíncrono para isso)
            OpenConnection();

            using (MySqlCommand cmd = GetConnection().CreateCommand())
            {
                cmd.CommandText = sql;
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                // Usando Task.Run para fazer o Fill de forma assíncrona
                await Task.Run(() => adapter.Fill(dataSet));
            }

            // Fechar a conexão de forma síncrona (novamente, se não houver método assíncrono)
            CloseConnection();

            return dataSet;
        }

        //public async DataSet ExecuteQuery(string sql)
        //{
        //    DataSet dataSet = new DataSet();

        //    OpenConnection();

        //    using (MySqlCommand cmd = GetConnection().CreateCommand())
        //    {
        //        cmd.CommandText = sql;
        //        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        //        adapter.Fill(dataSet);
        //        CloseConnection();
        //    }

        //    return dataSet;
        //}

        public List<T> GetAll<T>(int? maxRows = null)
        {
            DataSet dataSet = new DataSet();
            List<T> entities = new List<T>();

            string tableName = GetTableName<T>();

            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo primaryKey = GetPrimaryKeyProperty<T>();

            string selectQuery = maxRows != null && primaryKey != null
                ? $"SELECT TOP {maxRows.Value} * FROM `{tableName}` ORDER BY {GetColumnName(primaryKey)} DESC"
                : $"SELECT * FROM `{tableName}`";

            OpenConnection();

            using (MySqlCommand cmd = GetConnection().CreateCommand())
            {
                cmd.CommandText = selectQuery;
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dataSet);

                if (dataSet.Tables.Count > 0)
                {
                    DataTable dataTable = dataSet.Tables[0];

                    foreach (DataRow row in dataTable.Rows)
                    {
                        T entity = Activator.CreateInstance<T>();

                        foreach (PropertyInfo property in properties)
                        {
                            string columnName = GetColumnName(property);

                            if (dataTable.Columns.Contains(columnName) && !row.IsNull(columnName))
                            {
                                object value = row[columnName];

                                try
                                {
                                    property.SetValue(entity, Convert.ChangeType(value, property.PropertyType));
                                }
                                catch (InvalidCastException)
                                {
                                    //Logger.LogError(this, "Erro ao converter a variável '{0}'", columnName);
                                }
                            }
                        }

                        entities.Add(entity);
                    }
                }
            }
            CloseConnection();

            return entities;
        }

        public T GetById<T>(object id)
        {
            DataSet dataSet = new DataSet();
            string tableName = GetTableName<T>();

            PropertyInfo[] properties = typeof(T).GetProperties();

            PropertyInfo primaryKeyProperty = null;
            string primaryKeyColumnName = null;

            foreach (PropertyInfo property in properties)
            {
                if (Attribute.IsDefined(property, typeof(KeyAttribute)))
                {
                    primaryKeyProperty = property;
                    primaryKeyColumnName = GetColumnName(property);
                    break;
                }
            }

            if (primaryKeyProperty == null)
            {
                throw new Exception("Primary key not found for the entity.");
            }

            string selectQuery = $"SELECT * FROM `{tableName}` WHERE {primaryKeyColumnName} = @Id";

            OpenConnection();
            using (MySqlCommand cmd = GetConnection().CreateCommand())
            {
                cmd.CommandText = selectQuery;
                cmd.Parameters.AddWithValue("@Id", id);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dataSet);

                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    T entity = Activator.CreateInstance<T>();

                    foreach (PropertyInfo property in properties)
                    {
                        string columnName = GetColumnName(property);

                        if (dataSet.Tables[0].Columns.Contains(columnName) && !row.IsNull(columnName))
                        {
                            try
                            {
                                object value = row[columnName];
                                property.SetValue(entity, value);
                            }
                            catch (Exception ex)
                            {
                                //Logger.LogError(this, ex.Message);
                            }
                        }
                    }
                    CloseConnection();
                    return entity;
                }
            }
            return default;
        }

        public string Insert<T>(T entity)
        {
            string tableName = GetTableName<T>();

            PropertyInfo[] properties = typeof(T).GetProperties();

            List<string> columnNames = new List<string>();
            List<string> values = new List<string>();

            PropertyInfo primaryKeyProperty = null;
            string primaryKeyColumnName = null;

            foreach (PropertyInfo property in properties)
            {
                if (Attribute.IsDefined(property, typeof(KeyAttribute)))
                {
                    primaryKeyProperty = property;
                    primaryKeyColumnName = GetColumnName(property);
                    continue;
                }

                if (Attribute.IsDefined(property, typeof(NotMappedAttribute)))
                {
                    continue;
                }

                string columnName = string.Format("`{0}`", GetColumnName(property));
                object value = property.GetValue(entity);

                columnNames.Add(columnName);

                if (property.PropertyType == typeof(DateTime))
                {
                    values.Add(Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(double))
                {
                    values.Add(value.ToString().Replace(',', '.'));
                }
                else
                {
                    values.Add(value != null ? value.ToString() : "NULL");
                }
            }

            string columns = string.Join(", ", columnNames);
            string valueList = string.Join(", ", values.Select(v => $"'{v}'")).Replace("'NULL'", "NULL");

            string outputClause = string.Format("OUTPUT INSERTED.{0}", primaryKeyColumnName);

            string insert = string.Format("INSERT INTO `{0}` ({1}) {2} VALUES ({3})", tableName, columns, outputClause, valueList);

            return insert;
        }

        public string Update<T>(T entity)
        {
            string tableName = GetTableName<T>();
            string update = $"UPDATE `{tableName}` SET ";

            string where = string.Empty;
            string primaryKeyPropertyName = null;
            object primaryKeyValue = null;

            PropertyInfo[] properties = typeof(T).GetProperties();

            PropertyInfo[] filteredProperties = properties.Where(property =>
            !Attribute.IsDefined(property, typeof(NotMappedAttribute))
            ).ToArray();

            int propertyCount = filteredProperties.Length;

            for (int i = 0; i < propertyCount; i++)
            {
                PropertyInfo property = filteredProperties[i];
                object value = property.GetValue(entity);

                if (property.PropertyType == typeof(DateTime) && value.Equals(new DateTime()))
                {
                    continue;
                }

                if (Attribute.IsDefined(property, typeof(KeyAttribute)))
                {
                    primaryKeyPropertyName = GetColumnName(property);
                    primaryKeyValue = value;
                }
                else
                {
                    string propertyName = GetColumnName(property);
                    string formattedValue = string.Empty;

                    if (property.PropertyType == typeof(DateTime))
                    {
                        formattedValue = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                        formattedValue = $"'{formattedValue}'";
                    }
                    else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(double))
                    {
                        formattedValue = value != null ? FormatPropertyValue(value) : "NULL";
                        formattedValue = formattedValue.ToString().Replace(',', '.');
                    }
                    else if (property.PropertyType == typeof(int) && (int)value == int.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        formattedValue = value != null ? FormatPropertyValue(value) : "NULL";
                    }

                    update += $"`{propertyName}` = {formattedValue}";

                    if (i < propertyCount - 1)
                    {
                        update += ", ";
                    }
                }
            }

            if (string.IsNullOrEmpty(primaryKeyPropertyName) || primaryKeyValue == null)
            {
                throw new ArgumentException("Entity does not have a KeyAttribute property with a valid value.");
            }

            where = $" WHERE `{primaryKeyPropertyName}` = {FormatPropertyValue(primaryKeyValue)}";
            return update + where;
        }

        public string Delete<T>(T entity)
        {
            string tableName = GetTableName<T>();

            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo primaryKeyProperty = null;
            object primaryKeyValue = null;

            foreach (PropertyInfo property in properties)
            {
                if (Attribute.IsDefined(property, typeof(KeyAttribute)))
                {
                    primaryKeyProperty = property;
                    primaryKeyValue = property.GetValue(entity);
                    break;
                }
            }

            if (primaryKeyProperty == null)
            {
                throw new Exception("Primary key not found for the entity.");
            }

            string delete = string.Format("DELETE FROM `{0}` WHERE `{1}` = '{2}'", tableName, primaryKeyProperty.Name, primaryKeyValue);

            return delete;
        }

        private string FormatPropertyValue(object value)
        {
            if (value is string || value is DateTime || value is bool)
            {
                return $"'{value}'";
            }
            return value.ToString();
        }

        private string GetColumnName(PropertyInfo property)
        {
            var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));

            if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
            {
                return $"{columnAttribute.Name}";
            }

            return $"{property.Name}";
        }

        private string GetTableName<T>()
        {
            var attribute = typeof(T).GetCustomAttribute<TableAttribute>();
            return attribute?.Name ?? typeof(T).Name;
        }

        private PropertyInfo GetPrimaryKeyProperty<T>()
        {
            return typeof(T).GetProperties()
                            .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));
        }
    }
}
