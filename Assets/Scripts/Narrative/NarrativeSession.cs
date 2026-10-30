using System;
using System.Collections.Generic;
using System.Linq;

namespace BOCA.Narrative
{
    /// <summary>
    /// Pure narrative state machine. It is intentionally independent of Unity UI so
    /// dialogue content can change without rebuilding the chapter scene hierarchy.
    /// </summary>
    public sealed class NarrativeSession
    {
        private readonly ChapterFlowFile flow;
        private readonly Dictionary<string, NarrativeBeatFile> beats;
        private readonly HashSet<string> flags = new(StringComparer.Ordinal);
        private readonly HashSet<string> evidence = new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> reviewScores = new(StringComparer.Ordinal);
        private readonly List<NarrativeDecision> decisions = new();

        public NarrativeBeatFile CurrentBeat { get; private set; }
        public string SelectedRoute { get; private set; }
        public IReadOnlyCollection<string> Flags => flags;
        public IReadOnlyCollection<string> Evidence => evidence;
        public IReadOnlyDictionary<string, int> ReviewScores => reviewScores;
        public IReadOnlyList<NarrativeDecision> Decisions => decisions;
        public bool IsComplete => CurrentBeat == null;

        public NarrativeSession(ChapterFlowFile flow)
        {
            this.flow = flow ?? throw new ArgumentNullException(nameof(flow));
            beats = new Dictionary<string, NarrativeBeatFile>(StringComparer.Ordinal);

            if (flow.reviewDimensions != null)
            {
                foreach (var dimension in flow.reviewDimensions)
                {
                    if (dimension != null && !string.IsNullOrWhiteSpace(dimension.id))
                    {
                        reviewScores[dimension.id] = dimension.startingScore;
                    }
                }
            }

            if (flow.beats == null || flow.beats.Length == 0)
            {
                throw new InvalidOperationException($"Chapter '{flow.chapterId}' has no narrative beats.");
            }

            foreach (var beat in flow.beats)
            {
                if (beat == null || string.IsNullOrWhiteSpace(beat.id))
                {
                    throw new InvalidOperationException($"Chapter '{flow.chapterId}' contains a beat without an id.");
                }

                if (!beats.TryAdd(beat.id, beat))
                {
                    throw new InvalidOperationException($"Chapter '{flow.chapterId}' contains duplicate beat id '{beat.id}'.");
                }
            }

            CurrentBeat = ResolveVisible(flow.startBeatId);
        }

