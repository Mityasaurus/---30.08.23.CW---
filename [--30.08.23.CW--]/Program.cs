using System.Configuration;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Xml;
using System.Diagnostics;

namespace ___30._08._23.CW___
{
    internal class Program
    {
        private static DbProviderFactory factory;
        private static string connectionString => ConfigurationManager.ConnectionStrings["Default"].ToString();
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Select database\n1 - SqlServer\n2 - Oracle");

                string answer = Console.ReadLine();

                DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

                if (answer == "1")
                {
                    factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["Default"].ProviderName);
                }
                else if (answer == "2")
                {
                    factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["OracleDB"].ProviderName);
                }
                else
                {
                    factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["Default"].ProviderName);
                }

                int choice = -1;
                while (choice != 0)
                {
                    Console.WriteLine("Enter your choice");

                    Console.WriteLine("1 - Показати усю iнформацiю");
                    Console.WriteLine("2 - Показати усiх студентiв");
                    Console.WriteLine("3 - Показати середнi оцiнки студентiв");
                    Console.WriteLine("4 - Показати студентiв з середньою оцiнкою вище вказаної");
                    Console.WriteLine("5 - Додати нового студента");
                    Console.WriteLine();
                    Console.WriteLine("0 - Вихiд");

                    string query = "select * from Marks";
                    choice = int.Parse(Console.ReadLine());

                    switch (choice)
                    {
                        case 1: 
                            Console.Clear();
                            Stopwatch timer = new Stopwatch();
                            timer.Start();
                            await ReadDataAsync(factory, query);
                            timer.Stop();

                            Console.WriteLine($"Elapsed time: {timer.ElapsedMilliseconds}");
                            break;
                        case 2:
                            Console.Clear();
                            string queryForCase2 = "SELECT Name, Lastname FROM Marks";
                            timer = new Stopwatch();
                            timer.Start();
                            await ReadDataAsync(factory, queryForCase2);
                            timer.Stop();

                            Console.WriteLine($"Elapsed time: {timer.ElapsedMilliseconds}");
                            break;
                        case 3:
                            Console.Clear();
                            string queryForCase3 = "SELECT Average FROM Marks";
                            timer = new Stopwatch();
                            timer.Start();
                            await ReadDataAsync(factory, queryForCase3);
                            timer.Stop();

                            Console.WriteLine($"Elapsed time: {timer.ElapsedMilliseconds}");
                            break;
                        case 4:
                            Console.Clear();
                            Console.WriteLine("Enter minimum acceptable mark");
                            double minMark = double.Parse(Console.ReadLine());
                            string queryWithFilter = $"select * from Marks where Average >= {minMark}";
                            timer = new Stopwatch();
                            timer.Start();
                            await ReadDataAsync(factory, queryWithFilter);
                            timer.Stop();

                            Console.WriteLine($"Elapsed time: {timer.ElapsedMilliseconds}");
                            break;
                        //case 5:
                        //Console.Clear();

                        //    Console.WriteLine("Enter name");
                        //    string name = Console.ReadLine();

                        //    Console.WriteLine("Enter lastname");
                        //    string lastname = Console.ReadLine();

                        //    Console.WriteLine("Enter group name");
                        //    string group = Console.ReadLine();

                        //    Console.WriteLine("Enter average mark");
                        //    double average = double.Parse(Console.ReadLine());

                        //    Console.WriteLine("Enter subject with min average mark");
                        //    string minMarkSubject = Console.ReadLine();

                        //    Console.WriteLine("Enter subject with max average mark");
                        //    string maxMarkSubject = Console.ReadLine();

                        //    string addQuery = $"insert into Marks values('{name}', '{lastname}', '{group}', {average}, '{minMarkSubject}', '{maxMarkSubject}')";

                        //    using (SqlCommand cmd = new SqlCommand(addQuery, conn))
                        //    {
                        //        Console.WriteLine(cmd.ExecuteNonQuery());
                        //    }
                        //    break;
                        default:
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task ReadDataAsync(DbProviderFactory factory, string query) //DbProviderFactory factory
        {
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = connectionString;

                await conn.OpenAsync();

                using (DbCommand cmd = factory.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = query;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader.GetName(i).ToString().PadRight(20));
                        }
                        Console.WriteLine();
                        while (reader.Read())
                        {
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader[reader.GetName(i)].ToString().PadRight(20));
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
        static void Classwork()
        {
            string connectionString = "Data Source=Admin-PC\\SQLEXPRESS;Initial Catalog=Academy;Integrated Security=true;Connection Timeout=30;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "select * from Teachers";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "Teachers");
                    foreach (DataRow row in ds.Tables["Teachers"].Rows)
                    {
                        Console.WriteLine(row["Name"]);
                    }



                    //Console.WriteLine(connection.ToString());
                    //using (SqlCommand cmd = new SqlCommand(query, connection))
                    //{
                    //    using (var reader = cmd.ExecuteReader())
                    //    {
                    //        while (reader.Read())
                    //        {
                    //            Console.WriteLine($"{reader}");
                    //        }
                    //    }
                    //}


                    connection.Close();

                }
                //builder.DataSource = @"Admin-PC\SQLEXPRESS";
                //builder.IntegratedSecurity = true;
                //builder.InitialCatalog = "Academy";
                //builder.UserID = "Mark";
                //builder.Password = "123";
                //Console.WriteLine(builder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}