using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AllAboutDough
{
    class DAL
    {
        private string connstring;

        public string getconnstring()
        {
            this.connstring = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = AllAboutDough; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
            return this.connstring;
        }

        public SqlConnection getconnection()
        {
            this.connstring = getconnstring();
            SqlConnection conn = new SqlConnection(connstring);
            conn.Open();
            return conn;
        }

        public void InsertOrders(Orders item)
        {
            
            SqlConnection conn = getconnection();
            string query = "INSERT INTO [dbo].[Orders] VALUES( '" + item.OrderId + "', '" + item.OrderDate + "', '" + item.Toppings + "', '" + item.IsToppingVegetarian + "');";
            
            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = conn;

            SqlDataReader reader = cmd.ExecuteReader();
            
            conn.Close();
            conn.Dispose();

        }

        public int GetVeganOrders(DateTime date)
        {
            SqlConnection conn = getconnection();
            string query = "select count(*) as Vegan from orders where istoppingvegetarian = 1 and(@date is null or  datediff(day, orderdate, @date) = 0);";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@date", date.Date.ToString());

            var Vegan = (Int32)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();

            return Vegan;
        }

        public int GetNonVeganOrders(DateTime date)
        {
            SqlConnection conn = getconnection();
            string query = "select count(*) as nonVegan from orders where istoppingvegetarian = 0 and(@date is null or  datediff(day, orderdate, @date) = 0);";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@date", date.Date.ToString());

            var nonVegan = (Int32)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();

            return nonVegan;
        }
    }

}