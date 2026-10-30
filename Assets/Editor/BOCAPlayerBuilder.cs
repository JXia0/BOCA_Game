using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BOCA.Editor
{
    public static class BOCAPlayerBuilder
    {
        private const string VisualQaOutputPath = "tmp/BOCAVisualQA/BOCAVisualQA.exe";
        private const string WindowsReleaseOutputPath = "Builds/BOCA_Windows/BOCA.exe";
        private const string WebGlReleaseOutputPath = "Builds/BOCA_WebGL";

        private static readonly string[] ProductionScenes =
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Chapters/Chapter01_OilTide.unity",
            "Assets/Scenes/Chapters/Chapter02_SpermWhaleRescue.unity",
            "Assets/Scenes/Chapters/Chapter03_ReefWarning.unity",
            "Assets/Scenes/Chapters/Chapter04_HawksbillGuzheng.unity"
        };

        public static void BuildVisualQaPlayer()
        {
            BuildWindowsPlayer(VisualQaOutputPath, BuildOptions.Development, "BOCA_VISUAL_QA_BUILD_PASS");
        }

        public static void BuildWindowsRelease()
        {
            BOCAProductionValidator.ValidateProject();
            BuildWindowsPlayer(WindowsReleaseOutputPath, BuildOptions.None, "BOCA_WINDOWS_RELEASE_BUILD_PASS");
        }

        public static void BuildWebGlRelease()
        {
            BOCAProductionValidator.ValidateProject();
            if (Directory.Exists(WebGlReleaseOutputPath))
            {
                Directory.Delete(WebGlReleaseOutputPath, true);
            }

            var previousCompression = PlayerSettings.WebGL.compressionFormat;
            var previousFallback = PlayerSettings.WebGL.decompressionFallback;
            try
            {
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
                PlayerSettings.WebGL.decompressionFallback = true;

                var options = new BuildPlayerOptions
                {
                    scenes = ProductionScenes,
                    locationPathName = WebGlReleaseOutputPath,
                    target = BuildTarget.WebGL,
                    options = BuildOptions.None
                };

                var report = BuildPipeline.BuildPlayer(options);
                if (report.summary.result != BuildResult.Succeeded)
                {
                    throw new BuildFailedException($"BOCA WebGL player build failed: {report.summary.result}");
                }

                Debug.Log($"BOCA_WEBGL_RELEASE_BUILD_PASS: bytes={report.summary.totalSize} output={WebGlReleaseOutputPath}");
            }
            finally
            {
                PlayerSettings.WebGL.compressionFormat = previousCompression;
                PlayerSettings.WebGL.decompressionFallback = previousFallback;
            }
        }

        private static void BuildWindowsPlayer(string outputPath, BuildOptions buildOptions, string successMarker)
        {
            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var options = new BuildPlayerOptions
            {
                scenes = ProductionScenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = buildOptions
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException($"BOCA Windows player build failed: {report.summary.result}");
            }

            Debug.Log($"{successMarker}: bytes={report.summary.totalSize} output={outputPath}");
        }
    }
}
