using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace project_AI
{
    internal static class YearExtractor
    {
        static readonly string YEAR_TERMS =
            @"năm|nam|" +
            @"phát\s*hành|phat\s*hanh|" +
            @"ra\s*mắt|ra\s*mat|" +
            @"xuất\s*bản|xuat\s*ban|" +
            @"in\s*ấn|in\s*an|" +
            @"publish|published|publication|" +
            @"year";

        public static void Apply(string input, QueryIntent intent)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            string s = input;

            if (Regex.IsMatch(s,
                @"\b(liệt\s*kê|danh\s*sách|list)\b.*\b(năm|year)\b",
                RegexOptions.IgnoreCase))
            {
                intent.WantListYears = true;
                return;
            }

            var mRange = Regex.Match(s,
                @"\b(từ|trong\s*khoảng|giai\s*đoạn)\b.*?\b(năm)?\s*(?<y1>\d{4})\b\s*" +
                @"(đến|tới|\-)\s*\b(năm)?\s*(?<y2>\d{4})\b",
                RegexOptions.IgnoreCase);

            if (mRange.Success)
            {
                int y1 = int.Parse(mRange.Groups["y1"].Value);
                int y2 = int.Parse(mRange.Groups["y2"].Value);

                intent.YearFrom = System.Math.Min(y1, y2);
                intent.YearTo   = System.Math.Max(y1, y2);

                intent.YearValues = null;
                intent.YearEq = null;
                intent.YearBefore = null;
                intent.YearAfter = null;
                intent.YearNot = false;

                return;
            }

            var mAfter = Regex.Match(s,
                @"\b(sau)\b.*?\b(năm)?\s*(?<y>\d{4})\b",
                RegexOptions.IgnoreCase);

            if (mAfter.Success)
            {
                intent.YearAfter = int.Parse(mAfter.Groups["y"].Value);
                return;
            }

            var mBefore = Regex.Match(s,
                @"\b(trước)\b.*?\b(năm)?\s*(?<y>\d{4})\b",
                RegexOptions.IgnoreCase);

            if (mBefore.Success)
            {
                intent.YearBefore = int.Parse(mBefore.Groups["y"].Value);
                return;
            }

            var mMulti = Regex.Matches(s, @"\b\d{4}\b");
            if (mMulti.Count >= 2)
            {
                intent.YearValues = mMulti
                    .Select(m => int.Parse(m.Value))
                    .Distinct()
                    .ToList();

                if (Regex.IsMatch(s, @"\b(và|and)\b", RegexOptions.IgnoreCase))
                    intent.YearJoiner = "AND";
                else
                    intent.YearJoiner = "OR";

                intent.YearNot = Regex.IsMatch(s,
                    @"\b(không|loại|trừ)\b",
                    RegexOptions.IgnoreCase);

                return;
            }

            var mNot = Regex.Match(s,
                @"\b(không\s*phải|không\s*có|không\s*thuộc)\b.*?\b(?<y>\d{4})\b",
                RegexOptions.IgnoreCase);

            if (mNot.Success)
            {
                intent.YearEq = int.Parse(mNot.Groups["y"].Value);
                intent.YearNot = true;
                return;
            }

            var mNotPublish = Regex.Match(s,
                @"\b(không)\b.*\b(phát\s*hành|phat\s*hanh|ra\s*mắt|ra\s*mat|xuất\s*bản|xuat\s*ban|publish(?:ed)?)\b.*\b(năm|nam|year)\b\s*(?<y>\d{4})\b",
                RegexOptions.IgnoreCase);

            if (mNotPublish.Success)
            {
                intent.YearEq = int.Parse(mNotPublish.Groups["y"].Value);
                intent.YearNot = true;
                return;
            }

            var mExact = Regex.Match(s,
                $@"\b({YEAR_TERMS})\b.*?\b(là|=|:)?\b\s*(?<y>\d{{4}})\b",
                RegexOptions.IgnoreCase);

            if (mExact.Success)
            {
                intent.YearEq = int.Parse(mExact.Groups["y"].Value);
                intent.YearNot = false;
            }
        }
    }
}