namespace GenealogicalTree
{
    public static class StringExtensions
    {
        public static string Capitalize(this string input)
        {
            if (input == null || input.Length == 0)
            {
                return string.Empty;
            }

            return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
        }
    }
}
