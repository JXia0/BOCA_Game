using System;
using System.Collections.Generic;
using BOCA.Narrative;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BOCA.Editor
{
    public static class BOCAProductionValidator
    {
        private static readonly ValidationSpec[] Chapters =
        {
            new ValidationSpec("chapter-01", "Assets/Resources/BOCA/Flows/chapter-01.json", "Assets/Scenes/Chapters/Chapter01_OilTide.unity"),
            new ValidationSpec("chapter-02", "Assets/Resources/BOCA/Flows/chapter-02.json", "Assets/Scenes/Chapters/Chapter02_SpermWhaleRescue.unity"),
            new ValidationSpec("chapter-03", "Assets/Resources/BOCA/Flows/chapter-03.json", "Assets/Scenes/Chapters/Chapter03_ReefWarning.unity"),
            new ValidationSpec("chapter-04", "Assets/Resources/BOCA/Flows/chapter-04.json", "Assets/Scenes/Chapters/Chapter04_HawksbillGuzheng.unity")
        };

        [MenuItem("BOCA/Production/Validate Production Foundation")]
        public static void ValidateProject()
        {
            var errors = new List<string>();
            var sharedKeys = LoadLocalizationKeys("shared", errors);

            foreach (var spec in Chapters)
            {
                ValidateChapter(spec, sharedKeys, errors);
            }

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    Debug.LogError("BOCA_VALIDATION: " + error);
                }

                throw new BuildFailedException($"BOCA production validation failed with {errors.Count} error(s).");
            }

            Debug.Log($"BOCA_PRODUCTION_VALIDATION_PASS: chapters={Chapters.Length}");
        }

        private static void ValidateChapter(ValidationSpec spec, HashSet<string> sharedKeys, List<string> errors)
        {
            var flowAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(spec.flowPath);
            if (flowAsset == null)
            {
                errors.Add($"Missing flow asset {spec.flowPath}");
                return;
            }

            var flow = JsonUtility.FromJson<ChapterFlowFile>(flowAsset.text);
            if (flow == null || !string.Equals(flow.chapterId, spec.chapterId, StringComparison.Ordinal))
            {
                errors.Add($"Flow id mismatch in {spec.flowPath}");
                return;
            }

            var chapterKeys = LoadLocalizationKeys(flow.localizationTable, errors);
            var beats = new Dictionary<string, NarrativeBeatFile>(StringComparer.Ordinal);
            var setFlags = new HashSet<string>(StringComparer.Ordinal);
            if (flow.beats == null || flow.beats.Length == 0)
            {
                errors.Add($"{spec.chapterId} has no beats");
                return;
            }

            foreach (var beat in flow.beats)
            {
                if (beat == null || string.IsNullOrWhiteSpace(beat.id))
                {
                    errors.Add($"{spec.chapterId} contains a beat without an id");
                    continue;
                }

                if (!beats.TryAdd(beat.id, beat))
                {
                    errors.Add($"{spec.chapterId} has duplicate beat {beat.id}");
                }

                ValidateKey(spec.chapterId, beat.sceneLabelKey, chapterKeys, sharedKeys, errors);
                ValidateKey(spec.chapterId, beat.speakerKey, chapterKeys, sharedKeys, errors);
                ValidateKey(spec.chapterId, beat.textKey, chapterKeys, sharedKeys, errors);

                if (beat.choices != null && beat.choices.Length > 3)
                {
                    errors.Add($"{beat.id} has {beat.choices.Length} choices; the production UI supports at most 3");
                }

                if (beat.choices != null)
                {
                    foreach (var choice in beat.choices)
                    {
                        ValidateKey(spec.chapterId, choice.labelKey, chapterKeys, sharedKeys, errors);
                        if (!string.IsNullOrWhiteSpace(choice.setFlag))
                        {
                            setFlags.Add(choice.setFlag);
                        }
                    }
                }
            }

            foreach (var beat in flow.beats)
            {
                if (!string.IsNullOrWhiteSpace(beat.requiredFlag) && !setFlags.Contains(beat.requiredFlag))
                {
                    errors.Add($"{beat.id} requires flag {beat.requiredFlag}, but no choice sets it");
                }
            }

            ValidateKey(spec.chapterId, flow.titleKey, chapterKeys, sharedKeys, errors);
            ValidateLink(spec.chapterId, "start", flow.startBeatId, beats, errors);
            foreach (var beat in flow.beats)
            {
                ValidateOptionalLink(spec.chapterId, beat.id, beat.nextBeatId, beats, errors);
                ValidateOptionalLink(spec.chapterId, beat.id + " route A", beat.nextBeatIdRouteA, beats, errors);
                ValidateOptionalLink(spec.chapterId, beat.id + " route B", beat.nextBeatIdRouteB, beats, errors);
                if (beat.choices != null)
                {
                    foreach (var choice in beat.choices)
                    {
                        ValidateLink(spec.chapterId, beat.id + " choice " + choice.id, choice.nextBeatId, beats, errors);
                    }
                }
            }

            ValidateReachability(spec.chapterId, flow.startBeatId, beats, errors);
            ValidateScene(spec, flowAsset, errors);
        }

        private static void ValidateScene(ValidationSpec spec, TextAsset expectedFlow, List<string> errors)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(spec.scenePath) == null)
            {
                errors.Add($"Missing scene {spec.scenePath}");
                return;
            }

            EditorSceneManager.OpenScene(spec.scenePath, OpenSceneMode.Single);
            var controllers = UnityEngine.Object.FindObjectsByType<ChapterSceneController>(FindObjectsInactive.Include);
            if (controllers.Length != 1)
            {
                errors.Add($"{spec.scenePath} needs exactly one ChapterSceneController; found {controllers.Length}");
                return;
            }

            if (!string.Equals(controllers[0].ChapterId, spec.chapterId, StringComparison.Ordinal) || controllers[0].FlowAsset != expectedFlow)
            {
                errors.Add($"{spec.scenePath} controller is linked to the wrong chapter or flow");
            }
        }

        private static HashSet<string> LoadLocalizationKeys(string table, List<string> errors)
        {
            var path = $"Assets/Resources/BOCA/Localization/en-US/{table}.json";
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            var keys = new HashSet<string>(StringComparer.Ordinal);
            if (asset == null)
            {
                errors.Add($"Missing English localization table {path}");
                return keys;
            }

            var file = JsonUtility.FromJson<LocalizationTableFile>(asset.text);
            if (file?.entries == null)
            {
                errors.Add($"Localization table {path} has no entries");
                return keys;
            }

            foreach (var entry in file.entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.key))
                {
                    errors.Add($"Localization table {path} contains an empty key");
                    continue;
                }

                if (!keys.Add(entry.key))
                {
                    errors.Add($"Localization table {path} contains duplicate key {entry.key}");
                }

                if (string.IsNullOrWhiteSpace(entry.value))
                {
                    errors.Add($"Localization key {entry.key} has an empty English value");
                }
            }

            return keys;
        }

        private static void ValidateKey(string chapterId, string key, HashSet<string> chapterKeys,
            HashSet<string> sharedKeys, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            var set = key.StartsWith("shared.", StringComparison.Ordinal) ? sharedKeys : chapterKeys;
            if (!set.Contains(key))
            {
                errors.Add($"{chapterId} references missing English localization key {key}");
            }
        }

        private static void ValidateLink(string chapterId, string source, string target,
            Dictionary<string, NarrativeBeatFile> beats, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(target) || !beats.ContainsKey(target))
            {
                errors.Add($"{chapterId} {source} references missing beat {target}");
            }
        }

        private static void ValidateOptionalLink(string chapterId, string source, string target,
            Dictionary<string, NarrativeBeatFile> beats, List<string> errors)
        {
            if (!string.IsNullOrWhiteSpace(target))
            {
                ValidateLink(chapterId, source, target, beats, errors);
            }
        }

        private static void ValidateReachability(string chapterId, string startBeatId,
            Dictionary<string, NarrativeBeatFile> beats, List<string> errors)
        {
            var visited = new HashSet<string>(StringComparer.Ordinal);
            var pending = new Stack<string>();
            if (!string.IsNullOrWhiteSpace(startBeatId))
            {
                pending.Push(startBeatId);
            }

            while (pending.Count > 0)
            {
                var id = pending.Pop();
                if (!visited.Add(id) || !beats.TryGetValue(id, out var beat))
                {
                    continue;
                }

                Push(beat.nextBeatId);
                Push(beat.nextBeatIdRouteA);
                Push(beat.nextBeatIdRouteB);
                if (beat.choices != null)
                {
                    foreach (var choice in beat.choices)
                    {
                        Push(choice.nextBeatId);
                    }
                }
            }

            foreach (var id in beats.Keys)
            {
                if (!visited.Contains(id))
                {
                    errors.Add($"{chapterId} contains unreachable beat {id}");
                }
            }

            void Push(string id)
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    pending.Push(id);
                }
            }
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

        private readonly struct ValidationSpec
        {
            public readonly string chapterId;
            public readonly string flowPath;
            public readonly string scenePath;

            public ValidationSpec(string chapterId, string flowPath, string scenePath)
            {
                this.chapterId = chapterId;
                this.flowPath = flowPath;
                this.scenePath = scenePath;
            }
        }
    }
}
