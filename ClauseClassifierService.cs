using Microsoft.ML;

namespace project_AI
{
    internal sealed class ClauseClassifierService
    {
        private readonly MLContext _ml;
        private readonly PredictionEngine<ClauseModelInput, ClausePrediction> _engine;

        public ClauseClassifierService(string modelPath)
        {
            _ml = new MLContext();

            DataViewSchema schema;
            var model = _ml.Model.Load(modelPath, out schema);
            _engine = _ml.Model.CreatePredictionEngine<ClauseModelInput, ClausePrediction>(model);
        }

        public string PredictLabel(string clause)
        {
            var pred = _engine.Predict(new ClauseModelInput
            {
                Text = ClauseFeatureHelper.BuildFeatureText(clause),
                Label = ""
            });

            return string.IsNullOrWhiteSpace(pred.PredictedLabel) ? "OTHER" : pred.PredictedLabel;
        }
    }
}