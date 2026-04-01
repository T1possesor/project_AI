using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace project_AI
{
    internal class DatabaseHelper
    {
        static string connectionString =
            ConfigurationManager.ConnectionStrings["BookDB"].ConnectionString;

        public static DataTable ExecuteTable(string sql, params SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static int CountRows(string sql, params SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand($"SELECT COUNT(*) FROM ({sql}) AS T", conn);
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return (int)cmd.ExecuteScalar();
        }

        public static List<string> GetAllAuthors()
        {
            var list = new List<string>();
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT DISTINCT Author FROM Books ORDER BY Author", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(r.GetString(0));

            return list;
        }

        public static List<string> GetAllCategories()
        {
            var list = new List<string>();
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT DISTINCT Category FROM Books ORDER BY Category", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(r.GetString(0));

            return list;
        }

        public static (int minYear, int maxYear) GetYearRange()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT MIN(PublishYear), MAX(PublishYear) FROM Books", conn);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                int minY = r.IsDBNull(0) ? 1900 : r.GetInt32(0);
                int maxY = r.IsDBNull(1) ? 2100 : r.GetInt32(1);
                return (minY, maxY);
            }
            return (1900, 2100);
        }
    }
}
