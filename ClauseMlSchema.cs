using Microsoft.ML.Data;

namespace project_AI
{
    internal class ClauseTrainingRow
    {
        [LoadColumn(0)]
        public string Text { get; set; } = string.Empty;

        [LoadColumn(1)]
        public string Label { get; set; } = string.Empty;
    }

    internal class ClauseModelInput
    {
        public string Text { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    internal class ClausePrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; } = string.Empty;

        public float[] Score { get; set; } = System.Array.Empty<float>();
    }
}