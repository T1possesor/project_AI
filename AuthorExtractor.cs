using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace project_AI
{
    internal static class AuthorExtractor
    {

        static readonly string AUTHOR_TERMS =
            @"tác\s*giả|tac\s*gia|" +
            @"người\s*viết|nguoi\s*viet|" +
            @"người\s*sáng\s*tác|nguoi\s*sang\s*tac|" +
            @"nhà\s*văn|nha\s*van|" +
            @"soạn\s*giả|soan\s*gia|" +
            @"biên\s*soạn|bien\s*soan|" +
            @"chấp\s*bút|chap\s*but|" +
            @"author|writer";


        static readonly string AUTHOR_SIGNALS =
            AUTHOR_TERMS +
            @"|do\s+.+?\s+viết|" +
            @"viết\s+bởi|" +
            @"written\s+by|" +
            @"\bby\s+[A-ZÀ-Ỹ]";

        public static void Apply(string input, QueryIntent intent)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            if (!Regex.IsMatch(input, AUTHOR_SIGNALS, RegexOptions.IgnoreCase))
                return;

            if (!string.IsNullOrWhiteSpace(intent.Author))
                return;

            string s = input;

            var mNotJoin = Regex.Match(s,
                $@"\b(không\s*phải|không\s*có|không\s*do)\b.*?\b({AUTHOR_TERMS})?\b\s*" +
                @"(?<v>.+?)\s*\b(và|and|,|hay|hoặc|or)\b\s*(?<v2>.+)$",
                RegexOptions.IgnoreCase);

            if (mNotJoin.Success)
            {
                intent.Author =
                    Clean(mNotJoin.Groups["v"].Value) + "|" +
                    Clean(mNotJoin.Groups["v2"].Value);

                string joinWord = mNotJoin.Groups[3].Value;
                intent.AuthorJoiner = IsAnd(joinWord) ? "AND" : "OR";

                intent.AuthorNot = true;
                return;
            }

            var mNot = Regex.Match(s,
                $@"\b(không\s*phải|không\s*có|không\s*do)\b.*?\b({AUTHOR_TERMS})?\b\s*(?<v>.+)$",
                RegexOptions.IgnoreCase);

            if (mNot.Success)
            {
                intent.Author = Clean(mNot.Groups["v"].Value);
                intent.AuthorNot = true;
                intent.AuthorJoiner = "AND";
                return;
            }

            var mJoin = Regex.Match(s,
                $@"\b({AUTHOR_TERMS}|tác\s*phẩm|sách)\b.*?\b(là|có\s*tên\s*là|=|:)?\b\s*" +
                @"(?<v>.+?)\s*\b(và|and|,|hay|hoặc|or)\b\s*(?<v2>.+)$",
                RegexOptions.IgnoreCase);

            if (mJoin.Success)
            {
                string v1 = Clean(mJoin.Groups["v"].Value);
                string v2raw = mJoin.Groups["v2"].Value;

                if (Regex.IsMatch(v2raw, @"\b(tên|ten|title|tiêu\s*đề|tieu\s*de)\b", RegexOptions.IgnoreCase))
                {
                    intent.Author = v1;
                    intent.AuthorJoiner = "AND";
                    intent.AuthorNot = false;
                    return;
                }

                string v2 = Clean(v2raw);

                intent.Author = $"{v1}|{v2}";

                string joinWord = mJoin.Groups[4].Value;
                intent.AuthorJoiner = IsAnd(joinWord) ? "AND" : "OR";

                intent.AuthorNot = false;
                return;
            }

            var mDoViet = Regex.Match(s,
                @"\bdo\s+(?<v>.+?)\s+viết\b",
                RegexOptions.IgnoreCase);

            if (mDoViet.Success)
            {
                intent.Author = Clean(mDoViet.Groups["v"].Value);
                intent.AuthorJoiner = "AND";
                return;
            }

            var mBy = Regex.Match(s,
                @"\b(viết\s+bởi|written\s+by|by)\s+(?<v>.+)$",
                RegexOptions.IgnoreCase);

            if (mBy.Success)
            {
                intent.Author = Clean(mBy.Groups["v"].Value);
                intent.AuthorJoiner = "AND";
                return;
            }

            var mDefault = Regex.Match(s,
                $@"\b({AUTHOR_TERMS})\b\s*(là|có\s*tên\s*là|=|:)?\s*(?<v>.+)$",
                RegexOptions.IgnoreCase);

            if (mDefault.Success)
            {
                intent.Author = Clean(mDefault.Groups["v"].Value);
                intent.AuthorJoiner = "AND";
            }
        }

        static string Clean(string v)
        {

            v = Regex.Replace(v,
                @"\b(ông|bà|anh|chị|cô|chú|bác|thầy|cô\s*giáo|nhà\s*văn|tác\s*giả)\b",
                "",
                RegexOptions.IgnoreCase);

            v = Regex.Replace(v,
                @"\b(có\s*tên\s*là|tên\s*là|viết|bởi|written|by)\b",
                "",
                RegexOptions.IgnoreCase);

            v = Regex.Replace(v,
                $@"^\s*({AUTHOR_TERMS}|là|do|của)\s*",
                "",
                RegexOptions.IgnoreCase);

            return v.Trim().Trim('.', ',', ';', ':');
        }

        static bool IsAnd(string s)
        {
            s = s.ToLowerInvariant();
            return s == "và" || s == "and" || s == ",";
        }
    }
}
