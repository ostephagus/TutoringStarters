﻿

using System.Globalization;

namespace TutoringStarters
{
    //PLAN:
    //Each topic has a Topic object, that stores the current level of remembering.
    //A handler will manage ensuring that the Topics are up to date, since the level of remembering is a function of time and difficulty
    //Topics will also have their own serialisation method, to store the topics and a "last studied" to calculate the remembering level.
    //For finding topics to revisit, a subroutine searches the Topics for the lowest remembering level.
    public class Program
    {
        public static void Main(string[] args)
        {
            Topic[] topics =
            {
                new Topic("Algebra: Unknowns on both sides", 3, new DateTime(2023, 4, 17)),
                new Topic("Probability: Venn Diagrams", 4, new DateTime(2023, 4, 17)),
                new Topic("Proportional Representation: multiplying fractions", 2, new DateTime(2023, 3, 27)),
                new Topic("Proportional Representation: ratio simplification", 1, new DateTime(2023, 3, 20)),
                new Topic("Proportional Representation: splitting into ratio", 2, new DateTime(2023, 3, 13)),
                new Topic("Proportional Representation: adding fractions", 1, new DateTime(2023, 3, 6))
            };
            FileHandler.Save(topics, "topics.bin");

            Topic[] topicsDeserialised = FileHandler.Read("topics.bin");
            Topic[] top5 = GetTop5(topicsDeserialised);
            
        }

        public static Topic CreateNewTopic()
        {
            Console.WriteLine("Enter name for topic");
            string name = Console.ReadLine();
            int difficulty;
            string difficultyString;
            do
            {
                Console.WriteLine("Enter relative difficult for topic (logarithmic-ish scale, 1 = completely forgotten in 10 weeks, 10 = completely forgotten in 1 week)");
                difficultyString = Console.ReadLine();
            } while (!int.TryParse(difficultyString, out difficulty));
            return new Topic(name, difficulty);
        }

        public static void UpdateTopic(ref Topic topic)
        {
            Console.WriteLine($"The current difficulty rating of this topic is {topic.Difficulty}. What would you like to change it to?");
            int difficulty;
            string difficultyString;
            do
            {
                Console.WriteLine("Enter relative difficult for topic (logarithmic-ish scale, 1 = completely forgotten in 10 weeks, 10 = completely forgotten in 1 week)");
                difficultyString = Console.ReadLine();
            } while (!int.TryParse(difficultyString, out difficulty));
            topic.Difficulty = difficulty;
        }

        public static Topic[] GetTop5(Topic[] topics)
        {
            //Go through each topic and find its Remembering Level, based on these assign top 5.
            
            List<Topic> top5 = new List<Topic>();

            foreach (Topic topic in topics)
            {
                double currentRememberingLevel = topic.RememberingLevel;
                for (int i = 0; i < 5; i++)
                {
                    if (i >= top5.Count)
                    {
                        top5.Add(topic);
                        break;
                    }
                    else if (currentRememberingLevel < top5[i].RememberingLevel)
                    {
                        top5.Insert(i, topic);
                        break;
                    }
                }
                if (top5.Count > 5) 
                {
                    top5.RemoveAt(5); //Keep the list length to 5 (mem efficiency)
                }
            }
            return top5.ToArray();
        }
    }

    public static class FileHandler
    {
        public static void Save(Topic[] topics, string filename)
        {
            ///<summary>
            ///Serialises and saves an array of Topic objects to the file at <paramref name="filename" />. Creates the file if it does not already exist.
            ///</summary>
            ///<param name="topics">The array of Topic objects to save.</param>
            ///<param name="filename">The name of the binary file to save to.</param>
            FileStream outputStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(outputStream);
            
            foreach (Topic topic in topics)
            {
                writer.Write(SerialiseTopic(topic));
            }

            writer.Close();
        }

        private static byte[] SerialiseTopic(Topic topic)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(SerialiseString(topic.Name));
            bytes.Add((byte)topic.Difficulty);
            bytes.AddRange(BitConverter.GetBytes(topic.LastStudied.ToBinary()));
            return bytes.ToArray();
        }

        private static byte[] SerialiseString(string str)
        {
            List<byte> bytes = new List<byte>();
            foreach (char c in str)
            {
                bytes.Add((byte)c);
            }
            bytes.Add(4); //Add an EOT (ascii 4) to signal the end of the string, since the string is of variable length.
            return bytes.ToArray();
        }

        public static Topic[] Read(string filename)
        {
            /// <summary>
            /// Reads in a serialised binary file and return its constituent Topic objects.
            /// </summary>
            /// <param name="filename">The filename, including file extension, to read from.</param>
            /// <returns>A tuple with a Topic[] containing the file's topic objects, and an integer which counts weeks.</returns>
            List<Topic> topics = new List<Topic>();
            FileStream inputStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(inputStream);
            while (inputStream.Position < inputStream.Length) //While the current position is before the end of the file
            {
                byte nextByte = reader.ReadByte();
                string description = "";
                while (nextByte != 4) //Loop for reading the Topic's description. 4 is ascii end of transmission (EOT)
                {
                    description += (char)nextByte;
                    nextByte = reader.ReadByte();
                }
                int difficulty = reader.ReadByte();
                DateTime lastStudied = DateTime.FromBinary(BitConverter.ToInt64(reader.ReadBytes(8))); //Complex deserialisation from 8 bytes to a long and then to a datetime.
                topics.Add(new Topic(description, difficulty, lastStudied));
            }
            return topics.ToArray();
        }
    }

    public class Topic : IEquatable<Topic>
    {
        private string name;
        private int difficulty;
        private readonly DateTime lastStudied;

        public string Name { get => name; set => name = value; }
        public int Difficulty { get => difficulty; set => difficulty = value; }
        public double RememberingLevel { get => CalculateRememberingLevel(); }
        public DateTime LastStudied { get => lastStudied; }

        public Topic(string name, int difficulty, DateTime lastStudied)
        {
            this.name = name;
            this.difficulty = difficulty;
            this.lastStudied = lastStudied;
        }
        public Topic(string name, int difficulty)
        {
            this.name = name;
            this.difficulty = difficulty;
            lastStudied = DateTime.Today;
        }

        private int CalculateWeekOffset()
        {
            DateTime today = DateTime.Today;
            DateTime nextMonday = today.AddDays((8 - (int)today.DayOfWeek) % 7); //Calculation to find the next monday
            return (int)(nextMonday - lastStudied).TotalDays / 7; //Find the difference in days and divide by 7 to get weeks.
        }

        private double CalculateRememberingLevel()
        {
            //Use of modified sigmoid logistic function, 1/(1+e^(kt-6))
            return 1 / (1 + Math.Exp(difficulty * CalculateWeekOffset() - 6));
        }

        public bool Equals(Topic other)
        {
            return name == other.Name && difficulty == other.difficulty && lastStudied == other.lastStudied;
        }
    }
}