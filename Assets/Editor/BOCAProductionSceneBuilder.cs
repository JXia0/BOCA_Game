using System;
using System.Collections.Generic;
using BOCA.Art;
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
            var buildScenes = new List<EditorBuildSettingsScene>();

            foreach (var chapter in Chapters)
            {
                var scenePath = BuildScene(chapter, sharedCatalog);
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = buildScenes.ToArray();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("BOCA_PRODUCTION_SCENES_BUILT: 4");
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
            var background = CreateImage("SceneArtwork", canvasObject.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, chapter.fallbackColor);
            var backgroundKey = CreateText("BackgroundArtKey", background.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-500f, -45f), new Vector2(500f, 45f), font, 28, FontStyle.Bold, new Color(1f, 1f, 1f, 0.52f), TextAnchor.MiddleCenter);

            var topBar = CreateImage("ChapterHeader", canvasObject.transform, new Vector2(0f, 0.91f), Vector2.one, Vector2.zero, Vector2.zero, new Color(Navy.r, Navy.g, Navy.b, 0.94f));
            var chapterTitle = CreateText("ChapterTitle", topBar.transform, Vector2.zero, Vector2.one,
                new Vector2(42f, 8f), new Vector2(-620f, -8f), font, 28, FontStyle.Bold, LightText, TextAnchor.MiddleLeft);
            var sceneLabel = CreateText("SceneLabel", topBar.transform, Vector2.zero, Vector2.one,
                new Vector2(1180f, 8f), new Vector2(-42f, -8f), font, 20, FontStyle.Bold, MutedText, TextAnchor.MiddleRight);

            var dialoguePanel = CreateImage("DialoguePanel", canvasObject.transform, Vector2.zero, new Vector2(1f, 0.35f),
                Vector2.zero, Vector2.zero, new Color(Navy.r, Navy.g, Navy.b, 0.98f));
            CreateImage("DialogueTopBorder", dialoguePanel.transform, new Vector2(0f, 1f), Vector2.one,
                new Vector2(0f, -6f), Vector2.zero, SeaGreen);

            var portraitFrame = CreateImage("PortraitFrame", dialoguePanel.transform, Vector2.zero, Vector2.zero,
                new Vector2(30f, 24f), new Vector2(318f, 335f), DeepBlue);
            var portrait = CreateImage("PortraitArtwork", portraitFrame.transform, Vector2.zero, Vector2.one,
                new Vector2(6f, 6f), new Vector2(-6f, -6f), new Color32(31, 59, 68, 255));
            portrait.preserveAspect = true;
            var portraitKey = CreateText("PortraitArtKey", portraitFrame.transform, Vector2.zero, Vector2.one,
                new Vector2(18f, 18f), new Vector2(-18f, -18f), font, 16, FontStyle.Bold, new Color(1f, 1f, 1f, 0.5f), TextAnchor.MiddleCenter);

            var speaker = CreateText("Speaker", dialoguePanel.transform, Vector2.zero, Vector2.one,
                new Vector2(360f, 292f), new Vector2(-250f, -20f), font, 25, FontStyle.Bold, SeaGreen, TextAnchor.MiddleLeft);
            var body = CreateText("DialogueBody", dialoguePanel.transform, Vector2.zero, Vector2.one,
                new Vector2(360f, 110f), new Vector2(-120f, -68f), font, 29, FontStyle.Normal, LightText, TextAnchor.UpperLeft);
            body.resizeTextForBestFit = true;
            body.resizeTextMinSize = 19;
            body.resizeTextMaxSize = 29;

            var continueButton = CreateButton("ContinueButton", dialoguePanel.transform, new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(-270f, 24f), new Vector2(-35f, 92f), font, 23, "CONTINUE");

            var choiceButtons = new Button[3];
            var choiceLabels = new Text[3];
            for (var i = 0; i < choiceButtons.Length; i++)
            {
                var bottom = 20f + (choiceButtons.Length - 1 - i) * 62f;
                choiceButtons[i] = CreateButton($"ChoiceButton{i + 1}", dialoguePanel.transform, Vector2.zero, new Vector2(1f, 0f),
                    new Vector2(360f, bottom), new Vector2(-120f, bottom + 54f), font, 20, string.Empty);
                choiceLabels[i] = choiceButtons[i].GetComponentInChildren<Text>();
                choiceButtons[i].gameObject.SetActive(false);
            }

            var view = canvasObject.AddComponent<ChapterView>();
            view.Configure(chapterTitle, sceneLabel, speaker, body, backgroundKey, portraitKey, background, portrait,
                continueButton, continueButton.GetComponentInChildren<Text>(), choiceButtons, choiceLabels);

            var controllerObject = new GameObject("ChapterSceneController");
            var controller = controllerObject.AddComponent<ChapterSceneController>();
            var flowAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(chapter.flowPath);
            var chapterCatalog = BOCAArtCatalogBuilder.LoadChapterCatalog(chapter.number);
            controller.Configure(chapter.chapterId, flowAsset, new[] { chapterCatalog, sharedCatalog }, view);

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

            var text = CreateText("Label", image.transform, Vector2.zero, Vector2.one,
                new Vector2(18f, 6f), new Vector2(-18f, -6f), font, fontSize, FontStyle.Bold, LightText, TextAnchor.MiddleCenter);
            text.text = label;
            return button;
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
