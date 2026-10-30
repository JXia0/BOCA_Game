using System;
using UnityEngine;
using UnityEngine.UI;

namespace BOCA.UI
{
    public sealed class ChapterView : MonoBehaviour
    {
        [SerializeField] private Text chapterTitle;
        [SerializeField] private Text sceneLabel;
        [SerializeField] private Text speakerLabel;
        [SerializeField] private Text dialogueBody;
        [SerializeField] private Text backgroundKeyLabel;
        [SerializeField] private Text portraitKeyLabel;
        [SerializeField] private Text interactionPrompt;
        [SerializeField] private Text caseStatus;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image portraitImage;
        [SerializeField] private Button continueButton;
        [SerializeField] private Text continueButtonLabel;
        [SerializeField] private Button[] choiceButtons;
        [SerializeField] private Text[] choiceButtonLabels;

        private Action continueAction;
        private Action<int> choiceAction;

        private const float ReferencePixelsPerUnit = 16f;

        public void Configure(
            Text title,
            Text scene,
            Text speaker,
            Text body,
            Text backgroundKey,
            Text portraitKey,
            Text prompt,
            Text status,
            Image background,
            Image portrait,
            Button continueControl,
            Text continueLabel,
            Button[] choices,
            Text[] choiceLabels)
        {
            chapterTitle = title;
            sceneLabel = scene;
            speakerLabel = speaker;
            dialogueBody = body;
            backgroundKeyLabel = backgroundKey;
            portraitKeyLabel = portraitKey;
            interactionPrompt = prompt;
            caseStatus = status;
            backgroundImage = background;
            portraitImage = portrait;
            continueButton = continueControl;
            continueButtonLabel = continueLabel;
            choiceButtons = choices;
            choiceButtonLabels = choiceLabels;
        }

        public void Bind(Action onContinue, Action<int> onChoice)
        {
            continueAction = onContinue;
            choiceAction = onChoice;

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => continueAction?.Invoke());

