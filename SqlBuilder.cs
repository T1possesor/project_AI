using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace project_AI
{
    internal static class SqlBuilder
    {
        public static string Build(QueryIntent intent)
        {
            if (!string.IsNullOrWhiteSpace(intent.ErrorMessage))
                return intent.ErrorMessage!;

            if (intent.WantListAuthors)
            {
                var conds = BuildConditions(intent);

                var sb = new StringBuilder();
                sb.Append("SELECT DISTINCT Author FROM Books");

                if (conds.Count > 0)
                    sb.Append(" WHERE " + string.Join(" AND ", conds));

                sb.Append(" ORDER BY Author");
                return sb.ToString();
            }

            if (intent.WantListCategories)
            {
                var conds = BuildConditions(intent);

                var sb = new StringBuilder();
                sb.Append("SELECT DISTINCT Category FROM Books");

                if (conds.Count > 0)
                    sb.Append(" WHERE " + string.Join(" AND ", conds));

                sb.Append(" ORDER BY Category");
                return sb.ToString();
            }

            if (intent.WantListIds)
            {
                var conds = BuildConditions(intent);

                var sb = new StringBuilder();
                sb.Append("SELECT DISTINCT Id FROM Books");

                if (conds.Count > 0)
                    sb.Append(" WHERE " + string.Join(" AND ", conds));

                sb.Append(" ORDER BY Id");
                return sb.ToString();
            }

            if (intent.WantListYears)
            {
                var conds = BuildConditions(intent);

                var sb = new StringBuilder();
                sb.Append("SELECT DISTINCT PublishYear FROM Books");

                if (conds.Count > 0)
                    sb.Append(" WHERE " + string.Join(" AND ", conds));

                sb.Append(" ORDER BY PublishYear");
                return sb.ToString();
            }

            {
                var conds = BuildConditions(intent);

                var sb = new StringBuilder();

                sb.Append("SELECT ");

                if (intent.Limit.HasValue)
                    sb.Append($"TOP {intent.Limit.Value} ");

                sb.Append("* FROM Books");

                if (conds.Count > 0)
                    sb.Append(" WHERE " + string.Join(" AND ", conds));

                return sb.ToString();
            }
        }

        private static List<string> BuildConditions(QueryIntent intent)
        {
            var conds = new List<string>();

            if (intent.Id.HasValue)
                conds.Add(intent.IdNot ? $"Id <> {intent.Id.Value}" : $"Id = {intent.Id.Value}");

            if (intent.IdFrom.HasValue && intent.IdTo.HasValue)
                conds.Add($"Id BETWEEN {intent.IdFrom.Value} AND {intent.IdTo.Value}");

            if (intent.IdGreaterThan.HasValue)
                conds.Add($"Id > {intent.IdGreaterThan.Value}");

            if (intent.IdLessThan.HasValue)
                conds.Add($"Id < {intent.IdLessThan.Value}");

            if (intent.TitleValues != null && intent.TitleValues.Count > 0 && intent.TitleMode.HasValue)
            {
                var parts = new List<string>();

                foreach (var raw in intent.TitleValues)
                {
                    var v = TextUtil.EscapeSqlLike(raw);

                    string cond = intent.TitleMode.Value switch
                    {
                        TitleMatchMode.Exact =>
                            $"Title = N'{v}'",

                        TitleMatchMode.StartsWith =>
                            $"Title LIKE N'{v}%'",

                        TitleMatchMode.EndsWith =>
                            $"Title LIKE N'%{v}'",

                        _ =>
                            $"Title LIKE N'%{v}%'"
                    };

                    parts.Add(cond);
                }

                string joiner = string.IsNullOrWhiteSpace(intent.TitleJoiner) ? "AND" : intent.TitleJoiner!.ToUpperInvariant();
                if (joiner != "AND" && joiner != "OR") joiner = "AND";

                string joined =
                    parts.Count == 1
                        ? parts[0]
                        : $"({string.Join($" {joiner} ", parts)})";

                if (intent.TitleNot)
                    joined = $"NOT {joined}";

                conds.Add(joined);
            }
            else if (!string.IsNullOrWhiteSpace(intent.Title))
            {
                string clean = intent.Title.Trim();

                clean = Regex.Replace(clean, @"^\s*(tên|ten|title)\s*(là|=|:)\s*", "", RegexOptions.IgnoreCase);

                string v = TextUtil.EscapeSqlLike(clean);
                conds.Add(intent.TitleNot ? $"Title NOT LIKE N'%{v}%'" : $"Title LIKE N'%{v}%'");
            }

            if (!string.IsNullOrWhiteSpace(intent.Author))
            {
                var rawParts = intent.Author
                    .Split('|')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToList();

                var parts = new List<string>();

                foreach (var raw in rawParts)
                {
                    string v = TextUtil.EscapeSqlLike(raw);
                    parts.Add($"Author LIKE N'%{v}%'");
                }

                string joiner = string.IsNullOrWhiteSpace(intent.AuthorJoiner)
                    ? "AND"
                    : intent.AuthorJoiner.ToUpperInvariant();

                if (joiner != "AND" && joiner != "OR")
                    joiner = "AND";

                string joined =
                    parts.Count == 1
                        ? parts[0]
                        : $"({string.Join($" {joiner} ", parts)})";

                if (intent.AuthorNot)
                {
                    if (joiner == "OR")
                    {
                        joined = string.Join(" AND ",
                            parts.Select(p => $"NOT {p}")
                        );
                    }
                    else
                    {
                        joined = $"NOT {joined}";
                    }
                }

                conds.Add(joined);
            }

            if (!string.IsNullOrWhiteSpace(intent.Category))
            {
                var rawParts = intent.Category
                    .Split('|')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToList();

                var parts = new List<string>();

                foreach (var raw in rawParts)
                {
                    string v = TextUtil.EscapeSqlLike(raw);
                    parts.Add($"Category LIKE N'%{v}%'");
                }

                string joiner = string.IsNullOrWhiteSpace(intent.CategoryJoiner)
                    ? "AND"
                    : intent.CategoryJoiner.ToUpperInvariant();

                if (joiner != "AND" && joiner != "OR")
                    joiner = "AND";

                string joined =
                    parts.Count == 1
                        ? parts[0]
                        : $"({string.Join($" {joiner} ", parts)})";

                if (intent.CategoryNot)
                {
                    if (joiner == "OR")
                    {
                        joined = string.Join(" AND ",
                            parts.Select(p => $"NOT {p}")
                        );
                    }
                    else
                    {
                        joined = $"NOT {joined}";
                    }
                }


                conds.Add(joined);
            }

            if (intent.YearValues != null && intent.YearValues.Count > 0)
            {
                var parts = new List<string>();

                foreach (var y in intent.YearValues)
                    parts.Add($"PublishYear = {y}");

                string joiner = string.IsNullOrWhiteSpace(intent.YearJoiner)
                    ? "OR"
                    : intent.YearJoiner.ToUpperInvariant();

                if (joiner != "AND" && joiner != "OR")
                    joiner = "OR";

                string joined =
                    parts.Count == 1
                        ? parts[0]
                        : $"({string.Join($" {joiner} ", parts)})";

                if (intent.YearNot)
                    joined = $"NOT {joined}";

                conds.Add(joined);

                return conds;
            }

            if (intent.YearBefore.HasValue || intent.YearAfter.HasValue)
            {
                intent.YearEq = null;
                intent.YearNot = false;
            }


            if (intent.YearEq.HasValue)
                conds.Add(intent.YearNot ? $"PublishYear <> {intent.YearEq.Value}" : $"PublishYear = {intent.YearEq.Value}");

            if (intent.YearFrom.HasValue && intent.YearTo.HasValue)
                conds.Add($"PublishYear BETWEEN {intent.YearFrom.Value} AND {intent.YearTo.Value}");

            if (intent.YearBefore.HasValue)
                conds.Add($"PublishYear < {intent.YearBefore.Value}");

            if (intent.YearAfter.HasValue)
                conds.Add($"PublishYear > {intent.YearAfter.Value}");

            return conds;
        }
    }
}
