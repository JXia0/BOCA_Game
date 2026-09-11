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
        public NarrativeBeatFile[] beats;
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
    }
}
