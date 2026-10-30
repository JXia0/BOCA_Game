using BOCA.Art;
using BOCA.Core;
using BOCA.Core.Localization;
using BOCA.UI;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BOCA.Narrative
{
    public sealed class ChapterSceneController : MonoBehaviour
    {
        [SerializeField] private string chapterId;
        [SerializeField] private int chapterNumber;
        [SerializeField] private TextAsset flowAsset;
        [SerializeField] private ArtCatalog[] artCatalogs;
        [SerializeField] private ChapterView view;

        private ChapterFlowFile flow;
        private NarrativeSession session;
        private readonly List<int> pendingSelections = new();
        private bool completionRecorded;

        public string ChapterId => chapterId;
        public TextAsset FlowAsset => flowAsset;

        public void Configure(int number, string id, TextAsset chapterFlow, ArtCatalog[] catalogs, ChapterView chapterView)
        {
            chapterNumber = number;
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
            view.Bind(HandleContinue, HandleChoice, SceneNavigator.LoadMainMenu, SceneNavigator.RestartCurrentScene);
            RenderCurrentBeat();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneNavigator.LoadMainMenu();
                return;
            }

            if (session?.CurrentBeat == null)
            {
                return;
            }

            var beat = session.CurrentBeat;
            if (NarrativeSession.HasChoices(beat))
            {
                for (var index = 0; index < Mathf.Min(6, beat.choices.Length); index++)
                {
                    var numberKey = (KeyCode)((int)KeyCode.Alpha1 + index);
                    var keypadKey = (KeyCode)((int)KeyCode.Keypad1 + index);
                    if (Input.GetKeyDown(numberKey) || Input.GetKeyDown(keypadKey))
                    {
                        HandleChoice(index);
                    }
                }

                if (NarrativeSession.RequiresSubmission(beat) && Input.GetKeyDown(KeyCode.Return))
                {
                    HandleContinue();
                }

                return;
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
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

            if (session.CurrentBeat != null && NarrativeSession.RequiresSubmission(session.CurrentBeat))
            {
                if (!session.CommitSelections(pendingSelections))
                {
                    return;
                }

                pendingSelections.Clear();
            }
            else
            {
                session.Advance();
            }

            RenderCurrentBeat();
        }

        private void HandleChoice(int choiceIndex)
        {
            if (session?.CurrentBeat == null)
            {
                return;
            }

            var beat = session.CurrentBeat;
            if (NarrativeSession.RequiresSubmission(beat))
            {
                TogglePendingSelection(beat, choiceIndex);
                RenderCurrentBeat(false);
                return;
            }

            if (!session.Choose(choiceIndex))
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
                var selectedIndex = string.Equals(preferredRoute, "B", System.StringComparison.OrdinalIgnoreCase) &&
                                    beat.choices.Length > 1
                    ? 1
                    : 0;
                for (var i = 0; i < beat.choices.Length; i++)
                {
                    if (string.Equals(beat.choices[i].setRoute, preferredRoute, System.StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                if (NarrativeSession.RequiresSubmission(beat))
                {
                    var selections = BuildAutomatedSelections(beat, selectedIndex);
                    session.CommitSelections(selections);
                }
                else
                {
                    session.Choose(selectedIndex);
                }
            }
            else
            {
                session.Advance();
            }

            RenderCurrentBeat();
            return true;
        }

        private void RenderCurrentBeat(bool resetPendingSelections = true)
        {
            if (session == null || session.IsComplete)
            {
                if (!completionRecorded && chapterNumber >= 1 && chapterNumber <= GameProgressService.ChapterCount)
                {
                    GameProgressService.MarkChapterComplete(chapterNumber);
                    completionRecorded = true;
                }

                view.ShowComplete(
                    Localize("shared.chapter_complete", "Chapter complete"),
                    BuildCompletionBody(),
                    Localize("shared.replay", "Replay chapter"),
                    Localize("shared.chapter_select", "Chapter select"),
                    Localize("shared.menu", "Menu"));
                return;
            }

            var beat = session.CurrentBeat;
            if (resetPendingSelections)
            {
                pendingSelections.Clear();
            }

            var choices = beat.choices;
            var choiceLabels = choices == null ? null : new string[choices.Length];
            var selected = choices == null ? null : new bool[choices.Length];
            var ranks = choices == null ? null : new int[choices.Length];
            if (choices != null)
            {
                for (var i = 0; i < choices.Length; i++)
                {
                    choiceLabels[i] = Localize(choices[i].labelKey, choices[i].labelKey);
                    var rank = pendingSelections.IndexOf(i);
                    selected[i] = rank >= 0;
                    ranks[i] = rank + 1;
                }
            }

            var requiresSubmission = NarrativeSession.RequiresSubmission(beat);
            var minimum = Mathf.Max(1, beat.minimumSelections);
            view.Render(new ChapterRenderModel
            {
                title = Localize(flow.titleKey, flow.chapterId),
                scene = Localize(beat.sceneLabelKey, string.Empty),
                speaker = Localize(beat.speakerKey, string.Empty),
                body = Localize(beat.textKey, beat.textKey),
                backgroundKey = beat.backgroundKey,
                portraitKey = beat.portraitKey,
                background = ResolveSprite(beat.backgroundKey),
                portrait = ResolveSprite(beat.portraitKey),
                prompt = Localize(beat.promptKey, string.Empty),
                caseStatus = BuildCaseStatus(),
                interactionMode = beat.interactionMode,
                choiceLabels = choiceLabels,
                choiceSelected = selected,
                choiceRanks = ranks,
                actionLabel = requiresSubmission
                    ? Localize(beat.submitLabelKey, Localize("shared.submit", "Submit"))
                    : Localize("shared.continue", "Continue"),
                actionVisible = !NarrativeSession.HasChoices(beat) || requiresSubmission,
                actionEnabled = !requiresSubmission || pendingSelections.Count >= minimum,
                menuLabel = Localize("shared.menu", "Menu")
            });
        }

        private void TogglePendingSelection(NarrativeBeatFile beat, int choiceIndex)
        {
            if (beat.choices == null || choiceIndex < 0 || choiceIndex >= beat.choices.Length)
            {
                return;
            }

            var existingIndex = pendingSelections.IndexOf(choiceIndex);
            if (existingIndex >= 0)
            {
                pendingSelections.RemoveAt(existingIndex);
                return;
            }

            var maximum = beat.maximumSelections > 0 ? beat.maximumSelections : beat.choices.Length;
            if (pendingSelections.Count < maximum)
            {
                pendingSelections.Add(choiceIndex);
            }
        }

        private static List<int> BuildAutomatedSelections(NarrativeBeatFile beat, int fallbackIndex)
        {
            var result = new List<int>();
            if (beat.expectedChoiceIds != null)
            {
                foreach (var expectedId in beat.expectedChoiceIds)
                {
                    for (var index = 0; index < beat.choices.Length; index++)
                    {
                        if (string.Equals(beat.choices[index].id, expectedId, System.StringComparison.Ordinal))
                        {
                            result.Add(index);
                            break;
                        }
                    }
                }
            }

            var minimum = Mathf.Max(1, beat.minimumSelections);
            if (result.Count == 0)
            {
                result.Add(Mathf.Clamp(fallbackIndex, 0, beat.choices.Length - 1));
            }

            for (var index = 0; result.Count < minimum && index < beat.choices.Length; index++)
            {
                if (!result.Contains(index))
                {
                    result.Add(index);
                }
            }

            return result;
        }

        private string BuildCaseStatus()
        {
            return string.Format(
                Localize("shared.case_status", "CASE FILE  •  Decisions {0}  •  Evidence {1}"),
                session.Decisions.Count,
                session.Evidence.Count);
        }

        private string BuildCompletionBody()
        {
            if (flow.reviewDimensions == null || flow.reviewDimensions.Length == 0)
            {
                return Localize("shared.chapter_complete_body", "The chapter record has been saved.");
            }

            var builder = new StringBuilder();
            builder.AppendLine(Localize("shared.review_heading", "FINAL RECORD REVIEW"));
            builder.AppendLine();
            foreach (var dimension in flow.reviewDimensions)
            {
                if (dimension == null || string.IsNullOrWhiteSpace(dimension.id))
                {
                    continue;
                }

                var score = session.GetReviewScore(dimension.id);
                var ratingKey = score >= 2
                    ? "shared.review.strong"
                    : score >= 0
                        ? "shared.review.documented"
                        : "shared.review.needs_attention";
                var fallback = score >= 2 ? "Strong" : score >= 0 ? "Documented" : "Needs attention";
                builder.Append("• ")
                    .Append(Localize(dimension.labelKey, dimension.id))
                    .Append(": ")
                    .AppendLine(Localize(ratingKey, fallback));
            }

            builder.AppendLine();
            builder.AppendFormat(
                Localize("shared.review_totals", "{0} decisions recorded • {1} evidence items preserved"),
                session.Decisions.Count,
                session.Evidence.Count);
            return builder.ToString();
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
