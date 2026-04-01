using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace project_AI
{
    public partial class Form1 : Form
    {
        private enum ViewMode { Books, Authors, Categories }
        private ViewMode _mode = ViewMode.Books;

        private bool _waitingConfirm = false;
        private string _pendingSuggestedAuthor = "";

        public Form1()
        {
            InitializeComponent();
            this.AcceptButton = btnSend;
            txtQuery.KeyDown += txtQuery_KeyDown;


            try
            {
                LoadSummaryInfo();
                LoadBooks(); // default

                AIReply(
                    "Xin chào!",
                    "Chế độ xem: SÁCH (đang hiển thị toàn bộ).",
                    "Bạn có thể:",
                    "- Bấm các nút SÁCH / TÁC GIẢ / THỂ LOẠI",
                    "- Nếu ở TÁC GIẢ: chọn 1 tác giả rồi bấm 'Xem sách của tác giả đã chọn'",
                    "- Nếu ở THỂ LOẠI: chọn 1 thể loại rồi bấm 'Xem sách theo thể loại đã chọn'",
                    "- Chat: 'sách của Nguyễn Nhật Ánh' hoặc 'tác giả Nguyễn Nhật Ánh' hoặc 'thể loại Kinh tế'"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "LỖI DATABASE");
            }
        }

        private void AIReply(params string[] lines)
        {
            rtbChat.AppendText("AI:\n");
            foreach (var l in lines)
            {
                if (!string.IsNullOrWhiteSpace(l))
                    rtbChat.AppendText("- " + l + "\n");
            }
            rtbChat.AppendText("\n");
        }

        private void LoadSummaryInfo()
        {
            var authors = DatabaseHelper.GetAllAuthors();
            var cats = DatabaseHelper.GetAllCategories();
            var (minY, maxY) = DatabaseHelper.GetYearRange();

            lblInfo.Text = $"Tác giả: {authors.Count} | Thể loại: {cats.Count} | Năm: {minY}-{maxY}";
        }

        private void LoadBooks(string? authorExact = null, string? categoryLike = null)
        {
            _mode = ViewMode.Books;

            string sql = "SELECT * FROM Books WHERE 1=1";
            var ps = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(authorExact))
            {
                sql += " AND Author = @author";
                ps.Add(new SqlParameter("@author", authorExact));
            }

            if (!string.IsNullOrWhiteSpace(categoryLike))
            {
                sql += " AND Category LIKE @cat";
                ps.Add(new SqlParameter("@cat", $"%{categoryLike}%"));
            }

            sql += " ORDER BY PublishYear DESC, Title";

            dgvMain.DataSource = DatabaseHelper.ExecuteTable(sql, ps.ToArray());
            lblMode.Text = "CHẾ ĐỘ: SÁCH";
        }

        private void LoadAuthors(string? authorLike = null)
        {
            _mode = ViewMode.Authors;

            string sql = "SELECT DISTINCT Author FROM Books";
            var ps = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(authorLike))
            {
                sql += " WHERE Author LIKE @k";
                ps.Add(new SqlParameter("@k", $"%{authorLike}%"));
            }

            sql += " ORDER BY Author";

            dgvMain.DataSource = DatabaseHelper.ExecuteTable(sql, ps.ToArray());
            lblMode.Text = "CHẾ ĐỘ: TÁC GIẢ";
        }

        private void LoadCategories(string? categoryLike = null)
        {
            _mode = ViewMode.Categories;

            string sql = "SELECT DISTINCT Category FROM Books";
            var ps = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(categoryLike))
            {
                sql += " WHERE Category LIKE @k";
                ps.Add(new SqlParameter("@k", $"%{categoryLike}%"));
            }

            sql += " ORDER BY Category";

            dgvMain.DataSource = DatabaseHelper.ExecuteTable(sql, ps.ToArray());
            lblMode.Text = "CHẾ ĐỘ: THỂ LOẠI";
        }

        private void LoadBooksOfSelectedAuthor()
        {
            if (_mode != ViewMode.Authors)
            {
                AIReply("Bạn phải ở chế độ TÁC GIẢ trước (bấm nút 'Tác giả').");
                return;
            }

            if (dgvMain.CurrentRow == null)
            {
                AIReply("Bạn chưa chọn dòng tác giả nào.");
                return;
            }

            string author = dgvMain.CurrentRow.Cells["Author"].Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(author))
            {
                AIReply("Không đọc được tên tác giả từ dòng đang chọn.");
                return;
            }

            LoadBooks(authorExact: author);
            AIReply($"Đang xem sách của tác giả: {author}");
        }

        private void LoadBooksOfSelectedCategory()
        {
            if (_mode != ViewMode.Categories)
            {
                AIReply("Bạn phải ở chế độ THỂ LOẠI trước (bấm nút 'Thể loại').");
                return;
            }

            if (dgvMain.CurrentRow == null)
            {
                AIReply("Bạn chưa chọn dòng thể loại nào.");
                return;
            }

            string cat = dgvMain.CurrentRow.Cells["Category"].Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(cat))
            {
                AIReply("Không đọc được tên thể loại từ dòng đang chọn.");
                return;
            }

            LoadBooks(categoryLike: cat);
            AIReply($"Đang xem sách thuộc thể loại: {cat}");
        }


        private void btnModeBooks_Click(object sender, EventArgs e)
        {
            LoadBooks();
            AIReply("Chuyển chế độ: SÁCH", "Đang hiển thị toàn bộ sách.");
        }

        private void btnModeAuthors_Click(object sender, EventArgs e)
        {
            LoadAuthors();
            AIReply("Chuyển chế độ: TÁC GIẢ", "Bảng chỉ hiển thị TÊN TÁC GIẢ (không có BookCount).");
        }

        private void btnModeCategories_Click(object sender, EventArgs e)
        {
            LoadCategories();
            AIReply("Chuyển chế độ: THỂ LOẠI", "Bảng chỉ hiển thị TÊN THỂ LOẠI (không có BookCount).");
        }

        private void btnShowBooksBySelectedAuthor_Click(object sender, EventArgs e)
        {
            LoadBooksOfSelectedAuthor();
        }

        private void btnShowBooksBySelectedCategory_Click(object sender, EventArgs e)
        {
            LoadBooksOfSelectedCategory();
        }

        private void dgvMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (_mode == ViewMode.Authors)
                LoadBooksOfSelectedAuthor();
            else if (_mode == ViewMode.Categories)
                LoadBooksOfSelectedCategory();
        }

        private void txtQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSend.PerformClick();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string input = txtQuery.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            rtbChat.AppendText("Bạn: " + input + "\n");

            string aiResult = PatternAI.GenerateSql(input);

            if (aiResult == "__REJECT__")
            {
                AIReply(
                    "Mình chỉ hỗ trợ TÌM / XEM dữ liệu.",
                    "Ví dụ bạn có thể nhập:",
                    "• xem sách",
                    "• liệt kê tác giả",
                    "• sách năm 2020"
                );
                txtQuery.Clear();
                return;
            }

            if (aiResult.StartsWith("__SELECT_AUTHOR__:"))
            {
                string authorName = aiResult.Substring("__SELECT_AUTHOR__:".Length).Trim();

                string outputSql = $"SELECT Author WHERE Author LIKE N'%{authorName}%'";
                string realSql = $"SELECT DISTINCT Author FROM Books WHERE Author LIKE N'%{authorName.Replace("'", "''")}%'";
                DataTable dt = DatabaseHelper.ExecuteTable(realSql);

                dgvMain.DataSource = dt;
                lblMode.Text = "CHẾ ĐỘ: TÁC GIẢ (từ Chat)";

                AIReply(outputSql, $"Tìm thấy {dt.Rows.Count} dòng.");
                txtQuery.Clear();
                return;
            }
            if (aiResult.StartsWith("__SELECT_ID__:"))
            {
                string id = aiResult.Substring("__SELECT_ID__:".Length).Trim();

                string outputSql = $"SELECT * FROM Books WHERE Id = {id}";
                string realSql = $"SELECT * FROM Books WHERE Id = {id}";

                DataTable dt = DatabaseHelper.ExecuteTable(realSql);
                dgvMain.DataSource = dt;
                lblMode.Text = "CHẾ ĐỘ: SÁCH (lọc theo ID từ Chat)";

                AIReply(outputSql, $"Tìm thấy {dt.Rows.Count} dòng.");
                txtQuery.Clear();
                return;
            }


            if (aiResult.StartsWith("__SELECT_CATEGORY__:"))
            {
                string catName = aiResult.Substring("__SELECT_CATEGORY__:".Length).Trim();

                string outputSql = $"SELECT Category WHERE Category LIKE N'%{catName}%'";
                string realSql = $"SELECT DISTINCT Category FROM Books WHERE Category LIKE N'%{catName.Replace("'", "''")}%'";

                DataTable dt = DatabaseHelper.ExecuteTable(realSql);
                dgvMain.DataSource = dt;
                lblMode.Text = "CHẾ ĐỘ: THỂ LOẠI (từ Chat)";

                AIReply(outputSql, $"Tìm thấy {dt.Rows.Count} dòng.");
                txtQuery.Clear();
                return;
            }











            // ✅ MODE
            if (aiResult == "__MODE_AUTHORS__") { LoadAuthors(); AIReply("Chế độ: TÁC GIẢ."); txtQuery.Clear(); return; }
            if (aiResult == "__MODE_CATEGORIES__") { LoadCategories(); AIReply("Chế độ: THỂ LOẠI."); txtQuery.Clear(); return; }
            if (aiResult == "__MODE_BOOKS__") { LoadBooks(); AIReply("Chế độ: SÁCH."); txtQuery.Clear(); return; }

            // ✅ YEAR: chỉ output, KHÔNG chạy database
            if (aiResult.StartsWith("__SELECT_YEAR__"))
            {
                string year = aiResult.Replace("__SELECT_YEAR__:", "")
                                      .Replace("__SELECT_YEAR__", "")
                                      .Trim();

                AIReply($"SELECT PublishYear = {year}");
                txtQuery.Clear();
                return;
            }

            // ✅ SQL thật thì mới chạy DB
            try
            {
                DataTable dt = DatabaseHelper.ExecuteTable(aiResult);
                dgvMain.DataSource = dt;
                lblMode.Text = "CHẾ ĐỘ: SÁCH (từ Chat)";
                AIReply(PatternAI.LastDebugInfo, aiResult, $"Tìm thấy {dt.Rows.Count} dòng.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thực thi SQL: " + ex.Message);
            }

            txtQuery.Clear();
        }


        private void HandleConfirmation(string userInput)
        {
            string u = userInput.Trim().ToLowerInvariant();

            bool yes = u == "yes" || u == "y" || u == "ok" || u == "đồng ý" || u == "dong y" || u == "oui";
            bool no = u == "no" || u == "n" || u == "không" || u == "khong" || u == "hủy" || u == "huy";

            if (!yes && !no)
            {
                AIReply("Bạn trả lời giúp mình: yes/đồng ý/ok hoặc no/không nhé.");
                return;
            }

            if (no)
            {
                AIReply("Ok, bạn nhập lại tên tác giả khác nhé.");
                ClearPending();
                return;
            }

            LoadBooks(authorExact: _pendingSuggestedAuthor);
            AIReply(
                $"Bạn đã đồng ý dùng gợi ý \"{_pendingSuggestedAuthor}\".",
                "Mình đã hiển thị danh sách sách của tác giả này trong bảng."
            );

            ClearPending();
        }

        private void ClearPending()
        {
            _waitingConfirm = false;
            _pendingSuggestedAuthor = "";
        }


        private string ExtractAuthorFromSql(string sql)
        {
            var m = Regex.Match(sql, @"Author\s+LIKE\s+N'%(.+?)%'", RegexOptions.IgnoreCase);
            if (m.Success) return m.Groups[1].Value.Trim();
            return "";
        }

        private string FindClosestAuthorAlways(string input, List<string> authors, out int bestDistance)
        {
            string normInput = NormalizeForCompare(input);

            string best = authors[0];
            int minDist = int.MaxValue;

            foreach (var a in authors)
            {
                int d = Levenshtein(normInput, NormalizeForCompare(a));
                if (d < minDist)
                {
                    minDist = d;
                    best = a;
                }
            }

            bestDistance = minDist;
            return best;
        }

        private string NormalizeForCompare(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.ToLowerInvariant();
            s = RemoveDiacritics(s);
            s = Regex.Replace(s, @"[^a-z0-9]", "");
            return s;
        }

        private string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in normalized)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private int Levenshtein(string s, string t)
        {
            int n = s.Length, m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
}
