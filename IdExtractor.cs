using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class IdExtractor
    {
        public static void Apply(string input, QueryIntent intent)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            string s = NormalizeIdTerms(input.ToLowerInvariant());

            if (intent.Id.HasValue || intent.IdFrom.HasValue ||
                intent.IdGreaterThan.HasValue || intent.IdLessThan.HasValue)
                return;

            var mNot = Regex.Match(s,
                @"\bid\b\s*(không\s*phải|!=|<>)\s*(?<v>\d+)",
                RegexOptions.IgnoreCase);

            if (mNot.Success)
            {
                intent.Id = int.Parse(mNot.Groups["v"].Value);
                intent.IdNot = true;
                return;
            }

            var mGt = Regex.Match(s,
                @"\bid\b.*?(>|lớn\s*hơn|lon\s*hon|trên|tren)\s*(?<v>\d+)",
                RegexOptions.IgnoreCase);

            if (mGt.Success)
            {
                intent.IdGreaterThan = int.Parse(mGt.Groups["v"].Value);
                return;
            }

            var mLt = Regex.Match(s,
                @"\bid\b.*?(<|nhỏ\s*hơn|nho\s*hon|bé\s*hơn|be\s*hon|dưới|duoi)\s*(?<v>\d+)",
                RegexOptions.IgnoreCase);

            if (mLt.Success)
            {
                intent.IdLessThan = int.Parse(mLt.Groups["v"].Value);
                return;
            }

            var mRange = Regex.Match(s,
                @"\bid\b.*?(từ|trong\s*khoảng|between)?\s*(?<a>\d+)\s*(đến|tới|-|to|and|và|va)\s*(?<b>\d+)",
                RegexOptions.IgnoreCase);

            if (mRange.Success)
            {
                intent.IdFrom = int.Parse(mRange.Groups["a"].Value);
                intent.IdTo = int.Parse(mRange.Groups["b"].Value);
                return;
            }

            var mEq = Regex.Match(s,
                @"\bid\b\s*(là|=|:)?\s*(?<v>\d+)",
                RegexOptions.IgnoreCase);

            if (mEq.Success)
            {
                intent.Id = int.Parse(mEq.Groups["v"].Value);
                intent.IdNot = false;
                return;
            }

            if (!intent.Id.HasValue && Regex.IsMatch(s, @"\bid\b") && !Regex.IsMatch(s, @"\d+"))
            {
                intent.WantListIds = true;
            }
        }

        static string NormalizeIdTerms(string s)
        {
            string[] idTerms =
            {
                "mã định danh", "ma dinh danh",
                "định danh", "dinh danh",
                "mã số", "ma so",
                "mã", "ma",
                "stt",
                "số thứ tự", "so thu tu"
            };

            foreach (var t in idTerms)
            {
                s = Regex.Replace(
                    s,
                    $@"(?<!\p{{L}}){t.Replace(" ", @"\s*")}(?!\p{{L}})",
                    "id",
                    RegexOptions.IgnoreCase
                );
            }

            return s;
        }
    }
}