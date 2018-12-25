namespace UnravelTravel.Common.Extensions
{
    using System.Text;

    public static class StringExtensions
    {
        public static string SplitWords(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var result = new StringBuilder();
            foreach (var letter in text)
            {
                if (letter >= 65 && letter <= 90)
                {
                    result.Append(" ");
                }

                result.Append(letter);
            }

            return result.ToString();
        }

        public static string RemoveWhiteSpaces(this string text)
        {
            var texWithoutWhitespaces = string.Empty;
            foreach (var ch in text)
            {
                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                texWithoutWhitespaces = texWithoutWhitespaces + ch;
            }

            return texWithoutWhitespaces;
        }
    }
}
