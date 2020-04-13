using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{

    public class DataAccess 
    {
        string connString = "";
        public DataAccess(string connectionString)
        {
            connString = connectionString;
        }

        public  List<T> Query<T>(string commandText, CommandType commandType, SqlParameter[] sqlParams)
        {
            var results = new List<T>();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {
                sqlConn.Open();

                using (SqlCommand command = new SqlCommand(commandText, sqlConn))
                {
                    SetCommandType(commandType, command);
                    if (sqlParams != null)
                        command.Parameters.AddRange(sqlParams);
                    results = ReflectionPopulator<T>.CreateList(command.ExecuteReader());
                }
            }
            return results;
        }

        /// <summary>
        /// Please close datareader and SQlConnection after reading the data
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        //public IDataReader ExecuteReader(string commandText, CommandType commandType, SqlParameter[] sqlParams)
        //{
        //    SqlConnection sqlConn = null;
        //    SqlDataReader reader = null;

        //    try
        //    {
        //        sqlConn = new SqlConnection(connString);
        //        {
        //            sqlConn.Open();

        //            SqlCommand command = new SqlCommand(commandText, sqlConn);
        //            {
        //                SetCommandType(commandType, command);
        //                reader = command.ExecuteReader();
        //                return reader;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        if (reader != null) 
        //            reader.Close();
        //        if (sqlConn != null)
        //            sqlConn.Close();
        //        throw;
        //    }


        //}

        public void ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] sqlParams)
        {
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {
                sqlConn.Open();

                using (SqlCommand command = new SqlCommand(commandText, sqlConn))
                {
                    SetCommandType(commandType, command);
                    command.Parameters.AddRange(sqlParams);

                    command.ExecuteNonQuery();
                }
                sqlConn.Close();
            }
        }

        public object ExecuteScalar(string commandText, CommandType commandType, SqlParameter[] sqlParams)
        {
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {
                sqlConn.Open();

                using (SqlCommand command = new SqlCommand(commandText, sqlConn))
                {
                    SetCommandType(commandType, command);
                    command.Parameters.AddRange(sqlParams);

                    var returnValue = command.ExecuteScalar();

                    sqlConn.Close();
                    return returnValue;
                }
            }
        }

        private static void SetCommandType(CommandType commandType, SqlCommand command)
        {
            if (commandType == CommandType.Text)
            {
                command.CommandType = CommandType.Text;
            }
            else
            {
                command.CommandType = CommandType.StoredProcedure;
            }
        }
    }

    public class ReflectionPopulator<T>
    {
        public static List<T> CreateList(SqlDataReader reader)
        {
            var results = new List<T>();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                    }
                }
                results.Add(item);
            }
            return results;
        }
    }
}
