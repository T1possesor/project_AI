using System;
using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class RangeExtractors
    {
        public static void Apply(string input, QueryIntent intent)
        {
            string s = input.Trim();

            var mYearBetween = Regex.Match(s,
                @"\b(từ\s*năm|tu\s*nam|khoảng|khoang)?\s*(?<y1>\d{4})\s*(đến|den|\-)\s*(?<y2>\d{4})\b",
                RegexOptions.IgnoreCase);

            if (mYearBetween.Success)
            {
                int y1 = int.Parse(mYearBetween.Groups["y1"].Value);
                int y2 = int.Parse(mYearBetween.Groups["y2"].Value);
                intent.YearFrom = Math.Min(y1, y2);
                intent.YearTo = Math.Max(y1, y2);
            }

            var mIdBetween = Regex.Match(s,
                @"\b(id|mã|ma)\b.*\b(từ|tu)?\s*(?<i1>\d+)\s*(đến|den|tới|toi|\-)\s*(?<i2>\d+)\b",
                RegexOptions.IgnoreCase);

            if (mIdBetween.Success)
            {
                int i1 = int.Parse(mIdBetween.Groups["i1"].Value);
                int i2 = int.Parse(mIdBetween.Groups["i2"].Value);
                intent.IdFrom = Math.Min(i1, i2);
                intent.IdTo = Math.Max(i1, i2);
            }

            var mBefore = Regex.Match(s,
                @"\b(trước|truoc|<)\s*(năm|nam)?\s*(?<y>\d{4})\b",
                RegexOptions.IgnoreCase);

            if (mBefore.Success)
                intent.YearBefore = int.Parse(mBefore.Groups["y"].Value);

            var mAfter = Regex.Match(s,
                @"\b(sau|trên|tren|>)\s*(năm|nam)?\s*(?<y>\d{4})\b",
                RegexOptions.IgnoreCase);

            if (mAfter.Success)
                intent.YearAfter = int.Parse(mAfter.Groups["y"].Value);
        }
    }
}
