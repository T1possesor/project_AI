using System.Text;

namespace project_AI
{
    internal class PatternAI
    {
        public static string LastDebugInfo { get; private set; } = "";

        public static string GenerateSql(string input)
        {
            if (!QueryGateKeeper.IsAllowed(input))
                return "__REJECT__";

            var intent = new QueryIntent();

            intent.Id = null;
            intent.IdNot = false;
            intent.IdGreaterThan = null;
            intent.IdLessThan = null;
            intent.IdFrom = null;
            intent.IdTo = null;
            intent.WantListIds = false;

            intent.Author = null;
            intent.AuthorNot = false;
            intent.AuthorJoiner = null;

            intent.Category = null;
            intent.CategoryNot = false;
            intent.CategoryJoiner = null;

            intent.Title = null;
            intent.TitleValues = null;
            intent.TitleMode = null;
            intent.TitleNot = false;
            intent.TitleJoiner = null;

            intent.YearEq = null;
            intent.YearNot = false;
            intent.YearAfter = null;
            intent.YearBefore = null;
            intent.YearFrom = null;
            intent.YearTo = null;
            intent.YearValues = null;
            intent.YearJoiner = null;

            ModeExtractor.Apply(input, intent);

            var clauses = ClauseSplitter.Split(input);

            var debug = new StringBuilder();
            debug.AppendLine("DEBUG ML:");

            foreach (var c in clauses)
            {
                string label = Program.ClauseClassifier?.PredictLabel(c) ?? "OTHER";
                debug.AppendLine($"CLAUSE = [{c}] => LABEL = [{label}]");

                switch (label)
                {
                    case "AUTHOR":
                        AuthorExtractor.Apply(c, intent);
                        break;

                    case "TITLE":
                        TitleExtractor.Apply(c, intent);
                        KeywordExtractor.Apply(c, intent);
                        break;

                    case "CATEGORY":
                        CategoryExtractor.Apply(c, intent);
                        break;

                    case "YEAR":
                        YearExtractor.Apply(c, intent);
                        ComparatorExtractor.Apply(c, intent);
                        RangeExtractors.Apply(c, intent);
                        break;

                    case "ID":
                        IdExtractor.Apply(c, intent);
                        ComparatorExtractor.Apply(c, intent);
                        RangeExtractors.Apply(c, intent);
                        break;

                    default:
                        IdExtractor.Apply(c, intent);
                        YearExtractor.Apply(c, intent);
                        TitleExtractor.Apply(c, intent);
                        AuthorExtractor.Apply(c, intent);
                        CategoryExtractor.Apply(c, intent);
                        KeywordExtractor.Apply(c, intent);
                        break;
                }
            }

            LastDebugInfo = debug.ToString();

            return SqlBuilder.Build(intent);
        }
    }
}