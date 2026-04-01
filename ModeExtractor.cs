using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class ModeExtractor
    {
        public static void Apply(string input, QueryIntent intent)
        {
            intent.WantBooks = true;

            bool hasAuthorWord = Regex.IsMatch(input, @"\b(tác\s*giả|tac\s*gia|author)\b", RegexOptions.IgnoreCase);
            bool hasBookWord = Regex.IsMatch(input, @"\b(sách|sach|book)\b", RegexOptions.IgnoreCase);

            bool askListAuthors = Regex.IsMatch(input,
                @"\b(danh\s*sách|liet\s*ke|liệt\s*kê|tất\s*cả|tat\s*ca|các|cac|list|show)\b.*\b(tác\s*giả|tac\s*gia|author)\b",
                RegexOptions.IgnoreCase);

            bool hasFilterAuthor = Regex.IsMatch(input,
                @"\b(không\s*phải|khong\s*phai|!=|<>|tên|ten|name|chứa|chua|có|co)\b",
                RegexOptions.IgnoreCase);

            if (hasAuthorWord && !hasBookWord && askListAuthors && !hasFilterAuthor)
            {
                intent.WantListAuthors = true;
                intent.WantBooks = false;
            }

            bool askListCategories = Regex.IsMatch(input,
                @"\b(danh\s*sách|liet\s*ke|liệt\s*kê|tất\s*cả|tat\s*ca|các|cac|list|show)\b.*\b(thể\s*loại|the\s*loai|category)\b",
                RegexOptions.IgnoreCase);

            bool hasFilterCategory = Regex.IsMatch(input,
                @"\b(không\s*thuộc|khong\s*thuoc|không\s*phải|khong\s*phai|!=|<>|tên|ten|chứa|chua)\b",
                RegexOptions.IgnoreCase);

            if (Regex.IsMatch(input, @"\b(thể\s*loại|the\s*loai|category)\b", RegexOptions.IgnoreCase)
                && !hasBookWord
                && askListCategories
                && !hasFilterCategory)
            {
                intent.WantListCategories = true;
                intent.WantBooks = false;
            }
        }
    }
}
