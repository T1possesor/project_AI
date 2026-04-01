using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace project_AI
{
    internal static class TitleExtractor
    {

        static readonly string TITLE_TERMS =
            @"tên\s*sách|ten\s*sach|" +
            @"tên|ten|" +
            @"tiêu\s*đề|tieu\s*de|" +
            @"nhan\s*đề|nhan\s*de|" +
            @"tựa\s*đề|tua\s*de|" +
            @"tựa\s*sách|tua\s*sach|" +

            @"đầu\s*sách|dau\s*sach|" +
            @"đầu\s*đề|dau\s*de|" +
            @"tựa|tua|" +
            @"tít|tit|" +
            @"tít\s*sách|tit\s*sach|" +
            @"tựa\s*cuốn|tua\s*cuon|" +
            @"tên\s*cuốn|ten\s*cuon|" +
            @"tên\s*tác\s*phẩm|ten\s*tac\s*pham|" +
            @"tên\s*đầu\s*sách|ten\s*dau\s*sach|" +
            @"tên\s*đầu\s*đề|ten\s*dau\s*de|" +
            @"đầu\s*mục|dau\s*muc|" +
            @"mục\s*tên|muc\s*ten|" +
            @"tên\s*đề\s*mục|ten\s*de\s*muc|" +
            @"đề\s*mục|de\s*muc|" +
            @"heading\s*của\s*sách|heading\s*cua\s*sach|" +

            @"title|book\s*title|" +
            @"heading|headline|" +
            @"name|book\s*name|" +
            @"caption|subject";

        public static void Apply(string input, QueryIntent intent)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            if (Regex.IsMatch(input.Trim(),
                @"^(id|mã|ma|stt)\b",
                RegexOptions.IgnoreCase))
            {
                return;
            }

            if (Regex.IsMatch(input,
                @"^\s*(năm|nam|ra\s*mắt|phát\s*hành|xuất\s*bản|publish|published|year)\b",
                RegexOptions.IgnoreCase))
            {
                return;
            }

            if (Regex.IsMatch(input,
                @"^\s*(thuộc\s+)?(thể\s*loại|the\s*loai|category|genre|type|kind|topic)\b",
                RegexOptions.IgnoreCase))
            {
                return;
            }

            if (Regex.IsMatch(input,
                @"^\s*(của\s+)?(tác\s*giả|tac\s*gia|author|writer|written\s+by|viết\s+bởi|do\s+.+?\s+viết)\b",
                RegexOptions.IgnoreCase))
            {
                return;
            }

           

            string s = input;

            bool hasAuthorSignal = Regex.IsMatch(input,
                @"\b(tác\s*giả|tac\s*gia|author|writer|written\s+by|viết\s+bởi|do\s+.+?\s+viết)\b",
                RegexOptions.IgnoreCase);

            bool hasTitleSignal = Regex.IsMatch(input,
    $@"\b({TITLE_TERMS}|tên\s*sách|ten\s*sach|tên|ten|tiêu\s*đề|tieu\s*de|nhan\s*đề|nhan\s*de|tựa\s*đề|tua\s*de|title)\b",
    RegexOptions.IgnoreCase);

            if (hasAuthorSignal && !hasTitleSignal)
                return;

            bool hasIdSignal = Regex.IsMatch(input,
                @"\b(id|mã|ma|stt)\b",
                RegexOptions.IgnoreCase);

            if (hasIdSignal && !hasTitleSignal)
                return;

            bool hasCategorySignal = Regex.IsMatch(input,
    @"\b(" +
    @"thể\s*loại|the\s*loai|" +
    @"thuộc\s*loại|thuoc\s*loai|" +
    @"loại|loai|" +
    @"dòng\s*sách|dong\s*sach|" +
    @"sách\s*về|sach\s*ve|" +
    @"category|genre|type|kind|topic|subject|field|" +
    @"chủ\s*đề|chu\s*de|đề\s*tài|de\s*tai|lĩnh\s*vực|linh\s*vuc" +
    @")\b",
    RegexOptions.IgnoreCase);

            if (hasCategorySignal && !hasTitleSignal)
                return;

            bool hasYearSignal = Regex.IsMatch(input,
                @"\b(năm|nam|phát\s*hành|phat\s*hanh|ra\s*mắt|ra\s*mat|xuất\s*bản|xuat\s*ban|publish(?:ed)?|year)\b",
                RegexOptions.IgnoreCase);

            if (hasYearSignal && !hasTitleSignal)
                return;

            var mEqJoin = Regex.Match(s,
                $@"\b({TITLE_TERMS})\b\s*(là|=|:)\s*" +
                @"(?<v>.+?)\s*\b(và|and|,|hay|hoặc|or)\b\s*(?<v2>.+)$",
                RegexOptions.IgnoreCase);

            if (mEqJoin.Success)
            {
                string joinWord = mEqJoin.Groups[3].Value.ToLowerInvariant();
                intent.TitleJoiner =
                    (joinWord == "và" || joinWord == "and" || joinWord == ",")
                        ? "AND"
                        : "OR";

                intent.TitleValues = null; 
                AddMulti(intent,
                    new[] { mEqJoin.Groups["v"].Value, mEqJoin.Groups["v2"].Value },
                    TitleMatchMode.Like);

                return;
            }




            var mTitleInside = Regex.Match(s,
    $@"\b({TITLE_TERMS})\b\s*(là|=|:)?\s*(?<v>.+?)(?=\s+\b(và|and|của|tác\s*giả|tac\s*gia|thể\s*loại|the\s*loai|năm|nam|id)\b|$)",
    RegexOptions.IgnoreCase);

            if (mTitleInside.Success)
            {
                AddSingle(intent, mTitleInside.Groups["v"].Value, TitleMatchMode.Like);
                return;
            }

            var mNot = Regex.Match(s,
                $@"\b(không\s*có|không\s*chứa|không\s*phải)\b.*?\b({TITLE_TERMS}|sách|sach|book)?\b\s*(?<v>.+)$",
                RegexOptions.IgnoreCase);

            if (mNot.Success)
            {
                AddSingle(intent, mNot.Groups["v"].Value, TitleMatchMode.Like, true);
                return;
            }

            var mExact = Regex.Match(s,
                $@"\b(chính\s*xác)\b.*?\b({TITLE_TERMS})?\b\s*(là|=|:)?\s*(?<v>.+)$",
                RegexOptions.IgnoreCase);

            if (mExact.Success)
            {
                AddSingle(intent, mExact.Groups["v"].Value, TitleMatchMode.Exact);
                return;
            }

            var mStart = Regex.Match(s,
                @"\b(bắt\s*đầu\s*bằng|bat\s*dau\s*bang)\b\s*(?<v>.+)$",
                RegexOptions.IgnoreCase);

            if (mStart.Success)
            {
                AddSingle(intent, mStart.Groups["v"].Value, TitleMatchMode.StartsWith);
                return;
            }

            var mEnd = Regex.Match(s,
                @"\b(kết\s*thúc\s*bằng|ket\s*thuc\s*bang)\b\s*(?<v>.+)$",
                RegexOptions.IgnoreCase);

            if (mEnd.Success)
            {
                AddSingle(intent, mEnd.Groups["v"].Value, TitleMatchMode.EndsWith);
                return;
            }

            string JOIN_TOKEN = @"(?:,|\b(?:và|and|hay|hoặc|or)\b)";

            var mContainsJoin = Regex.Match(s,
                $@"\b({TITLE_TERMS}|sách|sach|book)\b.*?\b(" +
                @"có\s*chữ|có\s*từ|có\s*ký\s*tự|có\s*kí\s*tự|" +
                @"chứa|bao\s*gồm|gồm|contain|contains|" +
                @"keyword|từ\s*khóa|tu\s*khoa|" +
                @"cụm\s*từ|cum\s*tu|phrase|" +
                @"character|ký\s*tự|kí\s*tự|chữ\s*cái|chu\s*cai" +
                $@")\b\s*(?<v>.+?)\s*{JOIN_TOKEN}\s*(?<v2>.+)$",
                RegexOptions.IgnoreCase);

            if (mContainsJoin.Success)
            {

                bool isComma = Regex.IsMatch(s, @",");
                intent.TitleJoiner = isComma ? "AND"
                    : (Regex.IsMatch(s, @"\b(và|and)\b", RegexOptions.IgnoreCase) ? "AND" : "OR");

                intent.TitleValues = null;

                AddMulti(intent,
                    new[] { mContainsJoin.Groups["v"].Value, mContainsJoin.Groups["v2"].Value },
                    TitleMatchMode.Like);

                return;
            }

            var mContains = Regex.Match(s,
                $@"\b({TITLE_TERMS}|sách|sach|book)\b.*?\b(" +
                @"có\s*chữ|có\s*từ|có\s*ký\s*tự|có\s*kí\s*tự|" +
                @"chứa|bao\s*gồm|gồm|contain|contains|" +
                @"keyword|từ\s*khóa|tu\s*khoa|" +
                @"cụm\s*từ|cum\s*tu|phrase|" +
                @"character|ký\s*tự|kí\s*tự|chữ\s*cái|chu\s*cai" +
                @")\b\s*(?<v>.+)$",
                RegexOptions.IgnoreCase);

            if (mContains.Success)
            {
                AddSingle(intent, mContains.Groups["v"].Value, TitleMatchMode.Like);
                return;
            }

            var mJoin = Regex.Match(s,
                $@"\b({TITLE_TERMS}|sách|sach|book)\b.*?\b(là|=|:)\b\s*" +
                @"(?<v>.+?)\s*\b(và|and|hay|hoặc|or)\b\s*(?<v2>.+)$",
                RegexOptions.IgnoreCase);

            if (mJoin.Success)
            {
                string joinWord = mJoin.Groups[3].Value.ToLowerInvariant();
                intent.TitleJoiner =
                    (joinWord.Contains("và") || joinWord.Contains("and"))
                        ? "AND"
                        : "OR";

                intent.TitleValues = null;

                AddMulti(intent,
                    new[] { mJoin.Groups["v"].Value, mJoin.Groups["v2"].Value },
                    TitleMatchMode.Like);

                return;
            }

            if (hasTitleSignal)
            {
                var mLike = Regex.Match(s,
                    $@"\b({TITLE_TERMS})\b.*?\b(là|=|:)?\s*(?<v>.+)$",
                    RegexOptions.IgnoreCase);

                if (mLike.Success)
                    AddSingle(intent, mLike.Groups["v"].Value, TitleMatchMode.Like);
            }
        }

        static void AddSingle(QueryIntent intent, string v, TitleMatchMode mode, bool not = false)
        {
            string clean = Clean(v);
            if (string.IsNullOrWhiteSpace(clean)) return;

            if (intent.TitleValues == null)
                intent.TitleValues = new List<string>();

            intent.TitleValues.Add(clean);

            if (!intent.TitleMode.HasValue)
                intent.TitleMode = mode;
            else if (intent.TitleMode.Value != mode)
                intent.TitleMode = TitleMatchMode.Like;

            intent.TitleNot = not;
            intent.TitleJoiner ??= "AND";
        }

        static void AddMulti(QueryIntent intent, IEnumerable<string> vals, TitleMatchMode mode)
        {
            if (intent.TitleValues == null)
                intent.TitleValues = new List<string>();

            foreach (var raw in vals)
            {
                string clean = Clean(raw);
                if (!string.IsNullOrWhiteSpace(clean))
                    intent.TitleValues.Add(clean);
            }

            if (!intent.TitleMode.HasValue)
                intent.TitleMode = mode;
            else if (intent.TitleMode.Value != mode)
                intent.TitleMode = TitleMatchMode.Like;

            intent.TitleNot = false;
            intent.TitleJoiner ??= "AND";
        }

        static string Clean(string v)
        {

            v = Regex.Replace(v,
    @"^\s*(có\s*)?(chứa\s*)?(bao\s*gồm\s*)?(gồm\s*)?(contain(s)?\s*)?(keyword\s*)?" +
    @"(chữ|chu|từ|tu|ký\s*tự|kí\s*tự|character|từ\s*khóa|tu\s*khoa|cụm\s*từ|cum\s*tu|phrase)\s*",
    "",
    RegexOptions.IgnoreCase);


            v = Regex.Replace(v,
                @"^\s*(chữ|từ)\s*",
                "",
                RegexOptions.IgnoreCase);

            v = Regex.Replace(v,
                @"^\s*(chữ|từ|ký\s*tự|kí\s*tự|character|keyword|từ\s*khóa|tu\s*khoa|cụm\s*từ|cum\s*tu|phrase)\s*",
                "",
                RegexOptions.IgnoreCase);

            v = Regex.Replace(v,
                @"^\s*(có\s*)?(mang\s*)?(được\s*)?(đặt\s*)?(tên|tiêu\s*đề|nhan\s*đề|tựa\s*đề)\s*(là|=|:)?\s*",
                "",
                RegexOptions.IgnoreCase);

            v = Regex.Replace(v,
                $@"^\s*({TITLE_TERMS}|sách|sach|book)\s*",
                "",
                RegexOptions.IgnoreCase);

            v = Regex.Replace(v,
                @"^\s*(là|=|:)\s*",
                "",
                RegexOptions.IgnoreCase);

            return v.Trim().Trim('.', ',', ';', ':');
        }
    }
}