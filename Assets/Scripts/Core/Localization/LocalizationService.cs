using System;
using System.Collections.Generic;
using UnityEngine;

namespace BOCA.Core.Localization
{
    /// <summary>
    /// Small, dependency-free localization service for narrative and UI text.
    /// The future title-screen settings menu only needs to call SetLanguage.
    /// Missing Chinese entries deliberately fall back to English while translation is in progress.
    /// </summary>
    public static class LocalizationService
    {
        private const string LanguagePreferenceKey = "boca.settings.language";
        private const string EnglishLocale = "en-US";
        private const string ChineseLocale = "zh-CN";

        private static readonly Dictionary<string, Dictionary<string, string>> TableCache =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

        private static bool initialized;
        private static GameLanguage currentLanguage = GameLanguage.English;

        public static event Action<GameLanguage> LanguageChanged;

        public static GameLanguage CurrentLanguage
        {
            get
            {
                EnsureInitialized();
                return currentLanguage;
            }
        }

        public static string CurrentLocale => LocaleFor(CurrentLanguage);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad()
        {
            EnsureInitialized();
        }

        public static void SetLanguage(GameLanguage language, bool persist = true)
        {
            EnsureInitialized();

            if (currentLanguage == language)
            {
                return;
            }

            currentLanguage = language;
            if (persist)
            {
                PlayerPrefs.SetInt(LanguagePreferenceKey, (int)language);
                PlayerPrefs.Save();
            }

            LanguageChanged?.Invoke(language);
        }

        public static string Get(string table, string key, string fallback = null)
        {
            EnsureInitialized();

            if (TryGet(CurrentLocale, table, key, out var value))
            {
                return value;
            }

            if (!string.Equals(CurrentLocale, EnglishLocale, StringComparison.Ordinal) &&
                TryGet(EnglishLocale, table, key, out value))
            {
                return value;
            }

            return string.IsNullOrEmpty(fallback) ? key : fallback;
        }

        public static bool HasEnglishKey(string table, string key)
        {
            return TryGet(EnglishLocale, table, key, out _);
        }

        public static void ClearCache()
        {
            TableCache.Clear();
        }

        private static void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            var savedValue = PlayerPrefs.GetInt(LanguagePreferenceKey, (int)GameLanguage.English);
            currentLanguage = Enum.IsDefined(typeof(GameLanguage), savedValue)
                ? (GameLanguage)savedValue
                : GameLanguage.English;
            initialized = true;
        }

        private static bool TryGet(string locale, string table, string key, out string value)
        {
            var entries = LoadTable(locale, table);
            return entries.TryGetValue(key, out value);
        }

        private static Dictionary<string, string> LoadTable(string locale, string table)
        {
            var cacheKey = locale + "/" + table;
            if (TableCache.TryGetValue(cacheKey, out var cached))
            {
                return cached;
            }

            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            var textAsset = Resources.Load<TextAsset>($"BOCA/Localization/{locale}/{table}");
            if (textAsset != null)
            {
                var file = JsonUtility.FromJson<LocalizationTableFile>(textAsset.text);
                if (file?.entries != null)
                {
                    foreach (var entry in file.entries)
                    {
                        if (entry != null && !string.IsNullOrWhiteSpace(entry.key))
                        {
                            result[entry.key] = entry.value ?? string.Empty;
                        }
                    }
                }
            }

            TableCache[cacheKey] = result;
            return result;
        }

        private static string LocaleFor(GameLanguage language)
        {
            return language == GameLanguage.ChineseSimplified ? ChineseLocale : EnglishLocale;
        }

        [Serializable]
        private sealed class LocalizationTableFile
        {
            public LocalizationEntry[] entries;
        }

        [Serializable]
        private sealed class LocalizationEntry
        {
            public string key;
            public string value;
        }
    }
}
