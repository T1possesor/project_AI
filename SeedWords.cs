using System.Collections.Generic;

namespace project_AI
{
    internal static class SeedWords
    {
        public static readonly HashSet<string> Author = new()
        {
            "tác giả", "tac gia", "author", "writer", "written by",
            "người viết", "nguoi viet", "nhà văn", "nha van", "by"
        };

        public static readonly HashSet<string> Title = new()
        {
            "tên sách", "ten sach", "tên", "ten", "tiêu đề", "tieu de",
            "tựa đề", "tua de", "title", "book title"
        };

        public static readonly HashSet<string> Category = new()
        {
            "thể loại", "the loai", "category", "genre", "chủ đề", "chu de",
            "lĩnh vực", "linh vuc", "sách về", "thuộc loại"
        };

        public static readonly HashSet<string> Year = new()
        {
            "năm", "nam", "xuất bản", "xuat ban", "phát hành", "phat hanh",
            "ra mắt", "ra mat", "publish", "published", "year"
        };

        public static readonly HashSet<string> Id = new()
        {
            "id", "mã", "ma", "mã số", "ma so", "stt", "số thứ tự", "so thu tu"
        };
    }
}