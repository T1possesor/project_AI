using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class TextUtil
    {
        public static string EscapeSqlLike(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return s.Replace("'", "''");
        }

        public static string TrimEndPunc(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return s.Trim().Trim('.', ',', ';', ':', '!', '?');
        }

        public static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string NormalizeNoSpace(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            text = text.ToLowerInvariant();
            text = RemoveDiacritics(text);
            text = Regex.Replace(text, @"[^a-z0-9]", "");
            return text;
        }

        public static bool ContainsWord(string input, string wordPattern)
        {
            return Regex.IsMatch(input, $@"\b{wordPattern}\b", RegexOptions.IgnoreCase);
        }
    }
}