        public bool Advance()
        {
            if (CurrentBeat == null || HasChoices(CurrentBeat))
            {
                return false;
            }

            var nextBeatId = CurrentBeat.nextBeatId;
            if (string.Equals(SelectedRoute, "A", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(CurrentBeat.nextBeatIdRouteA))
            {
                nextBeatId = CurrentBeat.nextBeatIdRouteA;
            }
            else if (string.Equals(SelectedRoute, "B", StringComparison.OrdinalIgnoreCase) &&
                     !string.IsNullOrWhiteSpace(CurrentBeat.nextBeatIdRouteB))
            {
                nextBeatId = CurrentBeat.nextBeatIdRouteB;
            }

            CurrentBeat = string.IsNullOrWhiteSpace(nextBeatId) ? null : ResolveVisible(nextBeatId);
            return true;
        }

        public bool Choose(int choiceIndex)
        {
            return CommitSelections(new[] { choiceIndex });
        }

        public bool CommitSelections(IReadOnlyList<int> choiceIndices)
        {
            if (CurrentBeat?.choices == null || choiceIndices == null || choiceIndices.Count == 0)
            {
                return false;
            }

            var beat = CurrentBeat;
            var selected = new List<NarrativeChoiceFile>(choiceIndices.Count);
            var uniqueIndices = new HashSet<int>();
            foreach (var choiceIndex in choiceIndices)
            {
                if (choiceIndex < 0 || choiceIndex >= beat.choices.Length || !uniqueIndices.Add(choiceIndex))
                {
                    return false;
                }

                selected.Add(beat.choices[choiceIndex]);
            }

            var minimum = MathfMax(1, beat.minimumSelections);
            var maximum = beat.maximumSelections > 0 ? beat.maximumSelections : beat.choices.Length;
            if (selected.Count < minimum || selected.Count > maximum)
            {
                return false;
            }

            foreach (var choice in selected)
            {
                ApplyChoice(choice);
            }

            var matchedExpected = MatchesExpected(beat, selected);
            ApplyEffects(matchedExpected ? beat.successEffects : beat.failureEffects);
            decisions.Add(new NarrativeDecision(
                beat.id,
                selected.Where(choice => choice != null).Select(choice => choice.id).ToArray(),
                matchedExpected));

            var nextBeatId = selected.Count == 1 && !string.IsNullOrWhiteSpace(selected[0].nextBeatId)
                ? selected[0].nextBeatId
                : NextIdForCurrentRoute(beat);
            CurrentBeat = string.IsNullOrWhiteSpace(nextBeatId) ? null : ResolveVisible(nextBeatId);
            return true;
        }

        public static bool HasChoices(NarrativeBeatFile beat)
        {
            return beat?.choices != null && beat.choices.Length > 0;
        }

        public static bool RequiresSubmission(NarrativeBeatFile beat)
        {
            return beat != null &&
                   (string.Equals(beat.interactionMode, "checklist", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(beat.interactionMode, "sequence", StringComparison.OrdinalIgnoreCase));
        }

        public int GetReviewScore(string dimensionId)
        {
            return !string.IsNullOrWhiteSpace(dimensionId) && reviewScores.TryGetValue(dimensionId, out var score)
                ? score
                : 0;
        }

        private void ApplyChoice(NarrativeChoiceFile choice)
        {
            if (choice == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(choice.setRoute))
            {
                SelectedRoute = choice.setRoute;
            }

            AddFlag(choice.setFlag);
            if (choice.setFlags != null)
            {
                foreach (var flag in choice.setFlags)
                {
                    AddFlag(flag);
                }
            }

            if (choice.evidenceIds != null)
            {
                foreach (var evidenceId in choice.evidenceIds)
                {
                    if (!string.IsNullOrWhiteSpace(evidenceId))
                    {
                        evidence.Add(evidenceId);
                    }
                }
            }

            ApplyEffects(choice.effects);
        }

        private void AddFlag(string flag)
        {
            if (!string.IsNullOrWhiteSpace(flag))
            {
                flags.Add(flag);
            }
        }

        private void ApplyEffects(NarrativeScoreEffectFile[] effects)
        {
            if (effects == null)
            {
                return;
            }

            foreach (var effect in effects)
            {
                if (effect == null || string.IsNullOrWhiteSpace(effect.dimensionId))
                {
                    continue;
                }

                reviewScores.TryGetValue(effect.dimensionId, out var current);
                reviewScores[effect.dimensionId] = current + effect.delta;
            }
        }

        private static bool MatchesExpected(NarrativeBeatFile beat, IReadOnlyList<NarrativeChoiceFile> selected)
        {
            if (beat.expectedChoiceIds == null || beat.expectedChoiceIds.Length == 0)
            {
                return true;
            }

            var selectedIds = selected.Select(choice => choice?.id ?? string.Empty).ToArray();
            if (string.Equals(beat.interactionMode, "sequence", StringComparison.OrdinalIgnoreCase))
            {
                return selectedIds.SequenceEqual(beat.expectedChoiceIds, StringComparer.Ordinal);
            }

            return selectedIds.Length == beat.expectedChoiceIds.Length &&
                   new HashSet<string>(selectedIds, StringComparer.Ordinal).SetEquals(beat.expectedChoiceIds);
        }

        private static int MathfMax(int left, int right)
        {
            return left > right ? left : right;
        }

        private NarrativeBeatFile Resolve(string beatId)
        {
            if (string.IsNullOrWhiteSpace(beatId) || !beats.TryGetValue(beatId, out var beat))
            {
                throw new InvalidOperationException($"Chapter '{flow.chapterId}' references missing beat '{beatId}'.");
            }

            return beat;
        }

        private NarrativeBeatFile ResolveVisible(string beatId)
        {
            var candidateId = beatId;
            for (var guard = 0; guard <= beats.Count; guard++)
            {
                var beat = Resolve(candidateId);
                if (string.IsNullOrWhiteSpace(beat.requiredFlag) || flags.Contains(beat.requiredFlag))
                {
                    return beat;
                }

                candidateId = NextIdForCurrentRoute(beat);
                if (string.IsNullOrWhiteSpace(candidateId))
                {
                    return null;
                }
            }

            throw new InvalidOperationException($"Chapter '{flow.chapterId}' exceeded the conditional-beat guard. Check for a cycle among skipped beats.");
        }

        private string NextIdForCurrentRoute(NarrativeBeatFile beat)
        {
            if (string.Equals(SelectedRoute, "A", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(beat.nextBeatIdRouteA))
            {
                return beat.nextBeatIdRouteA;
            }

            if (string.Equals(SelectedRoute, "B", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(beat.nextBeatIdRouteB))
            {
                return beat.nextBeatIdRouteB;
            }

            return beat.nextBeatId;
        }
    }

    public sealed class NarrativeDecision
    {
        public NarrativeDecision(string beatId, string[] choiceIds, bool matchedExpected)
        {
            BeatId = beatId;
            ChoiceIds = choiceIds ?? Array.Empty<string>();
            MatchedExpected = matchedExpected;
        }

        public string BeatId { get; }
        public IReadOnlyList<string> ChoiceIds { get; }
        public bool MatchedExpected { get; }
    }
}
