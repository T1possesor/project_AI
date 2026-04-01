using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class NotDetector
    {
        public static bool HasNot(string input)
        {
            return Regex.IsMatch(input,
                @"\b(không\s*phải|khong\s*phai|không\s*có|khong\s*co|không\s*thuộc|khong\s*thuoc|loại\s*bỏ|ngoại\s*trừ|trừ|!=|<>)\b",
                RegexOptions.IgnoreCase);
        }

        public static bool HasNotBeforeTarget(string input, string targetWordPattern)
        {
            return Regex.IsMatch(input,
                $@"\b(không\s*phải|khong\s*phai|không\s*thuộc|khong\s*thuoc|loại\s*bỏ|ngoại\s*trừ|trừ|!=|<>)\b.*\b{targetWordPattern}\b",
                RegexOptions.IgnoreCase);
        }
    }
}
