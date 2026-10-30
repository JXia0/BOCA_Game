using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BOCA.Editor
{
    public static class BOCAPlayerBuilder
    {
        private const string OutputPath = "tmp/BOCAVisualQA/BOCAVisualQA.exe";

        public static void BuildVisualQaPlayer()
        {
            var outputDirectory = Path.GetDirectoryName(OutputPath);
            if (!string.IsNullOrWhiteSpace(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var options = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/Scenes/MainMenu.unity",
                    "Assets/Scenes/Chapters/Chapter01_OilTide.unity",
                    "Assets/Scenes/Chapters/Chapter02_SpermWhaleRescue.unity",
                    "Assets/Scenes/Chapters/Chapter03_ReefWarning.unity",
                    "Assets/Scenes/Chapters/Chapter04_HawksbillGuzheng.unity"
                },
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException($"BOCA visual QA player build failed: {report.summary.result}");
            }

            Debug.Log($"BOCA_VISUAL_QA_BUILD_PASS: bytes={report.summary.totalSize} output={OutputPath}");
        }
    }
}
