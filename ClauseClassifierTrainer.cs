using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace project_AI
{
    internal static class ClauseClassifierTrainer
    {
        public static void Train(string dataPath, string modelPath)
        {
            var ml = new MLContext(seed: 42);

            IDataView data = ml.Data.LoadFromTextFile<ClauseTrainingRow>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',');

            var pipeline =
                ml.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",
                    inputColumnName: nameof(ClauseTrainingRow.Text))
                .Append(ml.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "LabelKey",
                    inputColumnName: nameof(ClauseTrainingRow.Label)))
                .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                    labelColumnName: "LabelKey",
                    featureColumnName: "Features"))
                .Append(ml.Transforms.Conversion.MapKeyToValue(
                    outputColumnName: "PredictedLabel"));

            var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

            var model = pipeline.Fit(split.TrainSet);

            var predictions = model.Transform(split.TestSet);
            var metrics = ml.MulticlassClassification.Evaluate(
                predictions,
                labelColumnName: "LabelKey",
                predictedLabelColumnName: "PredictedLabel");

            Console.WriteLine($"MacroAccuracy: {metrics.MacroAccuracy:F4}");
            Console.WriteLine($"MicroAccuracy: {metrics.MicroAccuracy:F4}");
            Console.WriteLine($"LogLoss: {metrics.LogLoss:F4}");

            ml.Model.Save(model, split.TrainSet.Schema, modelPath);
        }
    }
}