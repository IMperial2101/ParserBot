using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseBotSolution
{
    partial class DataBase
    {
        public void SqlCommand(string sqlCommand)
        {
            MySqlCommand command = new MySqlCommand(sqlCommand, GetConnection());
            adapter.SelectCommand = command;
            command.ExecuteNonQuery();
        }
        public void SqlCommandShowInfo(string sqlCommand)
        {
            DataTable table = new DataTable();

            MySqlCommand command = new MySqlCommand(sqlCommand, GetConnection());
            adapter.SelectCommand = command;

            adapter.Fill(table);

            if (table.Rows.Count > 0)
                Console.WriteLine(table.Rows.Count);

            DataRow[] currentRows = table.Select(
    null, null, DataViewRowState.CurrentRows);

            if (currentRows.Length < 1)
                Console.WriteLine("No Current Rows Found");
            else
            {
                foreach (DataColumn column in table.Columns)
                    Console.Write("\t{0}", column.ColumnName);

                Console.WriteLine("\tRowState");

                foreach (DataRow row in currentRows)
                {
                    foreach (DataColumn column in table.Columns)
                        Console.Write("\t{0}", row[column]);

                    Console.WriteLine("\t" + row.RowState);
                }
            }

        }
        public List<string> SqlCommandGetOneColumn(string sqlCommand)
        {
            List<string> info = new List<string>();

            DataTable table = MakeSqlTable(sqlCommand);

            if (table.Rows.Count < 1)
                Console.WriteLine("No Current rows find");
            else if (table.Columns.Count > 1)
                Console.WriteLine("More than 1 column selected");
            else
            {
                foreach (DataRow row in table.Rows)
                    info.Add(row[0].ToString());
            }
            return info;
        }

        private DataTable MakeSqlTable(string sqlCommand)
        {
            DataTable table = new DataTable();

            MySqlCommand command = new MySqlCommand(sqlCommand, GetConnection());
            adapter.SelectCommand = command;

            adapter.Fill(table);
            return table;
        }
    }
}
