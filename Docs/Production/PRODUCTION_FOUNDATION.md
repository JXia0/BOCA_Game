# BOCA production foundation

## Current decision

BOCA is now a production Unity project, not a disposable sandbox. The game is an English-first pixel-art educational narrative with dialogue, scene illustrations, portraits, key CGs, hotspots, and short consequence-bearing choices. It does not use combat, platforming, reaction challenges, physics puzzles, or complex resource management.

The title screen and settings screen are deliberately not part of this milestone. Language selection is already supported by the runtime so a future Settings control can switch between English and Simplified Chinese without duplicating scenes.

## Scene status

| Scene | Current status | Narrative authority |
| --- | --- | --- |
| `Chapter01_OilTide` | Production hierarchy and replaceable English staging flow | Student revision still expected |
| `Chapter02_SpermWhaleRescue` | Production hierarchy and replaceable English staging flow | Student revision still expected |
| `Chapter03_ReefWarning` | Production hierarchy and replaceable English staging flow | Student revision still expected |
| `Chapter04_HawksbillGuzheng` | Production hierarchy and implemented English flow with Route A and Route B | Locked v3 DOCX |

The older `SampleScene` and `Chapter1PlayablePrototype` remain as a historical horizontal slice. The prototype now runs only inside `SampleScene` and cannot appear over the production chapters.

## Runtime structure

- `LocalizationService` owns the selected language and saves it in `PlayerPrefs` under `boca.settings.language`.
- `ChapterFlowFile` stores stable beat IDs, branches, art keys, and localization keys.
- English text lives in `Assets/Resources/BOCA/Localization/en-US`.
- Future Chinese files use the same filenames and keys in `Assets/Resources/BOCA/Localization/zh-CN`.
- `NarrativeSession` advances beats and preserves the Chapter 4 route choice.
- `ChapterSceneController` connects flow data, localization, art catalogs, and the scene view.
- `ChapterView` owns the shared 16:9 scene area, lower dialogue panel, portrait, continue control, and up to three short choices.
- `ArtCatalog` maps stable asset keys to imported sprites. The editor rebuild command scans the controlled art folders.

This separation is intentional: dialogue can change without rebuilding a scene, translation can change without editing flow, and artwork can be replaced without changing code.

## Updating student dialogue

When Chapters 1 to 3 are approved:

1. Keep existing scene and beat IDs whenever the dramatic function is unchanged.
2. Replace English values in the chapter localization JSON.
3. Add or remove beats only when staging or branching actually changes.
4. Run `BOCA > Production > Rebuild All Chapter Scenes` only if the shared scene hierarchy or serialized references intentionally change. This command replaces all four generated scene files and asks for confirmation in the editor.
5. Run `BOCA > Production > Validate Production Foundation` before committing.

Do not paste final dialogue directly into scene objects. That would break localization and make student revisions expensive.

## Language plan

The game currently defaults to English. Missing Chinese entries fall back to English rather than displaying blank text. Later, the title-screen Settings UI should call one of these methods:

```csharp
LocalizationService.SetLanguage(GameLanguage.English);
LocalizationService.SetLanguage(GameLanguage.ChineseSimplified);
```

Every active chapter view listens for the language-changed event and redraws the current line immediately.

## Quality gates

Before a production commit is accepted:

- all four chapter scenes open without compile errors;
- every flow starts at a valid beat and every branch target exists;
- every player-facing key has English text;
- no screen presents more than three choices;
- art keys are unique;
- unavailable Chinese text falls back to English;
- missing art shows a labelled staging placeholder instead of a null reference;
- Chapter 4 Route A and Route B preserve different evidence boundaries;
- fictional marketplace and privacy rules remain visible in implementation;
- the Unity project is validated in version 6000.5.3f1.

## Next production priorities

1. Replace the staging UI blocks with the first approved shared pixel UI sample.
2. Integrate Chapter 4 P0 backgrounds, portraits, and the hawksbill information CG.
3. Review the final student revisions for Chapters 1 to 3, then update localization text and any affected branches.
4. Add save progress, chapter selection, title screen, settings, and language controls after the four chapter loops are stable.
5. Complete an accessibility pass for font size, contrast, keyboard navigation, reduced motion, and text speed.
