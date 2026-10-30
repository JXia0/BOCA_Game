using BOCA.Core;
using BOCA.Core.Localization;
using UnityEngine;

namespace BOCA.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private MainMenuView view;

        private MainMenuScreen currentScreen;

        public void Configure(MainMenuView menuView)
        {
            view = menuView;
        }

        private void OnEnable()
        {
            LocalizationService.LanguageChanged += HandleLanguageChanged;
            GameProgressService.ProgressChanged += HandleProgressChanged;
        }

        private void OnDisable()
        {
            LocalizationService.LanguageChanged -= HandleLanguageChanged;
            GameProgressService.ProgressChanged -= HandleProgressChanged;
        }

        private void Start()
        {
            view.Bind(
                StartCampaign,
                () => ShowScreen(MainMenuScreen.Chapters),
                () => ShowScreen(MainMenuScreen.Settings),
                SceneNavigator.QuitGame,
                () => ShowScreen(MainMenuScreen.Home),
                () => LocalizationService.SetLanguage(GameLanguage.English),
                () => LocalizationService.SetLanguage(GameLanguage.ChineseSimplified),
                chapter => SceneNavigator.TryLoadChapter(chapter),
                GameSettingsService.SetMasterVolume);
            ShowScreen(MainMenuScreen.Home);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && currentScreen != MainMenuScreen.Home)
            {
                ShowScreen(MainMenuScreen.Home);
            }
        }

        public bool ShowForAutomatedQa(string screen)
        {
            if (!Debug.isDebugBuild)
            {
                return false;
            }

            var target = string.Equals(screen, "chapters", System.StringComparison.OrdinalIgnoreCase)
                ? MainMenuScreen.Chapters
                : string.Equals(screen, "settings", System.StringComparison.OrdinalIgnoreCase)
                    ? MainMenuScreen.Settings
                    : MainMenuScreen.Home;
            ShowScreen(target);
            return true;
        }

        private void StartCampaign()
        {
            for (var chapter = 1; chapter <= GameProgressService.ChapterCount; chapter++)
            {
                if (GameProgressService.IsChapterUnlocked(chapter) && !GameProgressService.IsChapterComplete(chapter))
                {
                    SceneNavigator.TryLoadChapter(chapter);
                    return;
                }
            }

            SceneNavigator.TryLoadChapter(GameProgressService.ChapterCount);
        }

        private void ShowScreen(MainMenuScreen screen)
        {
            currentScreen = screen;
            Render();
        }

        private void HandleLanguageChanged(GameLanguage _)
        {
            Render();
        }

        private void HandleProgressChanged()
        {
            Render();
        }

        private void Render()
        {
            if (view == null)
            {
                return;
            }

            var labels = new string[GameProgressService.ChapterCount];
            var unlocked = new bool[GameProgressService.ChapterCount];
            var complete = new bool[GameProgressService.ChapterCount];
            var completedCount = 0;
            var nextChapter = GameProgressService.ChapterCount;
            var foundNext = false;
            for (var index = 0; index < labels.Length; index++)
            {
                var chapter = index + 1;
                unlocked[index] = GameProgressService.IsChapterUnlocked(chapter);
                complete[index] = GameProgressService.IsChapterComplete(chapter);
                if (complete[index])
                {
                    completedCount++;
                }

                if (!foundNext && unlocked[index] && !complete[index])
                {
                    nextChapter = chapter;
                    foundNext = true;
                }

                var statusKey = !unlocked[index]
                    ? "menu.status.locked"
                    : GameProgressService.IsChapterComplete(chapter)
                        ? "menu.status.complete"
                        : "menu.status.available";
                labels[index] = Localize($"menu.chapter.{chapter}") + "\n" + Localize(statusKey);
            }

            view.Render(new MainMenuRenderModel
            {
                screen = currentScreen,
                title = Localize("menu.title"),
                subtitle = Localize("menu.subtitle"),
                footer = Localize("menu.footer"),
                panelHeading = currentScreen == MainMenuScreen.Home
                    ? Localize("menu.home_heading")
                    : currentScreen == MainMenuScreen.Chapters
                    ? Localize("menu.chapter_select")
                    : currentScreen == MainMenuScreen.Settings
                        ? Localize("menu.settings")
                        : string.Empty,
                panelHint = currentScreen == MainMenuScreen.Home
                    ? Localize("menu.home_hint")
                    : currentScreen == MainMenuScreen.Chapters
                    ? Localize("menu.chapter_hint")
                    : currentScreen == MainMenuScreen.Settings
                        ? Localize("menu.settings_hint")
                        : string.Empty,
                start = Localize("menu.start"),
                chapterSelect = Localize("menu.chapter_select"),
                settings = Localize("menu.settings"),
                quit = Localize("menu.quit"),
                back = Localize("menu.back"),
                english = (LocalizationService.CurrentLanguage == GameLanguage.English ? "●  " : string.Empty) + "ENGLISH",
                chinese = (LocalizationService.CurrentLanguage == GameLanguage.ChineseSimplified ? "●  " : string.Empty) + "简体中文",
                volume = Localize("menu.volume"),
                volumeValue = GameSettingsService.MasterVolume,
                chapterLabels = labels,
                chapterUnlocked = unlocked,
                chapterComplete = complete,
                heroEyebrow = Localize("menu.current_record"),
                currentRecord = Localize($"menu.chapter.{nextChapter}"),
                progress = string.Format(Localize("menu.progress"), completedCount)
            });
        }

        private static string Localize(string key)
        {
            return LocalizationService.Get("menu", key, key);
        }
    }
}
