// DB.cs
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace WFproject
{
    class DB
    {
        private MySqlConnection connection;

        public DB()
        {
            connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=richbalanceusers");
        }

        // Действия с БД
        public void openConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void closeConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }
    }
}
