using System;
using System.Collections.Generic;
using BOCA.Art;
using BOCA.Core;
using BOCA.Narrative;
using BOCA.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BOCA.Editor
{
    public static class BOCAProductionSceneBuilder
    {
        private const string SceneFolder = "Assets/Scenes/Chapters";
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private static readonly Color Navy = new Color32(10, 27, 38, 255);
        private static readonly Color DeepBlue = new Color32(18, 48, 65, 255);
        private static readonly Color SeaGreen = new Color32(50, 178, 142, 255);
        private static readonly Color LightText = new Color32(235, 244, 241, 255);
        private static readonly Color MutedText = new Color32(164, 193, 190, 255);
        private static readonly Color ChoiceIdle = new Color32(35, 72, 84, 255);
        private static readonly Color ChoiceHover = new Color32(50, 111, 112, 255);

        private static readonly ChapterSpec[] Chapters =
        {
            new ChapterSpec(1, "chapter-01", "Chapter01_OilTide", "Assets/Resources/BOCA/Flows/chapter-01.json"),
            new ChapterSpec(2, "chapter-02", "Chapter02_SpermWhaleRescue", "Assets/Resources/BOCA/Flows/chapter-02.json"),
            new ChapterSpec(3, "chapter-03", "Chapter03_ReefWarning", "Assets/Resources/BOCA/Flows/chapter-03.json"),
            new ChapterSpec(4, "chapter-04", "Chapter04_HawksbillGuzheng", "Assets/Resources/BOCA/Flows/chapter-04.json")
        };

        [MenuItem("BOCA/Production/Rebuild All Chapter Scenes")]
        private static void BuildAllScenesFromMenu()
        {
            if (EditorUtility.DisplayDialog(
                    "Rebuild all chapter scenes",
                    "This replaces the four generated chapter scene files. Continue only when intentionally changing the shared hierarchy.",
                    "Rebuild",
                    "Cancel"))
            {
                BuildAllScenes();
            }
        }

        public static void BuildAllScenes()
        {
            BOCAArtCatalogBuilder.RebuildAll();
            var sharedCatalog = AssetDatabase.LoadAssetAtPath<ArtCatalog>(BOCAArtCatalogBuilder.SharedCatalogPath);
            var buildScenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(BuildMainMenuScene(), true)
            };

            foreach (var chapter in Chapters)
            {
                var scenePath = BuildScene(chapter, sharedCatalog);
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = buildScenes.ToArray();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("BOCA_PRODUCTION_SCENES_BUILT: menu + 4 chapters");
        }

        private static string BuildMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateMenuCamera();
            CreateEventSystem();

            var canvasObject = new GameObject("MainMenuCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var backdrop = CreateImage("OceanBackdrop", canvasObject.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Color.white);
            backdrop.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_SUNSET_SEA.png");
            backdrop.preserveAspect = false;
            CreateImage("OceanTint", canvasObject.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.02f, 0.08f, 0.12f, 0.76f));

            var title = CreateText("Title", canvasObject.transform, new Vector2(0.08f, 0.69f), new Vector2(0.48f, 0.89f),
                Vector2.zero, Vector2.zero, font, 78, FontStyle.Bold, LightText, TextAnchor.LowerLeft);
            var subtitle = CreateText("Subtitle", canvasObject.transform, new Vector2(0.082f, 0.63f), new Vector2(0.54f, 0.70f),
                Vector2.zero, Vector2.zero, font, 22, FontStyle.Bold, SeaGreen, TextAnchor.UpperLeft);
            var footer = CreateText("Footer", canvasObject.transform, new Vector2(0.08f, 0.04f), new Vector2(0.70f, 0.10f),
                Vector2.zero, Vector2.zero, font, 17, FontStyle.Normal, MutedText, TextAnchor.MiddleLeft);

            var homePanel = CreatePanel("HomePanel", canvasObject.transform, new Vector2(0.08f, 0.18f), new Vector2(0.42f, 0.61f));
            var startButton = CreateMenuButton("StartButton", homePanel.transform, 0, font);
            var chapterButton = CreateMenuButton("ChapterSelectButton", homePanel.transform, 1, font);
            var settingsButton = CreateMenuButton("SettingsButton", homePanel.transform, 2, font);
            var quitButton = CreateMenuButton("QuitButton", homePanel.transform, 3, font);

            var rightPanel = CreatePanel("RightPanelFrame", canvasObject.transform, new Vector2(0.48f, 0.15f), new Vector2(0.92f, 0.84f));
            rightPanel.color = new Color(Navy.r, Navy.g, Navy.b, 0.94f);
            var panelHeading = CreateText("PanelHeading", rightPanel.transform, new Vector2(0.07f, 0.83f), new Vector2(0.93f, 0.96f),
                Vector2.zero, Vector2.zero, font, 32, FontStyle.Bold, LightText, TextAnchor.MiddleLeft);
            var panelHint = CreateText("PanelHint", rightPanel.transform, new Vector2(0.07f, 0.74f), new Vector2(0.93f, 0.84f),
                Vector2.zero, Vector2.zero, font, 17, FontStyle.Normal, MutedText, TextAnchor.UpperLeft);

            var homeFeature = CreateImage("HomeFeaturePanel", rightPanel.transform, new Vector2(0.06f, 0.12f), new Vector2(0.94f, 0.73f),
                Vector2.zero, Vector2.zero, Color.white);
            homeFeature.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Chapters/Chapter01_OilTide/Backgrounds/C01_BG_UNDERSEA_ARCHIVE_OPENING.png");
            homeFeature.preserveAspect = false;
            var homeOutline = homeFeature.gameObject.AddComponent<Outline>();
            homeOutline.effectColor = new Color(SeaGreen.r, SeaGreen.g, SeaGreen.b, 0.82f);
            homeOutline.effectDistance = new Vector2(2f, -2f);
            var homeShade = CreateImage("HomeFeatureShade", homeFeature.transform, Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero, new Color(Navy.r, Navy.g, Navy.b, 0.48f));
            homeShade.raycastTarget = false;
            var heroEyebrow = CreateText("HeroEyebrow", homeFeature.transform, new Vector2(0.06f, 0.25f), new Vector2(0.94f, 0.36f),
                Vector2.zero, Vector2.zero, font, 16, FontStyle.Bold, SeaGreen, TextAnchor.MiddleLeft);
            var currentRecord = CreateText("CurrentRecord", homeFeature.transform, new Vector2(0.06f, 0.10f), new Vector2(0.94f, 0.27f),
                Vector2.zero, Vector2.zero, font, 24, FontStyle.Bold, LightText, TextAnchor.MiddleLeft);
            var progress = CreateText("ProgressLabel", homeFeature.transform, new Vector2(0.06f, 0.01f), new Vector2(0.55f, 0.10f),
                Vector2.zero, Vector2.zero, font, 14, FontStyle.Bold, MutedText, TextAnchor.MiddleLeft);
            var progressNodes = new Image[GameProgressService.ChapterCount];
            for (var index = 0; index < progressNodes.Length; index++)
            {
                var xMin = 0.66f + index * 0.075f;
                progressNodes[index] = CreateImage($"RecordNode{index + 1}", homeFeature.transform,
                    new Vector2(xMin, 0.035f), new Vector2(xMin + 0.045f, 0.075f), Vector2.zero, Vector2.zero, DeepBlue);
                progressNodes[index].raycastTarget = false;
                var nodeLabel = CreateText("Label", progressNodes[index].transform, Vector2.zero, Vector2.one,
                    Vector2.zero, Vector2.zero, font, 11, FontStyle.Bold, LightText, TextAnchor.MiddleCenter);
                nodeLabel.text = (index + 1).ToString("00");
            }

            var chapterPanel = CreatePanel("ChapterPanel", rightPanel.transform, new Vector2(0.06f, 0.12f), new Vector2(0.94f, 0.73f));
            chapterPanel.color = Color.clear;
            var chapterControls = new Button[GameProgressService.ChapterCount];
            var chapterLabels = new Text[GameProgressService.ChapterCount];
            for (var index = 0; index < chapterControls.Length; index++)
            {
                var top = 1f - index * 0.24f;
                var image = CreateImage($"Chapter{index + 1}Button", chapterPanel.transform, new Vector2(0f, top - 0.20f), new Vector2(1f, top),
                    Vector2.zero, Vector2.zero, ChoiceIdle);
                chapterControls[index] = ConfigureButton(image);
                chapterLabels[index] = CreateText("Label", image.transform, Vector2.zero, Vector2.one,
                    new Vector2(24f, 8f), new Vector2(-24f, -8f), font, 19, FontStyle.Bold, LightText, TextAnchor.MiddleLeft);
            }

            var settingsPanel = CreatePanel("SettingsPanel", rightPanel.transform, new Vector2(0.06f, 0.12f), new Vector2(0.94f, 0.73f));
            settingsPanel.color = Color.clear;
            var englishButton = CreateButton("EnglishButton", settingsPanel.transform, new Vector2(0f, 0.70f), new Vector2(0.48f, 0.88f),
                Vector2.zero, Vector2.zero, font, 18, "ENGLISH");
            var chineseButton = CreateButton("ChineseButton", settingsPanel.transform, new Vector2(0.52f, 0.70f), new Vector2(1f, 0.88f),
                Vector2.zero, Vector2.zero, font, 18, "简体中文");
            var volumeLabel = CreateText("VolumeLabel", settingsPanel.transform, new Vector2(0f, 0.42f), new Vector2(1f, 0.58f),
                Vector2.zero, Vector2.zero, font, 19, FontStyle.Bold, LightText, TextAnchor.MiddleLeft);
            var volumeSlider = CreateSlider("VolumeSlider", settingsPanel.transform, new Vector2(0f, 0.27f), new Vector2(1f, 0.40f));

            var backButton = CreateButton("BackButton", rightPanel.transform, new Vector2(0.68f, 0.035f), new Vector2(0.94f, 0.115f),
                Vector2.zero, Vector2.zero, font, 17, "BACK");

            var view = canvasObject.AddComponent<MainMenuView>();
            view.Configure(homePanel.gameObject, rightPanel.gameObject, homeFeature.gameObject, chapterPanel.gameObject, settingsPanel.gameObject, title, subtitle, footer,
                panelHeading, panelHint, startButton, chapterButton, settingsButton, quitButton, backButton,
                englishButton, chineseButton, chapterControls, chapterLabels, volumeLabel, volumeSlider,
                heroEyebrow, currentRecord, progress, progressNodes);
            var controller = new GameObject("MainMenuController").AddComponent<MainMenuController>();
            controller.Configure(view);

            if (!EditorSceneManager.SaveScene(scene, MainMenuScenePath))
            {
                throw new InvalidOperationException($"Could not save production scene '{MainMenuScenePath}'.");
            }

            return MainMenuScenePath;
        }

        private static string BuildScene(ChapterSpec chapter, ArtCatalog sharedCatalog)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera(chapter);
            CreateEventSystem();

            var canvasObject = new GameObject("NarrativeCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var backgroundFrame = CreateImage("SceneArtworkFrame", canvasObject.transform, Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero, chapter.fallbackColor);
            var background = CreateImage("SceneArtwork", backgroundFrame.transform, Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero, chapter.fallbackColor);

            var topBar = CreateImage("ChapterHeader", canvasObject.transform, new Vector2(0f, 0.91f), Vector2.one, Vector2.zero, Vector2.zero, new Color(Navy.r, Navy.g, Navy.b, 0.94f));
            var chapterTitle = CreateText("ChapterTitle", topBar.transform, Vector2.zero, Vector2.one,
                new Vector2(42f, 8f), new Vector2(-920f, -8f), font, 28, FontStyle.Bold, LightText, TextAnchor.MiddleLeft);
            var sceneLabel = CreateText("SceneLabel", topBar.transform, Vector2.zero, Vector2.one,
                new Vector2(1000f, 8f), new Vector2(-330f, -8f), font, 20, FontStyle.Bold, MutedText, TextAnchor.MiddleRight);
            var continueButton = CreateButton("ContinueButton", topBar.transform, new Vector2(1f, 0f), Vector2.one,
                new Vector2(-300f, 14f), new Vector2(-34f, -14f), font, 21, "CONTINUE");
            var menuButton = CreateButton("MenuButton", topBar.transform, new Vector2(1f, 0f), Vector2.one,
                new Vector2(-448f, 14f), new Vector2(-312f, -14f), font, 17, "MENU");

            var caseStatus = CreateText("CaseStatus", canvasObject.transform, new Vector2(0.58f, 0.855f), new Vector2(0.98f, 0.905f),
                Vector2.zero, Vector2.zero, font, 17, FontStyle.Bold, new Color(0.82f, 0.94f, 0.90f, 0.94f), TextAnchor.MiddleRight);

            var dialoguePanel = CreateImage("DialoguePanel", canvasObject.transform, Vector2.zero, new Vector2(1f, 0.43f),
                Vector2.zero, Vector2.zero, new Color(Navy.r, Navy.g, Navy.b, 0.98f));
            CreateImage("DialogueTopBorder", dialoguePanel.transform, new Vector2(0f, 1f), Vector2.one,
                new Vector2(0f, -6f), Vector2.zero, SeaGreen);

            var portraitFrame = CreateImage("PortraitFrame", dialoguePanel.transform, Vector2.zero, Vector2.zero,
                new Vector2(30f, 24f), new Vector2(318f, 335f), DeepBlue);
            var portrait = CreateImage("PortraitArtwork", portraitFrame.transform, Vector2.zero, Vector2.one,
                new Vector2(6f, 6f), new Vector2(-6f, -6f), new Color32(31, 59, 68, 255));
            portrait.preserveAspect = true;

            var speaker = CreateText("Speaker", dialoguePanel.transform, Vector2.zero, Vector2.one,
                new Vector2(360f, 405f), new Vector2(-80f, -18f), font, 25, FontStyle.Bold, SeaGreen, TextAnchor.MiddleLeft);
            var body = CreateText("DialogueBody", dialoguePanel.transform, Vector2.zero, Vector2.one,
                new Vector2(360f, 90f), new Vector2(-80f, -70f), font, 27, FontStyle.Normal, LightText, TextAnchor.UpperLeft);
            body.resizeTextForBestFit = true;
            body.resizeTextMinSize = 18;
            body.resizeTextMaxSize = 27;

            var interactionPrompt = CreateText("InteractionPrompt", dialoguePanel.transform, Vector2.zero, Vector2.one,
                new Vector2(360f, 190f), new Vector2(-80f, -250f), font, 18, FontStyle.Bold, MutedText, TextAnchor.MiddleLeft);

            var choiceButtons = new Button[6];
            var choiceLabels = new Text[6];
            for (var i = 0; i < choiceButtons.Length; i++)
            {
                var column = i % 3;
                var row = i / 3;
                const float left = 360f;
                const float right = 80f;
                const float gap = 14f;
                var availableWidth = 1920f - left - right;
                var width = (availableWidth - gap * 2f) / 3f;
                var xMin = left + column * (width + gap);
                var xMax = xMin + width;
                var bottom = row == 0 ? 94f : 24f;
                choiceButtons[i] = CreateButton($"ChoiceButton{i + 1}", dialoguePanel.transform, Vector2.zero, Vector2.zero,
                    new Vector2(xMin, bottom), new Vector2(xMax, bottom + 60f), font, 17, string.Empty);
                choiceLabels[i] = choiceButtons[i].GetComponentInChildren<Text>();
                choiceButtons[i].gameObject.SetActive(false);
            }

            var replayButton = CreateButton("ReplayButton", dialoguePanel.transform, Vector2.zero, Vector2.zero,
                new Vector2(60f, 24f), new Vector2(350f, 84f), font, 17, "REPLAY CHAPTER");
            var chapterSelectButton = CreateButton("ChapterSelectButton", dialoguePanel.transform, Vector2.zero, Vector2.zero,
                new Vector2(370f, 24f), new Vector2(700f, 84f), font, 17, "CHAPTER SELECT");
            replayButton.gameObject.SetActive(false);
            chapterSelectButton.gameObject.SetActive(false);

            var view = canvasObject.AddComponent<ChapterView>();
            view.Configure(chapterTitle, sceneLabel, speaker, body, interactionPrompt, caseStatus, background, portrait,
                continueButton, continueButton.GetComponentInChildren<Text>(), menuButton, replayButton, chapterSelectButton, choiceButtons, choiceLabels);

            var controllerObject = new GameObject("ChapterSceneController");
            var controller = controllerObject.AddComponent<ChapterSceneController>();
            var flowAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(chapter.flowPath);
            var chapterCatalog = BOCAArtCatalogBuilder.LoadChapterCatalog(chapter.number);
            controller.Configure(chapter.number, chapter.chapterId, flowAsset, new[] { chapterCatalog, sharedCatalog }, view);

            var scenePath = $"{SceneFolder}/{chapter.sceneName}.unity";
            if (!EditorSceneManager.SaveScene(scene, scenePath))
            {
                throw new InvalidOperationException($"Could not save production scene '{scenePath}'.");
            }

            return scenePath;
        }

        private static void CreateCamera(ChapterSpec chapter)
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = chapter.fallbackColor;
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }

        private static void CreateMenuCamera()
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Navy;
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }

        private static Image CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            return CreateImage(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero,
                new Color(Navy.r, Navy.g, Navy.b, 0.76f));
        }

        private static Button CreateMenuButton(string name, Transform parent, int index, Font font)
        {
            const float height = 0.20f;
            const float gap = 0.055f;
            var top = 1f - index * (height + gap);
            var button = CreateButton(name, parent, new Vector2(0f, top - height), new Vector2(1f, top),
                Vector2.zero, Vector2.zero, font, 21, string.Empty);
            var arrow = CreateText("Arrow", button.transform, new Vector2(0.86f, 0f), Vector2.one,
                Vector2.zero, new Vector2(-20f, 0f), font, 26, FontStyle.Bold, SeaGreen, TextAnchor.MiddleRight);
            arrow.text = ">";
            return button;
        }

        private static Button ConfigureButton(Image image)
        {
            var button = image.gameObject.AddComponent<Button>();
            image.color = Color.white;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = ChoiceIdle;
            colors.highlightedColor = ChoiceHover;
            colors.pressedColor = SeaGreen;
            colors.selectedColor = ChoiceHover;
            colors.disabledColor = new Color32(35, 48, 52, 190);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            ApplyPixelButtonChrome(image);
            return button;
        }

        private static Slider CreateSlider(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Slider));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var background = CreateImage("Background", root.transform, new Vector2(0f, 0.35f), new Vector2(1f, 0.65f),
                Vector2.zero, Vector2.zero, DeepBlue);
            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(root.transform, false);
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.35f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.65f);
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;
            var fill = CreateImage("Fill", fillArea.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, SeaGreen);
            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(root.transform, false);
            var handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10f, 0f);
            handleAreaRect.offsetMax = new Vector2(-10f, 0f);
            var handle = CreateImage("Handle", handleArea.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                new Vector2(-12f, -24f), new Vector2(12f, 24f), LightText);

            var slider = root.GetComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.8f;
            background.raycastTarget = false;
            return slider;
        }

        private static void CreateEventSystem()
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static Image CreateImage(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            var rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            var image = gameObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax, Font font, int fontSize, FontStyle style, Color color, TextAnchor alignment)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            var rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            var text = gameObject.GetComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax, Font font, int fontSize, string label)
        {
            var image = CreateImage(name, parent, anchorMin, anchorMax, offsetMin, offsetMax, ChoiceIdle);
            var button = image.gameObject.AddComponent<Button>();
            image.color = Color.white;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = ChoiceIdle;
            colors.highlightedColor = ChoiceHover;
            colors.pressedColor = SeaGreen;
            colors.selectedColor = ChoiceHover;
            colors.disabledColor = new Color32(41, 54, 58, 180);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            ApplyPixelButtonChrome(image);

            var text = CreateText("Label", image.transform, Vector2.zero, Vector2.one,
                new Vector2(34f, 6f), new Vector2(-34f, -6f), font, fontSize, FontStyle.Bold, LightText, TextAnchor.MiddleLeft);
            text.text = label;
            return button;
        }

        private static void ApplyPixelButtonChrome(Image image)
        {
            var outline = image.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(SeaGreen.r, SeaGreen.g, SeaGreen.b, 0.72f);
            outline.effectDistance = new Vector2(2f, -2f);
            outline.useGraphicAlpha = true;

            var accent = CreateImage("PixelAccent", image.transform, Vector2.zero, new Vector2(0f, 1f),
                new Vector2(0f, 0f), new Vector2(7f, 0f), SeaGreen);
            accent.raycastTarget = false;
            var topRule = CreateImage("PixelTopRule", image.transform, new Vector2(0f, 1f), Vector2.one,
                new Vector2(0f, -3f), Vector2.zero, new Color(SeaGreen.r, SeaGreen.g, SeaGreen.b, 0.72f));
            topRule.raycastTarget = false;
            var lowerShadow = CreateImage("PixelLowerShadow", image.transform, Vector2.zero, new Vector2(1f, 0f),
                new Vector2(10f, -7f), new Vector2(-10f, -3f), new Color(Navy.r, Navy.g, Navy.b, 0.92f));
            lowerShadow.raycastTarget = false;
        }

        private readonly struct ChapterSpec
        {
            public readonly int number;
            public readonly string chapterId;
            public readonly string sceneName;
            public readonly string flowPath;
            public readonly Color fallbackColor;

            public ChapterSpec(int number, string chapterId, string sceneName, string flowPath)
            {
                this.number = number;
                this.chapterId = chapterId;
                this.sceneName = sceneName;
                this.flowPath = flowPath;
                fallbackColor = number switch
                {
                    1 => new Color32(28, 82, 92, 255),
                    2 => new Color32(28, 55, 78, 255),
                    3 => new Color32(25, 73, 77, 255),
                    _ => new Color32(67, 47, 70, 255)
                };
            }
        }
    }
}
