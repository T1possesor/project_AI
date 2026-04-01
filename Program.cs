using System;
using System.IO;
using System.Windows.Forms;

namespace project_AI
{
    internal static class Program
    {
        public static ClauseClassifierService? ClauseClassifier { get; private set; }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataPath = Path.Combine(baseDir, "clause_training_data.csv");
            string modelPath = Path.Combine(baseDir, "clause_model.zip");

            try
            {
                if (!File.Exists(modelPath))
                {
                    if (File.Exists(dataPath))
                    {
                        ClauseClassifierTrainer.Train(dataPath, modelPath);
                        MessageBox.Show("Train model xong");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy file train:\n" + dataPath);
                    }
                }

                if (File.Exists(modelPath))
                {
                    ClauseClassifier = new ClauseClassifierService(modelPath);
                    MessageBox.Show("Load model thành công");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy clause_model.zip tại:\n" + modelPath);
                    ClauseClassifier = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load/Train model lỗi:\n" + ex.Message);
                ClauseClassifier = null;
            }

            Application.Run(new Form1());
        }
    }
}