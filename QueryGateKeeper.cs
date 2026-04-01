using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class QueryGateKeeper
    {
        static readonly string[] ACTION_PREFIX =
        {
            @"chọn",
            @"cho\s+(tôi|toi|mình|minh)\b",
            @"(xem|coi|nhìn|nhin|xem\s*thử|coi\s*thử)",
            @"(tìm|tim|tìm\s*kiếm|tim\s*kiem|tra|tra\s*cứu|tra\s*cuu|search|lookup)",
            @"(liệt\s*kê|liet\s*ke|kể|ke|thống\s*kê|thong\s*ke)",
            @"(danh\s*sách|danh\s*sach|list|show|hiển\s*thị|hien\s*thi|in\s*ra|print)",
            @"(lấy|lay|get|fetch|pull)",
            @"(có\s+những|co\s+nhung)",
            @"(bao\s+nhiêu|bao\s+nhieu)",
            @"(ai\s+viết|ai\s+viet)",
            @"(gồm\s+những|gom\s+nhung)",
            @"(những\s+gì|nhung\s+gi)",
            @"(là\s+gì|la\s+gi)",
            @"(select|query|where|filter)"
        };

        static readonly string[] DIRECT_FIELD_PREFIX =
        {
            @"sách\b|sach\b|book\b",
            @"tác\s*giả\b|tac\s*gia\b|author\b|writer\b",
            @"thể\s*loại\b|the\s*loai\b|category\b|genre\b",
            @"tên\b|ten\b|title\b|tiêu\s*đề\b|tieu\s*de\b",
            @"năm\b|nam\b|year\b|publish(?:ed)?\b|phát\s*hành\b|phat\s*hanh\b|xuất\s*bản\b|xuat\s*ban\b",
            @"id\b|mã\b|ma\b|stt\b"
        };

        public static bool IsAllowed(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string s = input.Trim().ToLowerInvariant();

            foreach (var pattern in ACTION_PREFIX)
            {
                if (Regex.IsMatch(s, @"^\s*" + pattern, RegexOptions.IgnoreCase))
                    return true;
            }

            foreach (var pattern in DIRECT_FIELD_PREFIX)
            {
                if (Regex.IsMatch(s, @"^\s*" + pattern, RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }
    }
}