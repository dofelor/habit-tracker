//using System;
//using System.Globalization;
//using Microsoft.Data.Sqlite;

//namespace habit_tracker
//{
//    internal class Program1
//    {
//        static string connectionString = @"Data Source=habit-Tracker.db";

//        static void Main(string[] args)
//        {

//            using (var connection = new SqliteConnection(connectionString))
//            {
//                connection.Open();
//                var tableCmd = connection.CreateCommand();

//                tableCmd.CommandText =
//                    @"CREATE TABLE IF NOT EXISTS drinking_water (
//                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
//                        Date TEXT,
//                        Quantity INTEGER
//                        )";


//                tableCmd.ExecuteNonQuery();

//                connection.Close();
//            }

//            GetUserInput();
//        }

//        static void GetUserInput()
//        {
//            Console.Clear();
//            bool closeApp = false;
//            while (!closeApp)
//            {
//                Console.WriteLine("\n\nMAIN MENU");
//                Console.WriteLine("\nWhat would you like to do?");
//                Console.WriteLine("\nType 0 to Close Application.");
//                Console.WriteLine("Type 1 to View All Records.");
//                Console.WriteLine("Type 2 to Insert Record.");
//                Console.WriteLine("Type 3 to Delete Record.");
//                Console.WriteLine("Type 4 to Update Record.");
//                Console.WriteLine("-----------------------------------\n");

//                string command = Console.ReadLine();

//                switch (command)
//                {
//                    case "0":
//                        Console.WriteLine("\nGoodbye!\n");
//                        closeApp = true;
//                        Environment.Exit(0);
//                        break;
//                    case "1":
//                        GetAllRecords();
//                        break;
//                    case "2":
//                        Insert();
//                        break;
//                    case "3":
//                        Delete();
//                        break;
//                    case "4":
//                        Update();
//                        break;
//                    default:
//                        Console.WriteLine("\nInvalid Command. Pls type a number from 0 to 4.\n");
//                        break;
//                }

//            }

//        }

//        private static void GetAllRecords()
//        {
//            using(var connection = new SqliteConnection(connectionString))
//            {
//                connection.Open();

//                var tableCmd = connection.CreateCommand();

//                tableCmd.CommandText = $"SELECT * FROM drinking_water";

//                List<DrinkingWater> tableData = new();

//                SqliteDataReader sqliteDataReader = tableCmd.ExecuteReader();

//                if (sqliteDataReader.HasRows)
//                {
//                    while (sqliteDataReader.Read())
//                    {
//                        tableData.Add(
//                            new DrinkingWater
//                            {
//                                Id = sqliteDataReader.GetInt32(0),
//                                Date = DateTime.ParseExact(sqliteDataReader.GetString(1), "dd-MM-yy", new CultureInfo("en-us")),
//                                Quantity = sqliteDataReader.GetInt32(2)
//                            });
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("No rows found");
//                }

//                connection.Close();

//                Console.WriteLine("------------------------------------------\n");
//                foreach(var data in tableData)
//                {
//                    Console.WriteLine($"{data.Id} - {data.Date.ToString("dd-MMM-yyyy")} - Quantity: {data.Quantity}");
//                }
//                Console.WriteLine("------------------------------------------\n");
//            }
//        }

//        private static void Insert()
//        {
//            string date = GetDateInput();

//            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed)\n\n");

//            using (var connection = new SqliteConnection(connectionString))
//            {
//                connection.Open();
//                var tableCmd = connection.CreateCommand();
//                //tableCmd.CommandText =
//                //    $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";
//                tableCmd.CommandText = "INSERT INTO drinking_water(date, quantity) VALUES (@Date, @Quantity)";
//                tableCmd.Parameters.AddWithValue("@Date", date);
//                tableCmd.Parameters.AddWithValue("@Quantity", quantity);


//                tableCmd.ExecuteNonQuery();

//                connection.Close();
//            }

//        }

//        private static void Delete()
//        {
//            GetAllRecords();

//            var recordId = GetNumberInput("\n\nPlease type the Id of the record you want to delete or type 0 to go back to Main Menu\n\n");

//            using(var connection = new SqliteConnection(connectionString))
//            {
//                connection.Open();

//                var tableCmd = connection.CreateCommand();
//                tableCmd.CommandText = "DELETE FROM drinking_water WHERE Id = @RecordId";
//                tableCmd.Parameters.AddWithValue("@RecordId", recordId);

//                int rowCount = tableCmd.ExecuteNonQuery();

//                if (rowCount == 0)
//                {
//                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n\n");
//                    Delete();
//                }
//            }

//            Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n\n");

//            GetUserInput();
//        }

//        private static void Update()
//        {
//            Console.Clear();
//            GetAllRecords();

//            var recordId = GetNumberInput("\n\nPlease type Id of the record would like to update. Type 0 to return to main manu.\n\n");


//            using (var connection = new SqliteConnection(connectionString))
//            {
//                connection.Open();

//                var checkCmd = connection.CreateCommand();
//                checkCmd.CommandText = "SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = @RecordId)";
//                checkCmd.Parameters.AddWithValue("@RecordId", recordId);
//                int checkquery = Convert.ToInt32(checkCmd.ExecuteScalar());

//                if(checkquery == 0)
//                {
//                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
//                    connection.Close();
//                    Update();
//                }

//                string date = GetDateInput();

//                int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed)\n\n");

//                var tableCmd = connection.CreateCommand();
//                tableCmd.CommandText = "UPDATE drinking_water SET date = @Date, quantity = @Quantity WHERE Id = @RecordId";
//                tableCmd.Parameters.AddWithValue("@Date", date);
//                tableCmd.Parameters.AddWithValue("@Quantity", quantity);
//                tableCmd.Parameters.AddWithValue("@RecordId", recordId);

//                tableCmd.ExecuteNonQuery();

//                connection.Close();
//            }
//        }


//        internal static string GetDateInput()
//        {
//            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main manu.\n\n");

//            string dateInput = Console.ReadLine();

//            if (dateInput == "0") GetUserInput();

//            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-us"), DateTimeStyles.None, out _))
//            {
//                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main manu or try again:\n\n");
//                dateInput = Console.ReadLine();
//            }

//            return dateInput;

//        }

//        internal static int GetNumberInput(string message)
//        {
//            Console.WriteLine(message);

//            string numberInput = Console.ReadLine();

//            if (numberInput == "0") GetUserInput();

//            int finalInput = Convert.ToInt32(numberInput);

//            return finalInput;
//        }

//        public class DrinkingWater
//        {
//            public int Id { get; set; }
//            public DateTime Date { get; set; }
//            public int Quantity { get; set; }
//        }

//    }
//}

