using UnityEngine;
using UnityEngine.SceneManagement;

namespace BOCA.Core
{
    public static class SceneNavigator
    {
        public const string MainMenuScene = "MainMenu";

        private static readonly string[] ChapterScenes =
        {
            "Chapter01_OilTide",
            "Chapter02_SpermWhaleRescue",
            "Chapter03_ReefWarning",
            "Chapter04_HawksbillGuzheng"
        };

        public static void LoadMainMenu()
        {
            SceneManager.LoadScene(MainMenuScene, LoadSceneMode.Single);
        }

        public static bool TryLoadChapter(int chapterNumber)
        {
            if (chapterNumber < 1 || chapterNumber > ChapterScenes.Length ||
                !GameProgressService.IsChapterUnlocked(chapterNumber))
            {
                return false;
            }

            SceneManager.LoadScene(ChapterScenes[chapterNumber - 1], LoadSceneMode.Single);
            return true;
        }

        public static void RestartCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
