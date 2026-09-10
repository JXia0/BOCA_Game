using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BOCA.Diagnostics
{
    /// <summary>
    /// Opt-in visual QA hook used only when a built player receives -bocaCapture.
    /// It never runs during a normal player session.
    /// </summary>
    public sealed class AutomatedVisualCapture : MonoBehaviour
    {
        private const string CaptureArgument = "-bocaCapture=";
        private const string ChapterArgument = "-bocaChapter=";
        private const string StepArgument = "-bocaSteps=";
        private const string RouteArgument = "-bocaRoute=";

        private string capturePath;
        private int chapterBuildIndex;
        private int advanceSteps;
        private string preferredRoute;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            var capturePath = ReadArgument(CaptureArgument);
            if (string.IsNullOrWhiteSpace(capturePath))
            {
                return;
            }

            var chapterText = ReadArgument(ChapterArgument);
            var chapterNumber = int.TryParse(chapterText, out var parsed) ? Mathf.Clamp(parsed, 1, 4) : 4;

            var host = new GameObject("Automated Visual Capture");
            DontDestroyOnLoad(host);
            var runner = host.AddComponent<AutomatedVisualCapture>();
            runner.capturePath = capturePath;
            runner.chapterBuildIndex = chapterNumber - 1;
            runner.advanceSteps = int.TryParse(ReadArgument(StepArgument), out var steps) ? Mathf.Max(0, steps) : 0;
            runner.preferredRoute = ReadArgument(RouteArgument) ?? "A";
        }

        private IEnumerator Start()
        {
            if (SceneManager.GetActiveScene().buildIndex != chapterBuildIndex)
            {
                var operation = SceneManager.LoadSceneAsync(chapterBuildIndex, LoadSceneMode.Single);
                while (operation != null && !operation.isDone)
                {
                    yield return null;
                }
            }

            yield return null;
            yield return new WaitForEndOfFrame();
            var controller = FindAnyObjectByType<BOCA.Narrative.ChapterSceneController>();
            for (var step = 0; step < advanceSteps && controller != null; step++)
            {
                if (!controller.AdvanceForAutomatedQa(preferredRoute))
                {
                    break;
                }

                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.35f);

            var directory = Path.GetDirectoryName(capturePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            ScreenCapture.CaptureScreenshot(capturePath, 1);
            for (var frame = 0; frame < 180 && !File.Exists(capturePath); frame++)
            {
                yield return null;
            }

            Debug.Log(File.Exists(capturePath)
                ? $"BOCA_VISUAL_CAPTURE_PASS: {capturePath}"
                : $"BOCA_VISUAL_CAPTURE_FAIL: {capturePath}");
            Application.Quit(File.Exists(capturePath) ? 0 : 2);
        }

        private static string ReadArgument(string prefix)
        {
            foreach (var argument in Environment.GetCommandLineArgs())
            {
                if (argument.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return argument.Substring(prefix.Length).Trim('"');
                }
            }

            return null;
        }
    }
}
