using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class ClauseSplitter
    {
        public static List<string> Split(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            var s = Regex.Replace(input.Trim(), @"\s+", " ");

            string fieldHead =
    @"(của\b|" +
    @"tác\s*giả\b|tac\s*gia\b|author\b|writer\b|written\s+by\b|viết\s+bởi\b|" +
    @"(?:thuộc\s+)?thể\s*loại\b|(?:thuộc\s+)?the\s*loai\b|category\b|genre\b|sách\s+về\b|" +
    @"tên\s*sách\b|ten\s*sach\b|tên\b|ten\b|tiêu\s*đề\b|tieu\s*de\b|title\b|" +
    @"năm\b|nam\b|publish(?:ed)?\b|year\b|phát\s*hành\b|phat\s*hanh\b|ra\s*mắt\b|ra\s*mat\b|xuất\s*bản\b|xuat\s*ban\b|" +
    @"id\b|mã\b|ma\b|stt\b)";

            string splitPattern =
    $@"\s*(?:,|;|\brồi\b|\bsau đó\b|\btiếp theo\b|\b(?:và|and)\b)\s*(?=\s*(?:không\s*(?:phải|có|thuộc|do)?\s*)?{fieldHead})";

            var parts = Regex.Split(s, splitPattern, RegexOptions.IgnoreCase);

            var res = new List<string>();
            foreach (var p in parts)
            {
                var t = p.Trim();
                if (!string.IsNullOrWhiteSpace(t))
                    res.Add(t);
            }
            return res;
        }
    }
}