using System.Runtime.Serialization.Formatters.Binary;

namespace TutoringStarters
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DateTime weekZero = new DateTime(2023, 03, 27);
            Console.WriteLine(CalculateWeekOffset(DateTime.Today, weekZero));
            
        }

        static int CalculateWeekOffset(DateTime currentDay, DateTime weekZero)
        {
            DateTime nextMonday = currentDay.AddDays((8-(int)currentDay.DayOfWeek) % 7); //Calculation to find the next monday
            return (int)(nextMonday - weekZero).TotalDays / 7; //Find the difference in days and divide by 7 to get weeks.
        }

    }

    public static class FileHandler
    {
        static void Save(Topic[] topics, int weekCounter, string filename)
        {
            ///<summary>
            ///Serialises and saves Topic objects to the file at <paramref name="filename" />. Creates the file if it does not already exist.
            ///</summary>
            ///<param name="topics">The array of Topic objects to save</param>
            FileStream outputStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            //Need to implement binary serialisation

        }

        static (Topic[], int) Read(string filename)
        {
            /// <summary>
            /// Reads in a serialised binary file and return its constituent Topic objects.
            /// </summary>
            /// <param name="filename">The filename, including file extension, to read from.</param>
            /// <returns>A tuple with a Topic[] containing the file's topic objects, and an integer which counts weeks.</returns>
            FileStream inputStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(inputStream);
            //Need to implement binary deserialisation
        }
    }

    public class Topic
    {
        private string name;
        private int difficulty;
        private int score;

        public string Name { get => name; set => name = value; }
        public int Difficulty { get => difficulty; set => difficulty = value; }
        public int Score { get => score; set => score = value; }

        public Topic(string name, int difficulty)
        {
            this.name = name;
            this.difficulty = difficulty;
            score = 0;
        }

        public Topic(string name, int difficulty, int score)
        {
            this.name = name;
            this.difficulty = difficulty;
            this.score = score;
        }

        //Use of a sigmoid function, possibly 1/(1+e^(kx-4)), to model forgetting over time
    }
}