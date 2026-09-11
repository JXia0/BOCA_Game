using System;
using System.Collections.Generic;

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

        public NarrativeBeatFile CurrentBeat { get; private set; }
        public string SelectedRoute { get; private set; }
        public IReadOnlyCollection<string> Flags => flags;
        public bool IsComplete => CurrentBeat == null;

        public NarrativeSession(ChapterFlowFile flow)
        {
            this.flow = flow ?? throw new ArgumentNullException(nameof(flow));
            beats = new Dictionary<string, NarrativeBeatFile>(StringComparer.Ordinal);

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
            if (CurrentBeat?.choices == null || choiceIndex < 0 || choiceIndex >= CurrentBeat.choices.Length)
            {
                return false;
            }

            var choice = CurrentBeat.choices[choiceIndex];
            if (!string.IsNullOrWhiteSpace(choice.setRoute))
            {
                SelectedRoute = choice.setRoute;
            }

            if (!string.IsNullOrWhiteSpace(choice.setFlag))
            {
                flags.Add(choice.setFlag);
            }

            CurrentBeat = ResolveVisible(choice.nextBeatId);
            return true;
        }

        public static bool HasChoices(NarrativeBeatFile beat)
        {
            return beat?.choices != null && beat.choices.Length > 0;
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
}
