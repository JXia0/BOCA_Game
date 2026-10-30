using System;
using UnityEngine;

namespace BOCA.Core
{
    /// <summary>
    /// Stores chapter completion separately from narrative content so that script
    /// revisions never invalidate the player's campaign progress.
    /// </summary>
    public static class GameProgressService
    {
        public const int ChapterCount = 4;

        private const string CompletedMaskKey = "boca.progress.completed-mask";

        public static event Action ProgressChanged;

        public static bool IsChapterUnlocked(int chapterNumber)
        {
            ValidateChapterNumber(chapterNumber);
            return chapterNumber == 1 || IsChapterComplete(chapterNumber - 1);
        }

        public static bool IsChapterComplete(int chapterNumber)
        {
            ValidateChapterNumber(chapterNumber);
            var mask = PlayerPrefs.GetInt(CompletedMaskKey, 0);
            return (mask & ChapterBit(chapterNumber)) != 0;
        }

        public static void MarkChapterComplete(int chapterNumber)
        {
            ValidateChapterNumber(chapterNumber);
            var currentMask = PlayerPrefs.GetInt(CompletedMaskKey, 0);
            var updatedMask = currentMask | ChapterBit(chapterNumber);
            if (updatedMask == currentMask)
            {
                return;
            }

            PlayerPrefs.SetInt(CompletedMaskKey, updatedMask);
            PlayerPrefs.Save();
            ProgressChanged?.Invoke();
        }

        public static void ResetProgress()
        {
            if (!PlayerPrefs.HasKey(CompletedMaskKey))
            {
                return;
            }

            PlayerPrefs.DeleteKey(CompletedMaskKey);
            PlayerPrefs.Save();
            ProgressChanged?.Invoke();
        }

        private static int ChapterBit(int chapterNumber)
        {
            return 1 << (chapterNumber - 1);
        }

        private static void ValidateChapterNumber(int chapterNumber)
        {
            if (chapterNumber < 1 || chapterNumber > ChapterCount)
            {
                throw new ArgumentOutOfRangeException(nameof(chapterNumber), chapterNumber,
                    $"Chapter number must be between 1 and {ChapterCount}.");
            }
        }
    }
}
