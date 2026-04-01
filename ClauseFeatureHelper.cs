using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace project_AI
{
    internal static class ClauseFeatureHelper
    {
        public static string BuildFeatureText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string s = text.Trim().ToLowerInvariant();

            string authorFlag = HasAny(s, SeedWords.Author) ? "HAS_AUTHOR_SEED" : "";
            string titleFlag = HasAny(s, SeedWords.Title) ? "HAS_TITLE_SEED" : "";
            string categoryFlag = HasAny(s, SeedWords.Category) ? "HAS_CATEGORY_SEED" : "";
            string yearFlag = HasAny(s, SeedWords.Year) ? "HAS_YEAR_SEED" : "";
            string idFlag = HasAny(s, SeedWords.Id) ? "HAS_ID_SEED" : "";

            string has4Digit = Regex.IsMatch(s, @"\b\d{4}\b") ? "HAS_4DIGIT" : "";
            string hasNumber = Regex.IsMatch(s, @"\b\d+\b") ? "HAS_NUMBER" : "";
            string hasNot = Regex.IsMatch(s, @"\b(không|khong|not|!=|<>)\b") ? "HAS_NOT" : "";

            return $"{s} {authorFlag} {titleFlag} {categoryFlag} {yearFlag} {idFlag} {has4Digit} {hasNumber} {hasNot}".Trim();
        }

        private static bool HasAny(string text, IEnumerable<string> seeds)
        {
            foreach (var seed in seeds)
            {
                if (text.Contains(seed))
                    return true;
            }
            return false;
        }
    }
}