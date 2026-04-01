using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class ComparatorExtractor
    {
        public static void Apply(string input, QueryIntent intent)
        {



            string s = input.Trim();

            var mIdGt = Regex.Match(s,
                @"\b(id|mã|ma)\b\s*(lớn\s*hơn|lon\s*hon|>|>=)\s*(?<n>\d+)\b",
                RegexOptions.IgnoreCase);

            if (mIdGt.Success)
            {
                intent.IdGreaterThan = int.Parse(mIdGt.Groups["n"].Value);
                return;
            }

            var mIdLt = Regex.Match(s,
                @"\b(id|mã|ma)\b\s*(nhỏ\s*hơn|nho\s*hon|<|<=)\s*(?<n>\d+)\b",
                RegexOptions.IgnoreCase);

            if (mIdLt.Success)
            {
                intent.IdLessThan = int.Parse(mIdLt.Groups["n"].Value);
                return;
            }

            var mYearGt = Regex.Match(s,
    @"\b(năm|nam)\b\s*(lớn\s*hơn|lon\s*hon|>|>=)\s*(?<y>\d{1,4})\b",
    RegexOptions.IgnoreCase);


            if (mYearGt.Success)
            {
                intent.YearAfter = int.Parse(mYearGt.Groups["y"].Value);
                return;
            }

            var mYearLt = Regex.Match(s,
    @"\b(năm|nam)\b\s*(nhỏ\s*hơn|nho\s*hon|<|<=)\s*(?<y>\d{1,4})\b",
    RegexOptions.IgnoreCase);


            if (mYearLt.Success)
            {
                intent.YearBefore = int.Parse(mYearLt.Groups["y"].Value);
                return;
            }
            var mAfterYear = Regex.Match(s,
                @"\b(sau|trên|tren)\b.*\b(năm|nam)\b\s*(?<y>\d{1,4})\b",
                RegexOptions.IgnoreCase);

            if (mAfterYear.Success)
            {
                intent.YearAfter = int.Parse(mAfterYear.Groups["y"].Value);
                return;
            }

            var mBeforeYear = Regex.Match(s,
                @"\b(trước|truoc)\b.*\b(năm|nam)\b\s*(?<y>\d{1,4})\b",
                RegexOptions.IgnoreCase);

            if (mBeforeYear.Success)
            {
                intent.YearBefore = int.Parse(mBeforeYear.Groups["y"].Value);
                return;
            }


        }
    }
}