            for (var i = 0; i < choiceButtons.Length; i++)
            {
                var capturedIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => choiceAction?.Invoke(capturedIndex));
            }
        }

        public void Render(ChapterRenderModel model)
        {
            chapterTitle.text = model.title;
            sceneLabel.text = model.scene;
            speakerLabel.text = model.speaker;
            dialogueBody.text = model.body;
            interactionPrompt.text = model.prompt ?? string.Empty;
            caseStatus.text = model.caseStatus ?? string.Empty;
            continueButtonLabel.text = model.actionLabel;
            continueButton.interactable = model.actionEnabled;

            ApplySprite(backgroundImage, model.background, new Color(0.08f, 0.16f, 0.21f, 1f), true);
            ApplySprite(portraitImage, model.portrait, new Color(0.16f, 0.27f, 0.31f, 1f), true);
            var hasPortraitSlot = !string.IsNullOrWhiteSpace(model.portraitKey);
            portraitImage.transform.parent.gameObject.SetActive(hasPortraitSlot);
            SetLeftOffset(speakerLabel.rectTransform, hasPortraitSlot ? 360f : 60f);
            SetLeftOffset(dialogueBody.rectTransform, hasPortraitSlot ? 360f : 60f);
            SetLeftOffset(interactionPrompt.rectTransform, hasPortraitSlot ? 360f : 60f);
            backgroundKeyLabel.text = model.background == null && !string.IsNullOrWhiteSpace(model.backgroundKey)
                ? $"ART PENDING  {model.backgroundKey}"
                : string.Empty;
            portraitKeyLabel.text = model.portrait == null && !string.IsNullOrWhiteSpace(model.portraitKey)
                ? $"PORTRAIT\n{model.portraitKey}"
                : string.Empty;

            var choiceCount = model.choiceLabels?.Length ?? 0;
            var bodyOffsets = dialogueBody.rectTransform.offsetMin;
            bodyOffsets.y = choiceCount > 0 ? 235f : 90f;
            dialogueBody.rectTransform.offsetMin = bodyOffsets;
            for (var i = 0; i < choiceButtons.Length; i++)
            {
                var active = i < choiceCount;
                choiceButtons[i].gameObject.SetActive(active);
                if (active)
                {
                    var selected = model.choiceSelected != null && i < model.choiceSelected.Length && model.choiceSelected[i];
                    var rank = model.choiceRanks != null && i < model.choiceRanks.Length ? model.choiceRanks[i] : 0;
                    choiceButtonLabels[i].text = FormatChoiceLabel(model.choiceLabels[i], selected, rank, model.interactionMode);
                    ApplyChoiceColor(choiceButtons[i], selected);
                }
            }

            continueButton.gameObject.SetActive(model.actionVisible);
        }

        public void ShowComplete(string title, string body)
        {
            chapterTitle.text = title;
            sceneLabel.text = string.Empty;
            speakerLabel.text = string.Empty;
            dialogueBody.text = body;
            backgroundKeyLabel.text = string.Empty;
            portraitKeyLabel.text = string.Empty;
            interactionPrompt.text = string.Empty;
            caseStatus.text = string.Empty;
            ApplySprite(backgroundImage, null, new Color(0.035f, 0.09f, 0.12f, 1f), true);
            ApplySprite(portraitImage, null, new Color(0.10f, 0.16f, 0.18f, 1f), true);
            portraitImage.transform.parent.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);

            foreach (var button in choiceButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        private static string FormatChoiceLabel(string label, bool selected, int rank, string interactionMode)
        {
            if (!selected)
            {
                return label;
            }

            return string.Equals(interactionMode, "sequence", StringComparison.OrdinalIgnoreCase)
                ? $"{rank}.  {label}"
                : $"✓  {label}";
        }

        private static void ApplyChoiceColor(Button button, bool selected)
        {
            var colors = button.colors;
            colors.normalColor = selected ? new Color32(39, 139, 116, 255) : new Color32(35, 72, 84, 255);
            colors.highlightedColor = selected ? new Color32(53, 172, 143, 255) : new Color32(50, 111, 112, 255);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
        }

        private static void ApplySprite(Image image, Sprite sprite, Color fallbackColor, bool useIntegerScale)
        {
            image.sprite = sprite;
            image.color = sprite == null ? fallbackColor : Color.white;
            image.preserveAspect = sprite != null;
            image.type = Image.Type.Simple;
            image.SetNativeSize();

            if (sprite == null)
            {
                StretchToParent(image.rectTransform);
                return;
            }

            FitInsideParent(image.rectTransform, sprite, useIntegerScale);
        }

        private static void FitInsideParent(RectTransform rect, Sprite sprite, bool useIntegerScale)
        {
            if (!(rect.parent is RectTransform parent))
            {
                return;
            }

            var sourceSize = sprite.rect.size * (ReferencePixelsPerUnit / sprite.pixelsPerUnit);
            var available = parent.rect.size;
            if (sourceSize.x <= 0f || sourceSize.y <= 0f || available.x <= 0f || available.y <= 0f)
            {
                return;
            }

            var scale = Mathf.Min(available.x / sourceSize.x, available.y / sourceSize.y);
            if (useIntegerScale && scale >= 1f)
            {
                scale = Mathf.Max(1f, Mathf.Floor(scale));
            }

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = sourceSize * scale;
        }

        private static void StretchToParent(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetLeftOffset(RectTransform rect, float left)
        {
            var offsets = rect.offsetMin;
            offsets.x = left;
            rect.offsetMin = offsets;
        }
    }

    public sealed class ChapterRenderModel
    {
        public string title;
        public string scene;
        public string speaker;
        public string body;
        public string backgroundKey;
        public string portraitKey;
        public Sprite background;
        public Sprite portrait;
        public string prompt;
        public string caseStatus;
        public string interactionMode;
        public string[] choiceLabels;
        public bool[] choiceSelected;
        public int[] choiceRanks;
        public string actionLabel;
        public bool actionVisible;
        public bool actionEnabled;
    }
}
