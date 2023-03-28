using Microsoft.VisualBasic;

namespace TutoringStarters
{
    internal class Program
    {
        static void Main(string[] args)
        {
            readonly DateTime WEEKZERO = new DateTime(2023, 03, 27);
            Console.WriteLine("Hello, World!");
        }

    }

    static class FileHandler
    {
        static void Save(Topic[] topics, int weekCounter, string filename)
        {
            FileStream outputStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(outputStream);

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