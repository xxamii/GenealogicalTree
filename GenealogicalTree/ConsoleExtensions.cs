using System.Globalization;
using System.Text;

namespace GenealogicalTree
{
    public static class ConsoleExtensions
    {
        public static string ReadNonEmpty(string input)
        {
            while (input == null || input.Trim().Length < 1)
            {
                Console.Write("Can not be empty: ");
                input = Console.ReadLine();
            }

            return input;
        }

        public static int ReadInt(string input)
        {
            while (!int.TryParse(input, out _))
            {
                Console.WriteLine();
                Console.Write("Please enter a whole number: ");
                input = Console.ReadLine();
            }

            return int.Parse(input);
        }

        public static string ReadSpecificString(string input, List<string> strings)
        {
            string desiredInput = ConvertListToString(strings, ", ");

            while (!ContainsAny(input, strings))
            {
                Console.Write($"({desiredInput}): ");
                input = Console.ReadLine().ToLower();
            }

            return input;
        }

        public static DateTime ReadDate(string input)
        {
            string format = "dd.MM.yyyy";
            DateTime date = DateTime.MinValue;

            while (!DateTime.TryParseExact(input, format, null, DateTimeStyles.None, out date))
            {
                Console.Write("Please enter a valid date: ");
                input = Console.ReadLine().ToLower();
            }

            return date;
        }

        public static List<int> ReadIntList(string input)
        {
            List<int> list = new List<int>();

            StringBuilder current = new StringBuilder();

            foreach(char c in input)
            {
                string element = c.ToString();

                if (int.TryParse(element, out _))
                {
                    current.Append(element);
                }
                else
                {
                    if (current.Length > 0)
                    {
                        list.Add(int.Parse(current.ToString()));
                        current = new StringBuilder();
                    }
                }
            }

            if (current.Length > 0)
            {
                list.Add(int.Parse(current.ToString()));
            }

            return list;
        }

        private static bool ContainsAny(string input, List<string> strings)
        {
            foreach (string s in strings)
            {
                if (input.Contains(s))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ConvertListToString(List<string> strings, string separator)
        {
            StringBuilder result = new StringBuilder();

            foreach (string s in strings)
            {
                result.Append(s);
                result.Append(separator);
            }

            result.Remove(result.Length - separator.Length, separator.Length);
            return result.ToString();
        }
    }
}
