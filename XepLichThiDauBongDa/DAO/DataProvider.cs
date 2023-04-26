using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace XepLichThiDauBongDa.DAO
{
    public class DataProvider
    {
        private string connectionStr = "Data Source=.\\SQLEXPRESS;Initial Catalog=QLGD;Integrated Security=True";
        private static DataProvider instance;

        public static DataProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataProvider();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        private DataProvider() { }

        public DataTable ExecuteQuery(string query, object[] param = null)
        {
            DataTable data = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                if (param != null)
                {
                    string[] listParam = query.Split(' ');
                    int i = 0;
                    foreach (string item in listParam)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, param[i++]);
                        }
                    }
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                adapter.Fill(data);

                connection.Close();
            }

            return data;
        }

        public int ExecuteNonQuery(string query, object[] param = null)
        {
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                if (param != null)
                {
                    string[] listParam = query.Split(' ');
                    int i = 0;
                    foreach (string item in listParam)
                    {
                        if (item.Contains('@')) 
                        {  
                            command.Parameters.AddWithValue(item, param[i++]);
                        }
                    }
                }

                rowAffected = command.ExecuteNonQuery();

                connection.Close();
            }

            return rowAffected;
        }

        public object ExecuteScalar(string query, object[] param = null)
        {
            object scalar = 0;

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                if (param != null)
                {
                    string[] listParam = query.Split(' ');
                    int i = 0;
                    foreach (string item in listParam)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, param[i++]);
                        }
                    }
                }

                scalar = command.ExecuteScalar();

                connection.Close();
            }

            return scalar;
        }
    }
}
