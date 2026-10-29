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

        public void Render(
            string title,
            string scene,
            string speaker,
            string body,
            string backgroundKey,
            string portraitKey,
            Sprite background,
            Sprite portrait,
            string[] choices,
            string continueLabel)
        {
            chapterTitle.text = title;
            sceneLabel.text = scene;
            speakerLabel.text = speaker;
            dialogueBody.text = body;
            continueButtonLabel.text = continueLabel;

            ApplySprite(backgroundImage, background, new Color(0.08f, 0.16f, 0.21f, 1f), true);
            ApplySprite(portraitImage, portrait, new Color(0.16f, 0.27f, 0.31f, 1f), true);
            var hasPortraitSlot = !string.IsNullOrWhiteSpace(portraitKey);
            portraitImage.transform.parent.gameObject.SetActive(hasPortraitSlot);
            SetLeftOffset(speakerLabel.rectTransform, hasPortraitSlot ? 360f : 60f);
            SetLeftOffset(dialogueBody.rectTransform, hasPortraitSlot ? 360f : 60f);
            backgroundKeyLabel.text = background == null && !string.IsNullOrWhiteSpace(backgroundKey)
                ? $"ART PENDING  {backgroundKey}"
                : string.Empty;
            portraitKeyLabel.text = portrait == null && !string.IsNullOrWhiteSpace(portraitKey)
                ? $"PORTRAIT\n{portraitKey}"
                : string.Empty;

            var choiceCount = choices?.Length ?? 0;
            var bodyOffsets = dialogueBody.rectTransform.offsetMin;
            bodyOffsets.y = choiceCount > 0 ? 205f : 110f;
            dialogueBody.rectTransform.offsetMin = bodyOffsets;
            for (var i = 0; i < choiceButtons.Length; i++)
            {
                var active = i < choiceCount;
                choiceButtons[i].gameObject.SetActive(active);
                if (active)
                {
                    choiceButtonLabels[i].text = choices[i];
                }
            }

            continueButton.gameObject.SetActive(choiceCount == 0);
        }

        public void ShowComplete(string title, string body)
        {
            chapterTitle.text = title;
            sceneLabel.text = string.Empty;
            speakerLabel.text = string.Empty;
            dialogueBody.text = body;
            backgroundKeyLabel.text = string.Empty;
            portraitKeyLabel.text = string.Empty;
            ApplySprite(backgroundImage, null, new Color(0.035f, 0.09f, 0.12f, 1f), true);
            ApplySprite(portraitImage, null, new Color(0.10f, 0.16f, 0.18f, 1f), true);
            portraitImage.transform.parent.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);

            foreach (var button in choiceButtons)
            {
                button.gameObject.SetActive(false);
            }
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
}
