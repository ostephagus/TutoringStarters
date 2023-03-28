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

    static class FileHandler
    {
        static void Save(Topic[] topics, int weekCounter, string filename)
        {
            FileStream outputStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(outputStream, topics);

        }

        static (Topic[], int) Read(string filename)
        {
            FileStream inputStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(inputStream);
        }
    }

    public class Topic
    {
        private string name;
        private int forgettingFactor;
        private int score;

        public string Name { get => name; set => name = value; }
        public int ForgettingFactor { get => forgettingFactor; set => forgettingFactor = value; }
        public int Score { get => score; set => score = value; }

        public Topic(string name, int forgettingFactor)
        {
            this.name = name;
            this.forgettingFactor = forgettingFactor;
            score = 0;
        }

        public Topic(string name, int forgettingFactor, int score) : this(name, forgettingFactor)
        {
            this.score = score;
        }
    }
}