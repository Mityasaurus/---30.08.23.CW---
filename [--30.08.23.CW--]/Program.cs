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
        private static Stopwatch timer = new Stopwatch();
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

                    Console.WriteLine("1 - Показати всю iнформацiю");
                    Console.WriteLine("2 - Показати усiх студентiв");
                    Console.WriteLine("3 - Показати середнi оцiнки студентiв");
                    Console.WriteLine("4 - Показати студентiв з середньою оцiнкою вище вказаної");
                    Console.WriteLine("5 - Додати нового студента");
                    Console.WriteLine("6 - Видалили iснуючого студента");
                    Console.WriteLine();
                    Console.WriteLine("0 - Вихiд");

                    string query = "select * from Marks";
                    choice = int.Parse(Console.ReadLine());

                    switch (choice)
                    {
                        case 1: 
                            Console.Clear();
                            StartTimer();
                            await ReadDataAsync(factory, query);
                            Console.WriteLine($"\nElapsed time: {StopTimer()} ms");
                            break;
                        case 2:
                            Console.Clear();
                            string queryForCase2 = "SELECT Name, Lastname FROM Marks";
                            StartTimer();
                            await ReadDataAsync(factory, queryForCase2);
                            Console.WriteLine($"\nElapsed time: {StopTimer()} ms");
                            break;
                        case 3:
                            Console.Clear();
                            string queryForCase3 = "SELECT Average FROM Marks";
                            StartTimer();
                            await ReadDataAsync(factory, queryForCase3);
                            Console.WriteLine($"\nElapsed time: {StopTimer()} ms");
                            break;
                        case 4:
                            Console.Clear();
                            Console.WriteLine("Enter minimum acceptable mark");
                            double minMark = double.Parse(Console.ReadLine());
                            string queryWithFilter = $"select * from Marks where Average >= {minMark}";
                            StartTimer();
                            await ReadDataAsync(factory, queryWithFilter);
                            Console.WriteLine($"\nElapsed time: {StopTimer()} ms");
                            break;
                        case 5:
                            Console.Clear();
                            string addQuery = GetAddStudentQuery();
                            StartTimer();
                            await ExecCommandAsync(factory, addQuery);
                            Console.WriteLine($"\nElapsed time: {StopTimer()} ms");
                            break;
                        case 6:
                            Console.Clear();
                            string deleteQuery = GetDeleteStudentQuery();
                            StartTimer();
                            await ExecCommandAsync(factory, deleteQuery);
                            Console.WriteLine($"\nElapsed time: {StopTimer()} ms");
                            break;
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

        private static void StartTimer()
        {
            timer.Reset();
            timer.Start();
        }
        private static long StopTimer()
        {
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        private static async Task ReadDataAsync(DbProviderFactory factory, string query)
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
                        Console.WriteLine("\n");
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

                await conn.CloseAsync();
            }
        }
        private static async Task ExecCommandAsync(DbProviderFactory factory, string query)
        {
            using(DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = connectionString;

                await conn.OpenAsync();

                using(DbCommand cmd = factory.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = query;

                    Console.WriteLine(cmd.ExecuteNonQuery());
                }

                await conn.CloseAsync();
            }
        }

        private static string GetAddStudentQuery()
        {
            Console.WriteLine("Enter name");
            string name = Console.ReadLine();

            Console.WriteLine("Enter lastname");
            string lastname = Console.ReadLine();

            Console.WriteLine("Enter group name");
            string group = Console.ReadLine();

            Console.WriteLine("Enter average mark");
            double average = double.Parse(Console.ReadLine());

            Console.WriteLine("Enter subject with min average mark");
            string minMarkSubject = Console.ReadLine();

            Console.WriteLine("Enter subject with max average mark");
            string maxMarkSubject = Console.ReadLine();

            return $"insert into Marks values('{name}', '{lastname}', '{group}', {average}, '{minMarkSubject}', '{maxMarkSubject}')";
        }
        private static string GetDeleteStudentQuery()
        {
            Console.WriteLine("Enter the id of the student you want to delete");
            string id = Console.ReadLine();

            return $"delete from Marks where ID={id}";
        }
    }
}