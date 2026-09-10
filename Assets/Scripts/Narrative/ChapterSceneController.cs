using BOCA.Art;
using BOCA.Core.Localization;
using BOCA.UI;
using UnityEngine;

namespace BOCA.Narrative
{
    public sealed class ChapterSceneController : MonoBehaviour
    {
        [SerializeField] private string chapterId;
        [SerializeField] private TextAsset flowAsset;
        [SerializeField] private ArtCatalog[] artCatalogs;
        [SerializeField] private ChapterView view;

        private ChapterFlowFile flow;
        private NarrativeSession session;

        public string ChapterId => chapterId;
        public TextAsset FlowAsset => flowAsset;

        public void Configure(string id, TextAsset chapterFlow, ArtCatalog[] catalogs, ChapterView chapterView)
        {
            chapterId = id;
            flowAsset = chapterFlow;
            artCatalogs = catalogs;
            view = chapterView;
        }

        private void OnEnable()
        {
            LocalizationService.LanguageChanged += HandleLanguageChanged;
        }

        private void OnDisable()
        {
            LocalizationService.LanguageChanged -= HandleLanguageChanged;
        }

        private void Start()
        {
            if (flowAsset == null || view == null)
            {
                Debug.LogError($"{nameof(ChapterSceneController)} is not fully configured in scene '{gameObject.scene.name}'.", this);
                enabled = false;
                return;
            }

            flow = JsonUtility.FromJson<ChapterFlowFile>(flowAsset.text);
            session = new NarrativeSession(flow);
            view.Bind(HandleContinue, HandleChoice);
            RenderCurrentBeat();
        }

        private void Update()
        {
            if (session?.CurrentBeat != null && !NarrativeSession.HasChoices(session.CurrentBeat) &&
                (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                HandleContinue();
            }
        }

        private void HandleContinue()
        {
            if (session == null)
            {
                return;
            }

            session.Advance();
            RenderCurrentBeat();
        }

        private void HandleChoice(int choiceIndex)
        {
            if (session == null || !session.Choose(choiceIndex))
            {
                return;
            }

            RenderCurrentBeat();
        }

        private void HandleLanguageChanged(GameLanguage _)
        {
            if (session != null)
            {
                RenderCurrentBeat();
            }
        }

        public bool AdvanceForAutomatedQa(string preferredRoute)
        {
            if (!Debug.isDebugBuild || session == null || session.IsComplete)
            {
                return false;
            }

            var beat = session.CurrentBeat;
            if (NarrativeSession.HasChoices(beat))
            {
                var selectedIndex = 0;
                for (var i = 0; i < beat.choices.Length; i++)
                {
                    if (string.Equals(beat.choices[i].setRoute, preferredRoute, System.StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                session.Choose(selectedIndex);
            }
            else
            {
                session.Advance();
            }

            RenderCurrentBeat();
            return true;
        }

        private void RenderCurrentBeat()
        {
            if (session == null || session.IsComplete)
            {
                view.ShowComplete(
                    Localize("shared.chapter_complete", "Chapter complete"),
                    Localize("shared.chapter_complete_body", "This chapter foundation is ready for the next production pass."));
                return;
            }

            var beat = session.CurrentBeat;
            var choices = beat.choices;
            var choiceLabels = choices == null ? null : new string[choices.Length];
            if (choices != null)
            {
                for (var i = 0; i < choices.Length; i++)
                {
                    choiceLabels[i] = Localize(choices[i].labelKey, choices[i].labelKey);
                }
            }

            view.Render(
                Localize(flow.titleKey, flow.chapterId),
                Localize(beat.sceneLabelKey, string.Empty),
                Localize(beat.speakerKey, string.Empty),
                Localize(beat.textKey, beat.textKey),
                beat.backgroundKey,
                beat.portraitKey,
                ResolveSprite(beat.backgroundKey),
                ResolveSprite(beat.portraitKey),
                choiceLabels,
                Localize("shared.continue", "Continue"));
        }

        private string Localize(string key, string fallback)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return fallback;
            }

            var table = key.StartsWith("shared.", System.StringComparison.Ordinal)
                ? "shared"
                : flow.localizationTable;
            return LocalizationService.Get(table, key, fallback);
        }

        private Sprite ResolveSprite(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || artCatalogs == null)
            {
                return null;
            }

            foreach (var catalog in artCatalogs)
            {
                var sprite = catalog == null ? null : catalog.Resolve(key);
                if (sprite != null)
                {
                    return sprite;
                }
            }

            return null;
        }
    }
}
