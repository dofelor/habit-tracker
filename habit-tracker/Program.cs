using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace habit_tracker
{
    internal class Program
    {
        static string connectionString = @"Data Source = habit-tracker.db";
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

            HabitsMenu();
        }

        static void HabitsMenu()
        {
            Console.Clear();
            bool closeApp = false;
            while (!closeApp)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("Type 0 to Close Application.");
                Console.WriteLine("Type 1 to View your Habits.");
                Console.WriteLine("Type 2 to Insert Habit.");
                Console.WriteLine("Type 3 to Delete Habit.");
                Console.WriteLine("Type 4 to Update Habit Records.");
                Console.WriteLine("-----------------------------------\n");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        Console.WriteLine("#_#");
                        closeApp = true;
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
        

        

        private static void GetAllHabits()
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
                Console.WriteLine($"{habit.Id} | {habit.Name}");
            }
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
            GetAllHabits();
            int habitToDeleteID = GetNumInputForHabits("Select the habit ID to delete it.");

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
            GetAllHabits();
            int habitToUpdateID = GetNumInputForHabits("Select the habit ID to update his name.");
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
            int finalInput = Convert.ToInt32(inputNumber);
            return finalInput;
        }
        private class Habits
        {
            public int Id {  get; set; }
            public string Name { get; set; }
        }
    }
}
