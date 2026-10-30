using System;

namespace BOCA.Narrative
{
    [Serializable]
    public sealed class ChapterFlowFile
    {
        public string chapterId;
        public string localizationTable;
        public string titleKey;
        public string startBeatId;
        public ChapterReviewDimensionFile[] reviewDimensions;
        public NarrativeBeatFile[] beats;
    }

    [Serializable]
    public sealed class ChapterReviewDimensionFile
    {
        public string id;
        public string labelKey;
        public int startingScore;
    }

    [Serializable]
    public sealed class NarrativeBeatFile
    {
        public string id;
        public string sceneLabelKey;
        public string speakerKey;
        public string textKey;
        public string backgroundKey;
        public string portraitKey;
        public string nextBeatId;
        public string nextBeatIdRouteA;
        public string nextBeatIdRouteB;
        public string requiredFlag;
        public string interactionMode;
        public string promptKey;
        public string submitLabelKey;
        public int minimumSelections;
        public int maximumSelections;
        public string[] expectedChoiceIds;
        public NarrativeScoreEffectFile[] successEffects;
        public NarrativeScoreEffectFile[] failureEffects;
        public NarrativeChoiceFile[] choices;
    }

    [Serializable]
    public sealed class NarrativeChoiceFile
    {
        public string id;
        public string labelKey;
        public string nextBeatId;
        public string setRoute;
        public string setFlag;
        public string[] setFlags;
        public string[] evidenceIds;
        public string feedbackKey;
        public NarrativeScoreEffectFile[] effects;
    }

    [Serializable]
    public sealed class NarrativeScoreEffectFile
    {
        public string dimensionId;
        public int delta;
    }
}
