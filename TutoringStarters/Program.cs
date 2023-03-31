using System.Security.Cryptography.X509Certificates;

namespace TutoringStarters
{
    //PLAN:
    //Each topic has a Topic object, that stores the current level of remembering.
    //A handler will manage ensuring that the Topics are up to date, since the level of remembering is a function of time and difficulty
    //Topics will also have their own serialisation method, to store the topics and a "week zero" which will be used to calculate the time for the remembering level function.
    //For finding topics to revisit, a subroutine searches the Topics for the lowest remembering level.
    internal class Program
    {
        static void Main(string[] args)
        {
            (DateTime weekZero, Topic[] topics) = FileHandler.Read("topics.bin");

        }
    }

    public static class FileHandler
    {
        public static void Save(DateTime weekZero, Topic[] topics, string filename)
        {
            ///<summary>
            ///Serialises and saves Topic objects to the file at <paramref name="filename" />. Creates the file if it does not already exist.
            ///</summary>
            ///<param name="weekZero">The date of the week used as a baseline for time calculations</param>
            ///<param name="topics">The array of Topic objects to save</param>
            FileStream outputStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            //Need to implement binary serialisation

        }

        public static (DateTime, Topic[]) Read(string filename)
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
        private DateTime creationDate;

        public string Name { get => name; set => name = value; }
        public int Difficulty { get => difficulty; set => difficulty = value; }
        public double RememberingLevel { get => CalculateRememberingLevel(); }

        public Topic(string name, int difficulty, DateTime creationDate)
        {
            this.name = name;
            this.difficulty = difficulty;
            this.creationDate = creationDate;
        }
        public Topic(string name, int difficulty)
        {
            this.name = name;
            this.difficulty = difficulty;
            creationDate = DateTime.Today;
        }

        private int CalculateWeekOffset()
        {
            DateTime today = DateTime.Today;
            DateTime nextMonday = today.AddDays((8 - (int)today.DayOfWeek) % 7); //Calculation to find the next monday
            return (int)(nextMonday - creationDate).TotalDays / 7; //Find the difference in days and divide by 7 to get weeks.
        }

        private double CalculateRememberingLevel()
        {
            //Use of modified sigmoid logistic function, 1/(1+2^(kt-6))
            return 1 / (1 + Math.Pow(2, difficulty * CalculateWeekOffset() - 6));
        }
    }
}