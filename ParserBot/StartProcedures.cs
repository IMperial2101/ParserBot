using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.BC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseBotSolution
{
    internal class StartProcedures
    {
        public static Dictionary<long, string>  GetLinksFromDBtoParse(Dictionary<long,string> links, DataBase dataBase)
        {
            links.Clear();
            string query = "SELECT * FROM Active_search";
            MySqlCommand command = new MySqlCommand(query, dataBase.GetConnection());

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    links.Add(reader.GetInt32(1), reader.GetString(2));
                }
            }
            return links;
        }
    }
}
