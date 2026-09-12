using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace habit_tracker
{
    internal class Program
    {
        static string connectionString = @"Data Source = habit-tracker.db; Foreign Keys=True;";
        static void Main(string[] args)
        {
            using(var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Habits (
                        HabitId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS HabitLogs(
                        HabitLogsId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Quantity INTEGER NOT NULL,
                        DateAdded TEXT NOT NULL,
                        HabitId INT,
                        FOREIGN KEY (HabitId) REFERENCES Habits(HabitId)
                        ON DELETE CASCADE
                    );";


                
                tableCmd.ExecuteNonQuery();

            }
            bool closeApp = false;
            while (!closeApp)
            {
                Console.WriteLine("Choose the path");
                Console.Write("1 ==> Habits\n2 ==> Habits records\n0 ==> EXIT\n");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        HabitsMenu();
                        break;
                    case "2":
                        HabitRecordsMenu();
                        break;
                    case "0":
                        closeApp = true;
                        break;
                    default: 
                        Console.WriteLine("err");
                        break;
                }

            }
            
        }


       
        static void HabitRecordsMenu()
        {
            Console.Clear();
            bool closeApp = false;

            List<Habits> habitList = GetAllHabits();
            
            
            int choice = GetNumInputForHabits("Select the habit ID to edit records.");
            Habits selectedHabit = habitList.FirstOrDefault(h => h.Id == choice);

            if (selectedHabit == null)
            {
                Console.Clear();
                Console.WriteLine($"\nHabit with ID {choice} doesn't exist!\n");
                return;
            }
            Console.Clear();
            while (!closeApp)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine($"\nWhat would you like to do with - {selectedHabit.Name}?");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("Type 1 to View All Habit Records.");
                Console.WriteLine("Type 2 to Insert Habit Record.");
                Console.WriteLine("Type 3 to Delete Habit Record.");
                Console.WriteLine("Type 4 to Update Habit Record.");
                Console.WriteLine("-----------------------------------\n");

                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        Console.Clear();
                        closeApp = true;
                        break;
                    case "1":
                        GetAllRecords(selectedHabit);
                        break;
                    case "2":
                        Insert(selectedHabit);
                        break;
                    case "3":
                        Delete(selectedHabit);
                        break;
                    case "4":
                        Update(selectedHabit);
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Pls type a number from 0 to 4.\n");
                        break;
                }

            }

        }

        private static void GetAllRecords(Habits habit)
        {
            Console.Clear();
            List<HabitLogs> tableData = new();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = """
                    SELECT * FROM HabitLogs
                    WHERE HabitId = @habitId;
                    """;

                tableCmd.Parameters.AddWithValue("@habitId", habit.Id);

                SqliteDataReader sqliteDataReader = tableCmd.ExecuteReader();

                if (sqliteDataReader.HasRows)
                {
                    while (sqliteDataReader.Read())
                    {
                        tableData.Add(
                            new HabitLogs
                            {
                                Id = sqliteDataReader.GetInt32(0),
                                Quantity = sqliteDataReader.GetInt32(1),
                                Date = DateTime.ParseExact(sqliteDataReader.GetString(2), "dd-MM-yy", new CultureInfo("en-us")),
                                HabitId = sqliteDataReader.GetInt32(3)
                            });
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                    connection.Close();
                    return;
                }

                
            }
            Console.WriteLine("------------------------------------------\n");
            foreach (var data in tableData)
            {
                Console.WriteLine($"{data.Id} - Name: {habit.Name} - {data.Date.ToString("dd-MMM-yyyy")} - Quantity: {data.Quantity}");
            }
            Console.WriteLine("------------------------------------------\n");
        }

        private static void Insert(Habits habit)
        {
            string date = GetDateInput();

            int quantity = GetNumInputForHabits("\n\nSpecify the quantity of the habit.\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "INSERT INTO HabitLogs VALUES (Null, @Quantity, @Date, @HabitId)";
                tableCmd.Parameters.AddWithValue("@Quantity", quantity);
                tableCmd.Parameters.AddWithValue("@Date", date);
                tableCmd.Parameters.AddWithValue("@HabitId", habit.Id);


                tableCmd.ExecuteNonQuery();

            }
            Console.Clear();
            Console.WriteLine("Record was added");

        }

        private static void Delete(Habits habit)
        {
            GetAllRecords(habit);

            var recordId = GetNumInputForHabits("\n\nPlease type the Id of the record you want to delete.\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "DELETE FROM HabitLogs WHERE HabitLogsId = @RecordId";
                tableCmd.Parameters.AddWithValue("@RecordId", recordId);

                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n\n");
                    return;
                }
            }
            Console.Clear();
            Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n\n");
        }

        private static void Update(Habits habit)
        {
            Console.Clear();
            GetAllRecords(habit);

            var recordId = GetNumInputForHabits("\n\nPlease type Id of the record would like to update.\n\n");


            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT EXISTS(SELECT 1 FROM HabitLogs WHERE HabitLogsId = @RecordId)";
                checkCmd.Parameters.AddWithValue("@RecordId", recordId);
                int checkquery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkquery == 0)
                {
                    connection.Close();
                    Console.Clear();
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
                    return;
                }

                string date = GetDateInput();

                int quantity = GetNumInputForHabits("\n\nSpecify the quantity of the habit.\n\n");

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = "UPDATE HabitLogs SET DateAdded = @Date, Quantity = @Quantity WHERE HabitLogsId = @RecordId";
                tableCmd.Parameters.AddWithValue("@Date", date);
                tableCmd.Parameters.AddWithValue("@Quantity", quantity);
                tableCmd.Parameters.AddWithValue("@RecordId", recordId);

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy).\n\n");

            string dateInput = Console.ReadLine();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-us"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy).\n\n");
                dateInput = Console.ReadLine();
            }

            return dateInput;

        }

        // ____________________________________________________________ //
        // habits menu

        static void HabitsMenu()
        {
            Console.Clear();
            bool closeMenu = false;
            while (!closeMenu)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("Type 0 to Close Gate.");
                Console.WriteLine("Type 1 to View your Habits.");
                Console.WriteLine("Type 2 to Insert Habit.");
                Console.WriteLine("Type 3 to Delete Habit.");
                Console.WriteLine("Type 4 to Update Habit Records.");
                Console.WriteLine("-----------------------------------\n");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        Console.Clear();
                        closeMenu = true;
                        break;
                    case "1":
                        GetAllHabits();
                        break;
                    case "2":
                        InsertHabit();
                        break;
                    case "3":
                        DeleteHabit();
                        break;
                    case "4":
                        UpdateHabit();
                        break;
                    default:
                        Console.WriteLine("Error");
                        break;
                }
            }
        }
        

        private static List<Habits> GetAllHabits()
        {
            Console.Clear();
            List<Habits> habitsDataFromTable = new();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = """SELECT * FROM Habits""";
                SqliteDataReader reader = tableCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        habitsDataFromTable.Add(
                                new Habits
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                }
                            );
                    }
                }
                else
                {
                    Console.WriteLine("Habits not found!");
                }
            }

            foreach (var habit in habitsDataFromTable)
            {
                Console.WriteLine($"ID:{habit.Id} | habit name: {habit.Name}");
            }

            return habitsDataFromTable;
        }

        private static void InsertHabit()
        {
            Console.Clear();
            Console.WriteLine("Enter the name of the habit.");
            string nameHabit = Console.ReadLine();

            using(var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = """
                    INSERT INTO Habits
                    VALUES (NULL, @nameHabit);
                    """;
                tableCmd.Parameters.AddWithValue("@nameHabit", nameHabit);
                tableCmd.ExecuteNonQuery();
            }
            Console.Clear();
            Console.WriteLine($"Habit {nameHabit} was added.");
        }

        private static void DeleteHabit()
        {
            Console.Clear();
            List<Habits> habitList = GetAllHabits();
            int habitToDeleteID = GetNumInputForHabits("Select the habit ID to delete it.");
            Habits selectedHabit = habitList.FirstOrDefault(h => h.Id == habitToDeleteID);
            if (selectedHabit == null)
            {
                Console.Clear();
                Console.WriteLine($"\nHabit with ID {habitToDeleteID} doesn't exist!\n");
                return;
            }

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open( );
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = """
                    DELETE FROM Habits
                    WHERE HabitId = @InputID;
                    """;
                tableCmd.Parameters.AddWithValue("@InputID", habitToDeleteID);
                tableCmd.ExecuteNonQuery();
            }
            Console.Clear() ;
            Console.WriteLine("Habit was delete.");
        }

        private static void UpdateHabit()
        {
            Console.Clear( );
            List<Habits> habitList = GetAllHabits();
            int habitToUpdateID = GetNumInputForHabits("Select the habit ID to update his name.");
            Habits selectedHabit = habitList.FirstOrDefault(h => h.Id == habitToUpdateID);
            if (selectedHabit == null)
            {
                Console.Clear();
                Console.WriteLine($"\nHabit with ID {habitToUpdateID} doesn't exist!\n");
                return;
            }
            Console.WriteLine("Pls update the name of habit: ");
            string updateNameHabit = Console.ReadLine();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = """
                    UPDATE Habits
                    SET Name = @Name
                    WHERE HabitId = @InputID;
                    """;
                tableCmd.Parameters.AddWithValue("@InputID", habitToUpdateID);
                tableCmd.Parameters.AddWithValue("@Name", updateNameHabit);
                tableCmd.ExecuteNonQuery();
            }
            Console.Clear();
            Console.WriteLine("Habit was updated.");
        }
        private static int GetNumInputForHabits(string message)
        {
            Console.WriteLine(message);
            string inputNumber = Console.ReadLine();
            int finalInput;
            while (!int.TryParse(inputNumber, out finalInput) || finalInput < 0)
            {
                Console.WriteLine("\nInvalid number. Please enter a valid non-negative integer:\n");
                inputNumber = Console.ReadLine();
            }
            return finalInput;
        }
        private class Habits
        {
            public int Id {  get; set; }
            public string Name { get; set; }
        }

        private class HabitLogs
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public DateTime Date { get; set; }
            public int HabitId { get; set; }
        }
    }
}
