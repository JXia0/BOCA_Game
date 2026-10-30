using System;
using System.Collections.Generic;
using System.Linq;
using BOCA.Art;
using BOCA.Narrative;
using BOCA.UI;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BOCA.Editor
{
    public static class BOCAProductionValidator
    {
        private static readonly ValidationSpec[] Chapters =
        {
            new ValidationSpec(1, "chapter-01", "Assets/Resources/BOCA/Flows/chapter-01.json", "Assets/Scenes/Chapters/Chapter01_OilTide.unity"),
            new ValidationSpec(2, "chapter-02", "Assets/Resources/BOCA/Flows/chapter-02.json", "Assets/Scenes/Chapters/Chapter02_SpermWhaleRescue.unity"),
            new ValidationSpec(3, "chapter-03", "Assets/Resources/BOCA/Flows/chapter-03.json", "Assets/Scenes/Chapters/Chapter03_ReefWarning.unity"),
            new ValidationSpec(4, "chapter-04", "Assets/Resources/BOCA/Flows/chapter-04.json", "Assets/Scenes/Chapters/Chapter04_HawksbillGuzheng.unity")
        };

        [MenuItem("BOCA/Production/Validate Production Foundation")]
        public static void ValidateProject()
        {
            var errors = new List<string>();
            var sharedKeys = LoadLocalizationKeys("shared", errors);
            LoadLocalizationKeys("menu", errors);
            var sharedArtCatalog = AssetDatabase.LoadAssetAtPath<ArtCatalog>(BOCAArtCatalogBuilder.SharedCatalogPath);

            ValidateMainMenu(errors);

            foreach (var spec in Chapters)
            {
                ValidateChapter(spec, sharedKeys, sharedArtCatalog, errors);
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

        private static void ValidateMainMenu(List<string> errors)
        {
            const string menuPath = "Assets/Scenes/MainMenu.unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(menuPath) == null)
            {
                errors.Add($"Missing main menu scene {menuPath}");
                return;
            }

            var buildScenes = EditorBuildSettings.scenes;
            if (buildScenes.Length == 0 || !buildScenes[0].enabled ||
                !string.Equals(buildScenes[0].path, menuPath, StringComparison.Ordinal))
            {
                errors.Add("Main menu must be the first enabled scene in Build Settings");
            }

            EditorSceneManager.OpenScene(menuPath, OpenSceneMode.Single);
            ValidateSceneObjectCount<MainMenuController>(menuPath, errors);
            ValidateSceneObjectCount<MainMenuView>(menuPath, errors);
            ValidateSceneObjectCount<Canvas>(menuPath, errors);
            ValidateSceneObjectCount<EventSystem>(menuPath, errors);
        }

        private static void ValidateSceneObjectCount<T>(string scenePath, List<string> errors) where T : UnityEngine.Object
        {
            var objects = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (objects.Length != 1)
            {
                errors.Add($"{scenePath} needs exactly one {typeof(T).Name}; found {objects.Length}");
            }
        }

        private static void ValidateChapter(ValidationSpec spec, HashSet<string> sharedKeys,
            ArtCatalog sharedArtCatalog, List<string> errors)
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
            var chapterArtCatalog = BOCAArtCatalogBuilder.LoadChapterCatalog(spec.chapterNumber);
            var beats = new Dictionary<string, NarrativeBeatFile>(StringComparer.Ordinal);
            var setFlags = new HashSet<string>(StringComparer.Ordinal);
            var reviewDimensions = new HashSet<string>(StringComparer.Ordinal);
            if (flow.reviewDimensions != null)
            {
                foreach (var dimension in flow.reviewDimensions)
                {
                    if (dimension == null || string.IsNullOrWhiteSpace(dimension.id))
                    {
                        errors.Add($"{spec.chapterId} contains a review dimension without an id");
                        continue;
                    }

                    if (!reviewDimensions.Add(dimension.id))
                    {
                        errors.Add($"{spec.chapterId} contains duplicate review dimension {dimension.id}");
                    }

                    ValidateKey(spec.chapterId, dimension.labelKey, chapterKeys, sharedKeys, errors);
                }
            }

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
                ValidateKey(spec.chapterId, beat.promptKey, chapterKeys, sharedKeys, errors);
                ValidateKey(spec.chapterId, beat.submitLabelKey, chapterKeys, sharedKeys, errors);
                ValidateArtKey(spec, beat.id, "background", beat.backgroundKey, chapterArtCatalog,
                    sharedArtCatalog, errors);
                ValidateArtKey(spec, beat.id, "portrait", beat.portraitKey, chapterArtCatalog,
                    sharedArtCatalog, errors);

                if (RequiresDialoguePortrait(beat.id) && string.IsNullOrWhiteSpace(beat.portraitKey))
                {
                    errors.Add($"{beat.id} is a character dialogue beat but has no production portrait");
                }

                if (beat.choices != null && beat.choices.Length > 6)
                {
                    errors.Add($"{beat.id} has {beat.choices.Length} choices; the production UI supports at most 6");
                }

                if (beat.choices != null)
                {
                    ValidateInteraction(spec.chapterId, beat, errors);
                    foreach (var choice in beat.choices)
                    {
                        ValidateKey(spec.chapterId, choice.labelKey, chapterKeys, sharedKeys, errors);
                        ValidateKey(spec.chapterId, choice.feedbackKey, chapterKeys, sharedKeys, errors);
                        if (!string.IsNullOrWhiteSpace(choice.setFlag))
                        {
                            setFlags.Add(choice.setFlag);
                        }

                        if (choice.setFlags != null)
                        {
                            foreach (var flag in choice.setFlags.Where(flag => !string.IsNullOrWhiteSpace(flag)))
                            {
                                setFlags.Add(flag);
                            }
                        }

                        ValidateEffects(spec.chapterId, beat.id, choice.effects, reviewDimensions, errors);
                    }

                    ValidateEffects(spec.chapterId, beat.id, beat.successEffects, reviewDimensions, errors);
                    ValidateEffects(spec.chapterId, beat.id, beat.failureEffects, reviewDimensions, errors);
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
                        if (NarrativeSession.RequiresSubmission(beat))
                        {
                            ValidateOptionalLink(spec.chapterId, beat.id + " choice " + choice.id, choice.nextBeatId, beats, errors);
                        }
                        else
                        {
                            ValidateLink(spec.chapterId, beat.id + " choice " + choice.id, choice.nextBeatId, beats, errors);
                        }
                    }
                }
            }

            ValidateReachability(spec.chapterId, flow.startBeatId, beats, errors);
            var namedRoutes = flow.beats
                .Where(beat => beat?.choices != null)
                .SelectMany(beat => beat.choices)
                .Where(choice => choice != null && !string.IsNullOrWhiteSpace(choice.setRoute))
                .Select(choice => choice.setRoute)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (namedRoutes.Length == 0)
            {
                ValidateRouteCompletion(flow, null, errors);
            }
            else
            {
                foreach (var route in namedRoutes)
                {
                    ValidateRouteCompletion(flow, route, errors);
                }
            }
            ValidateScene(spec, flowAsset, errors);
        }

        private static void ValidateRouteCompletion(ChapterFlowFile flow, string route, List<string> errors)
        {
            try
            {
                var session = new NarrativeSession(flow);
                var guard = (flow.beats?.Length ?? 0) * 3 + 1;
                var usedNamedRoute = false;
                var routeLabel = string.IsNullOrWhiteSpace(route) ? "default" : route;

                for (var step = 0; step < guard && !session.IsComplete; step++)
                {
                    var beat = session.CurrentBeat;
                    if (!string.IsNullOrWhiteSpace(beat.requiredFlag) && !session.Flags.Contains(beat.requiredFlag))
                    {
                        errors.Add($"{flow.chapterId} route {routeLabel} displayed conditional beat {beat.id} without flag {beat.requiredFlag}");
                        return;
                    }

                    if (!NarrativeSession.HasChoices(beat))
                    {
                        session.Advance();
                        continue;
                    }

                    var choiceIndex = 0;
                    for (var i = 0; i < beat.choices.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(route) ||
                            !string.Equals(beat.choices[i].setRoute, route, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        choiceIndex = i;
                        usedNamedRoute = true;
                        break;
                    }

                    if (NarrativeSession.RequiresSubmission(beat))
                    {
                        var selectedIndices = new List<int>();
                        if (beat.expectedChoiceIds != null && beat.expectedChoiceIds.Length > 0)
                        {
                            foreach (var expectedId in beat.expectedChoiceIds)
                            {
                                var expectedIndex = Array.FindIndex(beat.choices,
                                    choice => string.Equals(choice.id, expectedId, StringComparison.Ordinal));
                                if (expectedIndex >= 0)
                                {
                                    selectedIndices.Add(expectedIndex);
                                }
                            }
                        }

                        var minimum = Math.Max(1, beat.minimumSelections);
                        for (var i = 0; selectedIndices.Count < minimum && i < beat.choices.Length; i++)
                        {
                            if (!selectedIndices.Contains(i))
                            {
                                selectedIndices.Add(i);
                            }
                        }

                        session.CommitSelections(selectedIndices);
                    }
                    else
                    {
                        session.Choose(choiceIndex);
                    }
                }

                if (!session.IsComplete)
                {
                    errors.Add($"{flow.chapterId} route {routeLabel} did not reach chapter completion");
                }

                if (!string.IsNullOrWhiteSpace(route) && !usedNamedRoute)
                {
                    errors.Add($"{flow.chapterId} has no choice that selects route {routeLabel}");
                }
            }
            catch (Exception exception)
            {
                var routeLabel = string.IsNullOrWhiteSpace(route) ? "default" : route;
                errors.Add($"{flow.chapterId} route {routeLabel} failed during simulation: {exception.Message}");
            }
        }

        private static void ValidateInteraction(string chapterId, NarrativeBeatFile beat, List<string> errors)
        {
            var mode = string.IsNullOrWhiteSpace(beat.interactionMode) ? "single" : beat.interactionMode;
            if (!string.Equals(mode, "single", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(mode, "checklist", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(mode, "sequence", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"{chapterId} beat {beat.id} uses unsupported interaction mode {beat.interactionMode}");
            }

            var minimum = Math.Max(1, beat.minimumSelections);
            var maximum = beat.maximumSelections > 0 ? beat.maximumSelections : beat.choices.Length;
            if (minimum > maximum || maximum > beat.choices.Length)
            {
                errors.Add($"{chapterId} beat {beat.id} has invalid selection range {minimum}-{maximum}");
            }

            var choiceIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var choice in beat.choices)
            {
                if (choice == null || string.IsNullOrWhiteSpace(choice.id))
                {
                    errors.Add($"{chapterId} beat {beat.id} contains a choice without an id");
                    continue;
                }

                if (!choiceIds.Add(choice.id))
                {
                    errors.Add($"{chapterId} beat {beat.id} contains duplicate choice {choice.id}");
                }
            }

            if (beat.expectedChoiceIds == null)
            {
                return;
            }

            foreach (var expectedId in beat.expectedChoiceIds)
            {
                if (!choiceIds.Contains(expectedId))
                {
                    errors.Add($"{chapterId} beat {beat.id} expects missing choice {expectedId}");
                }
            }
        }

        private static void ValidateEffects(string chapterId, string beatId, NarrativeScoreEffectFile[] effects,
            HashSet<string> reviewDimensions, List<string> errors)
        {
            if (effects == null)
            {
                return;
            }

            foreach (var effect in effects)
            {
                if (effect == null || string.IsNullOrWhiteSpace(effect.dimensionId))
                {
                    errors.Add($"{chapterId} beat {beatId} contains a score effect without a dimension id");
                }
                else if (!reviewDimensions.Contains(effect.dimensionId))
                {
                    errors.Add($"{chapterId} beat {beatId} affects undeclared review dimension {effect.dimensionId}");
                }
            }
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
            var englishKeys = LoadLocaleKeys("en-US", table, errors);
            var chineseKeys = LoadLocaleKeys("zh-CN", table, errors);

            foreach (var key in englishKeys.Where(key => !chineseKeys.Contains(key)))
            {
                errors.Add($"Simplified Chinese localization table {table} is missing key {key}");
            }

            foreach (var key in chineseKeys.Where(key => !englishKeys.Contains(key)))
            {
                errors.Add($"Simplified Chinese localization table {table} contains extra key {key}");
            }

            return englishKeys;
        }

        private static HashSet<string> LoadLocaleKeys(string locale, string table, List<string> errors)
        {
            var path = $"Assets/Resources/BOCA/Localization/{locale}/{table}.json";
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            var keys = new HashSet<string>(StringComparer.Ordinal);
            if (asset == null)
            {
                errors.Add($"Missing {locale} localization table {path}");
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
                    errors.Add($"Localization key {entry.key} has an empty {locale} value");
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
                errors.Add($"{chapterId} references missing runtime localization key {key}");
            }
        }

        private static void ValidateArtKey(ValidationSpec spec, string beatId, string slot, string key,
            ArtCatalog chapterCatalog, ArtCatalog sharedCatalog, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (key.StartsWith("GB_", StringComparison.Ordinal))
            {
                errors.Add($"{beatId} {slot} still references greybox art key {key}");
                return;
            }

            if (spec.chapterNumber == 3 &&
                key.StartsWith("C03_CHAR_PLAYER_FULLBODY_", StringComparison.Ordinal))
            {
                errors.Add($"{beatId} {slot} key {key} references Chapter 3 concept art; use Characters/Player production pixel art instead");
                return;
            }

            var expectedPrefix = $"C{spec.chapterNumber:00}_";
            if (key.StartsWith("C0", StringComparison.Ordinal) &&
                !key.StartsWith(expectedPrefix, StringComparison.Ordinal))
            {
                errors.Add($"{beatId} {slot} key {key} does not belong to {spec.chapterId}");
                return;
            }

            var sprite = chapterCatalog == null ? null : chapterCatalog.Resolve(key);
            sprite ??= sharedCatalog == null ? null : sharedCatalog.Resolve(key);
            if (sprite == null)
            {
                errors.Add($"{beatId} {slot} references missing exact art key {key}");
            }
        }

        private static bool RequiresDialoguePortrait(string beatId)
        {
            return beatId.EndsWith(".gb.02", StringComparison.Ordinal) ||
                   beatId.EndsWith(".gb.choice", StringComparison.Ordinal) ||
                   beatId.EndsWith(".gb.merge", StringComparison.Ordinal);
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
            public readonly int chapterNumber;
            public readonly string chapterId;
            public readonly string flowPath;
            public readonly string scenePath;

            public ValidationSpec(int chapterNumber, string chapterId, string flowPath, string scenePath)
            {
                this.chapterNumber = chapterNumber;
                this.chapterId = chapterId;
                this.flowPath = flowPath;
                this.scenePath = scenePath;
            }
        }
    }
}
