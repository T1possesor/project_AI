using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace project_AI
{
    internal static class CategoryExtractor
    {

        static readonly string CATEGORY_TERMS =
            @"thể\s*loại|the\s*loai|" +
            @"loại|loai|" +
            @"dạng|dang|" +
            @"kiểu|kieu|" +
            @"mục|muc|" +
            @"chủ\s*đề|chu\s*de|" +
            @"đề\s*tài|de\s*tai|" +
            @"lĩnh\s*vực|linh\s*vuc|" +
            @"mảng|mang|" +
            @"nhóm|nhom|" +
            @"phân\s*loại|phan\s*loai|" +
            @"chuyên\s*mục|chuyen\s*muc|" +
            @"thể\s*tài|the\s*tai|" +
            @"sách\s*về|" +
            @"thuộc\s*loại|" +
            @"thuộc\s*dạng|" +
            @"thuộc\s*kiểu|" +
            @"dòng\s*sách|" +
            @"category|genre|type|kind|topic|subject|field";

        static readonly string CATEGORY_SIGNALS =
            CATEGORY_TERMS + @"|sách\s+về|thuộc\s+loại|thuộc\s+dạng";

        public static void Apply(string input, QueryIntent intent)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            
            string s = input.Trim();

            if (Regex.IsMatch(s,
                @"^\s*(năm|nam|phát\s*hành|phat\s*hanh|ra\s*mắt|ra\s*mat|xuất\s*bản|xuat\s*ban|publish(?:ed)?|year)\b",
                RegexOptions.IgnoreCase))
            {
                return;
            }

            if (!Regex.IsMatch(s, CATEGORY_SIGNALS, RegexOptions.IgnoreCase))
                return;

            string stop =
@"(?=\s+\b(và|and|,|hay|hoặc|or|;|của|tác\s*giả|tac\s*gia|author|writer|written\s+by|viết\s+bởi" +
@"|vào|vao" +
@"|năm|nam|phát\s*hành|phat\s*hanh|ra\s*mắt|ra\s*mat|xuất\s*bản|xuat\s*ban|publish(?:ed)?|year" +
@"|id|mã|ma|stt|tên\s*sách|ten\s*sach|tên|ten|tiêu\s*đề|tieu\s*de|title)\b|$)";

            var mNotJoin = Regex.Match(s,
                $@"\b(không\s*thuộc|không\s*phải|không\s*có)\b.*?\b({CATEGORY_TERMS})\b\s*(?<v>.+?){stop}\s*\b(và|and|,|hay|hoặc|or)\b\s*(?<v2>.+?){stop}",
                RegexOptions.IgnoreCase);

            if (mNotJoin.Success)
            {
                intent.Category = null;

                AppendCategory(intent, mNotJoin.Groups["v"].Value);
                AppendCategory(intent, mNotJoin.Groups["v2"].Value);

                string joinWord = mNotJoin.Groups[3].Value.ToLowerInvariant();
                intent.CategoryJoiner =
                    (joinWord == "và" || joinWord == "and" || joinWord == ",")
                        ? "AND"
                        : "OR";

                intent.CategoryNot = true;
                return;
            }

            var mNot = Regex.Match(s,
                $@"\b(không\s*thuộc|không\s*phải|không\s*có)\b.*?\b({CATEGORY_TERMS})\b\s*(?<v>.+?){stop}",
                RegexOptions.IgnoreCase);

            if (mNot.Success)
            {
                AppendCategory(intent, mNot.Groups["v"].Value);
                intent.CategoryNot = true;
                intent.CategoryJoiner ??= "AND";
                return;
            }

            var mJoin = Regex.Match(s,
                $@"\b({CATEGORY_TERMS}|sách)\b.*?\b(là|thuộc|=|:)?\b\s*(?<v>.+?){stop}\s*\b(và|and|,|hay|hoặc|or)\b\s*(?<v2>.+?){stop}",
                RegexOptions.IgnoreCase);

            if (mJoin.Success)
            {
                intent.Category = null;

                AppendCategory(intent, mJoin.Groups["v"].Value);
                AppendCategory(intent, mJoin.Groups["v2"].Value);

                string joinWord = mJoin.Groups[4].Value.ToLowerInvariant();
                intent.CategoryJoiner =
                    (joinWord == "và" || joinWord == "and" || joinWord == ",")
                        ? "AND"
                        : "OR";

                intent.CategoryNot = false;
                return;
            }

            var mContains = Regex.Match(s,
                $@"\b({CATEGORY_TERMS}|sách)\b.*?\b(là|thuộc|về|=|:)?\b\s*(?<v>.+?){stop}",
                RegexOptions.IgnoreCase);

            if (mContains.Success)
            {
                AppendCategory(intent, mContains.Groups["v"].Value);
                intent.CategoryNot = false;
                intent.CategoryJoiner ??= "AND";
            }
        }

        static void AppendCategory(QueryIntent intent, string raw)
        {
            string v = Clean(raw);
            if (string.IsNullOrWhiteSpace(v)) return;

            if (string.IsNullOrWhiteSpace(intent.Category))
                intent.Category = v;
            else
                intent.Category = intent.Category + "|" + v;

            intent.CategoryJoiner ??= "AND";
        }

        static string Clean(string v)
        {
            v = Regex.Replace(v,
                @"^\s*(thuộc|về|là|=|:)\s*",
                "",
                RegexOptions.IgnoreCase);

            v = Regex.Replace(v,
                $@"^\s*({CATEGORY_TERMS})\s*",
                "",
                RegexOptions.IgnoreCase);

            return v.Trim().Trim('.', ',', ';', ':');
        }
    }
}