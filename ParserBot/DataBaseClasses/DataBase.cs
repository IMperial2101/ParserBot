using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace ParseBotSolution
{
    partial class DataBase
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();

        MySqlConnection connection = new MySqlConnection(@"Server = localhost;port=3306;Initial Catalog=login;User id=root;password='';database = avitoparserdatabase");
        public DataBase()
        {
            Open();
            Chek();
        }

        public void Chek()
        {
            if (connection.State != ConnectionState.Open)
            {
                Console.WriteLine("Ошибка подключения!");
            }
            else
                Console.WriteLine("Подключение установлено!");
        }
        
        public void Close()
        {
            connection.Close();
        }
        public void Open()
        {
            connection.Open();
        }
        public MySqlConnection GetConnection()
        {
            return connection;
        }

    }
}

