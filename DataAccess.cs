using System;
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

        /// <summary>
        /// Please do reader.Close(); after reading the data
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string commandText, CommandType commandType, SqlParameter[] sqlParams)
        {
            SqlConnection sqlConn;

            sqlConn = new SqlConnection(connString);
            sqlConn.Open();

            SqlDataReader reader;
            SqlCommand command;

            command = new SqlCommand(commandText, sqlConn);
            SetCommandType(commandType, command);
            reader = command.ExecuteReader();

            return reader;
        }

        public void ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] sqlParams)
        {
            SqlConnection sqlConn;

            sqlConn = new SqlConnection(connString);
            sqlConn.Open();

            SqlCommand command;

            command = new SqlCommand(commandText, sqlConn);
            SetCommandType(commandType, command);
            command.Parameters.AddRange(sqlParams);

            command.ExecuteNonQuery();

            sqlConn.Close();
        }

        public object ExecuteScalar(string commandText, CommandType commandType, SqlParameter[] sqlParams)
        {
            SqlConnection sqlConn;

            sqlConn = new SqlConnection(connString);
            sqlConn.Open();

            SqlCommand command;

            command = new SqlCommand(commandText, sqlConn);
            SetCommandType(commandType, command);
            command.Parameters.AddRange(sqlParams);

            var returnValue = command.ExecuteScalar();

            sqlConn.Close();

            return returnValue;
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
}
