using System;
using UnityEngine;
using UnityEngine.UI;

namespace BOCA.UI
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject homePanel;
        [SerializeField] private GameObject detailPanelFrame;
        [SerializeField] private GameObject homeFeaturePanel;
        [SerializeField] private GameObject chapterPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Text titleLabel;
        [SerializeField] private Text subtitleLabel;
        [SerializeField] private Text footerLabel;
        [SerializeField] private Text panelHeadingLabel;
        [SerializeField] private Text panelHintLabel;
        [SerializeField] private Button startButton;
        [SerializeField] private Button chapterSelectButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button englishButton;
        [SerializeField] private Button chineseButton;
        [SerializeField] private Button[] chapterButtons;
        [SerializeField] private Text[] chapterButtonLabels;
        [SerializeField] private Text startButtonLabel;
        [SerializeField] private Text chapterSelectButtonLabel;
        [SerializeField] private Text settingsButtonLabel;
        [SerializeField] private Text quitButtonLabel;
        [SerializeField] private Text backButtonLabel;
        [SerializeField] private Text englishButtonLabel;
        [SerializeField] private Text chineseButtonLabel;
        [SerializeField] private Text volumeLabel;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Text heroEyebrowLabel;
        [SerializeField] private Text currentRecordLabel;
        [SerializeField] private Text progressLabel;
        [SerializeField] private Image[] progressNodes;

        public void Configure(
            GameObject home,
            GameObject detailFrame,
            GameObject homeFeature,
            GameObject chapters,
            GameObject settings,
            Text title,
            Text subtitle,
            Text footer,
            Text panelHeading,
            Text panelHint,
            Button start,
            Button chapterSelect,
            Button settingsControl,
            Button quit,
            Button back,
            Button english,
            Button chinese,
            Button[] chapterControls,
            Text[] chapterLabels,
            Text volume,
            Slider slider,
            Text heroEyebrow,
            Text currentRecord,
            Text progress,
            Image[] nodes)
        {
            homePanel = home;
            detailPanelFrame = detailFrame;
            homeFeaturePanel = homeFeature;
            chapterPanel = chapters;
            settingsPanel = settings;
            titleLabel = title;
            subtitleLabel = subtitle;
            footerLabel = footer;
            panelHeadingLabel = panelHeading;
            panelHintLabel = panelHint;
            startButton = start;
            chapterSelectButton = chapterSelect;
            settingsButton = settingsControl;
            quitButton = quit;
            backButton = back;
            englishButton = english;
            chineseButton = chinese;
            chapterButtons = chapterControls;
            chapterButtonLabels = chapterLabels;
            startButtonLabel = start.GetComponentInChildren<Text>();
            chapterSelectButtonLabel = chapterSelect.GetComponentInChildren<Text>();
            settingsButtonLabel = settingsControl.GetComponentInChildren<Text>();
            quitButtonLabel = quit.GetComponentInChildren<Text>();
            backButtonLabel = back.GetComponentInChildren<Text>();
            englishButtonLabel = english.GetComponentInChildren<Text>();
            chineseButtonLabel = chinese.GetComponentInChildren<Text>();
            volumeLabel = volume;
            volumeSlider = slider;
            heroEyebrowLabel = heroEyebrow;
            currentRecordLabel = currentRecord;
            progressLabel = progress;
            progressNodes = nodes;
        }

        public void Bind(
            Action onStart,
            Action onChapterSelect,
            Action onSettings,
            Action onQuit,
            Action onBack,
            Action onEnglish,
            Action onChinese,
            Action<int> onChapter,
            Action<float> onVolume)
        {
            BindButton(startButton, onStart);
            BindButton(chapterSelectButton, onChapterSelect);
            BindButton(settingsButton, onSettings);
            BindButton(quitButton, onQuit);
            BindButton(backButton, onBack);
            BindButton(englishButton, onEnglish);
            BindButton(chineseButton, onChinese);

            for (var index = 0; index < chapterButtons.Length; index++)
            {
                var chapterNumber = index + 1;
                BindButton(chapterButtons[index], () => onChapter?.Invoke(chapterNumber));
            }

            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(value => onVolume?.Invoke(value));
        }

        public void Render(MainMenuRenderModel model)
        {
            titleLabel.text = model.title;
            subtitleLabel.text = model.subtitle;
            footerLabel.text = model.footer;
            panelHeadingLabel.text = model.panelHeading;
            panelHintLabel.text = model.panelHint;
            startButtonLabel.text = "01  //  " + model.start;
            chapterSelectButtonLabel.text = "02  //  " + model.chapterSelect;
            settingsButtonLabel.text = "03  //  " + model.settings;
            quitButtonLabel.text = "04  //  " + model.quit;
            backButtonLabel.text = model.back;
            englishButtonLabel.text = model.english;
            chineseButtonLabel.text = model.chinese;
            volumeLabel.text = model.volume;
            volumeSlider.SetValueWithoutNotify(model.volumeValue);
            heroEyebrowLabel.text = model.heroEyebrow;
            currentRecordLabel.text = model.currentRecord;
            progressLabel.text = model.progress;

            for (var index = 0; index < chapterButtons.Length; index++)
            {
                chapterButtonLabels[index].text = model.chapterLabels[index];
                chapterButtons[index].interactable = model.chapterUnlocked[index];
            }

            homePanel.SetActive(model.screen == MainMenuScreen.Home);
            detailPanelFrame.SetActive(true);
            homeFeaturePanel.SetActive(model.screen == MainMenuScreen.Home);
            chapterPanel.SetActive(model.screen == MainMenuScreen.Chapters);
            settingsPanel.SetActive(model.screen == MainMenuScreen.Settings);
            backButton.gameObject.SetActive(model.screen != MainMenuScreen.Home);

            for (var index = 0; index < progressNodes.Length; index++)
            {
                progressNodes[index].color = model.chapterComplete[index]
                    ? new Color32(50, 178, 142, 255)
                    : model.chapterUnlocked[index]
                        ? new Color32(232, 174, 89, 255)
                        : new Color32(55, 76, 82, 255);
            }
        }

        private static void BindButton(Button button, Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action?.Invoke());
        }
    }

    public enum MainMenuScreen
    {
        Home,
        Chapters,
        Settings
    }

    public sealed class MainMenuRenderModel
    {
        public MainMenuScreen screen;
        public string title;
        public string subtitle;
        public string footer;
        public string panelHeading;
        public string panelHint;
        public string start;
        public string chapterSelect;
        public string settings;
        public string quit;
        public string back;
        public string english;
        public string chinese;
        public string volume;
        public float volumeValue;
        public string[] chapterLabels;
        public bool[] chapterUnlocked;
        public bool[] chapterComplete;
        public string heroEyebrow;
        public string currentRecord;
        public string progress;
    }
}
