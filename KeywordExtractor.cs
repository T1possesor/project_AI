using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class KeywordExtractor
    {
        public static void Apply(string input, QueryIntent intent)
        {

            if (Regex.IsMatch(input, @"\b(tác\s*giả|tac\s*gia|author)\b", RegexOptions.IgnoreCase))
                return;


            if (!string.IsNullOrWhiteSpace(intent.Title))
                return;

            if (intent.IdGreaterThan.HasValue || intent.IdLessThan.HasValue ||
                intent.YearBefore.HasValue || intent.YearAfter.HasValue ||
                intent.Id.HasValue || intent.YearEq.HasValue)
            {
                return;
            }

            string s = input.Trim();

            string STOP = @"(và|va|and|của|tác\s*giả|tac\s*gia|thể\s*loại|the\s*loai|category|năm|nam|id|mã|ma|tên|ten|title)";

            var mNot = Regex.Match(s,
                $@"\b(không\s*có|khong\s*co|không\s*chứa|khong\s*chua|không\s*bao\s*gồm|khong\s*bao\s*gom)\b\s*" +
                $@"\b(chữ|chu|từ|tu|keyword|từ\s*khóa|tu\s*khoa)?\b\s*(là|=|:)?\s*(?<val>.+?)" +
                $@"(?=\s+\b{STOP}\b|\s*$)",
                RegexOptions.IgnoreCase);

            if (mNot.Success)
            {
                string kw = TextUtil.TrimEndPunc(mNot.Groups["val"].Value);
                if (!string.IsNullOrWhiteSpace(kw))
                {
                    intent.Title = kw;
                    intent.TitleNot = true;
                }
                return;
            }

            var mYes = Regex.Match(s,
                $@"\b(có|co|chứa|chua|bao\s*gồm|bao\s*gom)\b\s*" +
                $@"\b(chữ|chu|từ|tu|keyword|từ\s*khóa|tu\s*khoa)?\b\s*(là|=|:)?\s*(?<val>.+?)" +
                $@"(?=\s+\b{STOP}\b|\s*$)",
                RegexOptions.IgnoreCase);

            if (mYes.Success)
            {
                string kw = TextUtil.TrimEndPunc(mYes.Groups["val"].Value);
                if (!string.IsNullOrWhiteSpace(kw))
                {
                    intent.Title = kw;
                    intent.TitleNot = false;
                }
            }
        }
    }
}
