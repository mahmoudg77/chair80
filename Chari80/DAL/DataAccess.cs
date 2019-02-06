using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Chari80.DAL
{
    public class DataAccess
    {
        public static string SQLToCSV(string sqlview)
        {

            SqlConnection conn = new SqlConnection(Properties.Settings.Default.conString.ToString());
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from "+ sqlview, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            string csvContent = "";
            using (System.IO.StringWriter sb = new System.IO.StringWriter())
            {
                // Loop through the fields and add headers
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i);
                    if (name.Contains(","))
                        name = "\"" + name + "\"";

                    sb.Write(name + ",");
                }
                sb.WriteLine();

                // Loop through the rows and output the data
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(","))
                            value = "\"" + value + "\"";

                        sb.Write(value + ",");
                    }
                    sb.WriteLine();
                }
                csvContent = sb.ToString();
                sb.Close();
            }

            return csvContent;
        }

        public static DataTable getData(string sql)
        {
            SqlConnection conn = new SqlConnection(Properties.Settings.Default.conString.ToString());
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(sql,conn);
            DataTable dt = new DataTable();

            da.Fill(dt);
            conn.Close();
            return dt;
        }
    }

  


}