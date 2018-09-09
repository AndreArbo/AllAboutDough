using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AllAboutDough
{
    class Program
    {
        public static List<string> animalTopping = new List<string>();
        
        static void Main(string[] args)
        {
            animalTopping.Add("ham");
            animalTopping.Add("salami");
            animalTopping.Add("pepperoni");
            animalTopping.Add("anchovies");

            DAL dal = new DAL();

            Console.WriteLine("Welcome!");

            var result = loadJSONfile();

            writetoCSV(result);

            ReadfromCSV(dal);
            Console.WriteLine("Insert date:");
            var date = Console.ReadLine();

            DateTime datetime = new DateTime();

            while (!DateTime.TryParse(date, out datetime))
            {
                Console.WriteLine("Error: Invalid Date. Try again.");
                date = Console.ReadLine();

            }
            int vegan = dal.GetVeganOrders(datetime);

            int nonvegan = dal.GetNonVeganOrders(datetime);

            table_Result(date, vegan, nonvegan);


        }

        private static rootobject loadJSONfile()
        {

            using (System.IO.StreamReader r = new System.IO.StreamReader("orders.json"))
            {
                string json = r.ReadToEnd();
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" };
                rootobject result = Newtonsoft.Json.JsonConvert.DeserializeObject<rootobject>(json, dateTimeConverter);

                Console.WriteLine("Read Json File");
                return result;

            }

        }

        #region CSV File
        private static void writetoCSV(rootobject orders)
        {
            string filename = "Orders.csv";

            var csv = new StringBuilder();
            var Header = "OrderID|OrderDate|Toppings|IsToppingVegetarian";
            csv.AppendLine(Header);

            foreach (var list in orders.orders)
            {
                Guid id = Guid.NewGuid();
                var first = id;

                var second = list.orderDate.ToString();
                string toppings = string.Join(",", list.toppings.ToArray());
                var third = toppings;

                var fourth = IsVegetarian(list.toppings);

                var newLine = first + "|" + second + "|" + third + "|" + fourth;

                csv.AppendLine(newLine);
            }
            File.WriteAllText(filename, csv.ToString());

            Console.WriteLine("Written CSV File");
        }

        private static void ReadfromCSV(DAL dal)
        {
            List<Orders> Result = new List<Orders>();
            using (var reader = new StreamReader("Orders.csv"))
            {

                while (!reader.EndOfStream)
                {
                    Orders order = new Orders();
                    var line = reader.ReadLine();
                    var values = line.Split('|');
                    if (DateTime.TryParse(values[1], out order.OrderDate))
                    {
                        order.OrderId = values[0];
                        order.Toppings = values[2];
                        order.IsToppingVegetarian = bool.Parse(values[3]);
                        Result.Add(order);
                    }

                }

            }
            foreach (var item in Result)
            {

                dal.InsertOrders(item);
            };
            Console.WriteLine("Data inserted on DB from CSV file");
        }
        #endregion
        private static bool IsVegetarian(List<string> toppings)
        {
            bool vegan = true;
            foreach (var item in toppings)
            {

                if (animalTopping.Contains(item))
                {
                    vegan = false;
                    break;
                }
            }
            return vegan;
        }
       

        #region Build Table
        static int tableWidth = 110;

        private static void table_Result(string date, int vegan, int nonvegan)
        {
            
            string[] columnsHeader = { "Date Period of Report", "Number of Vegetarian Pizzas Sold", "Number of Non-vegetarian Pizzas Sold" };
            string[] columns = { date, vegan.ToString(), nonvegan.ToString() };
            Console.WriteLine(new string('-', tableWidth));
            PrintRow(columnsHeader);
            Console.WriteLine(new string('-', tableWidth));
            PrintRow(columns);
            Console.WriteLine(new string('-', tableWidth));
            Console.ReadKey();
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
        #endregion

        public class Order
        {
            public DateTime orderDate;
            public List<string> toppings;
        }
        public class rootobject
        {

            public List<Order> orders;
        }
    }
}