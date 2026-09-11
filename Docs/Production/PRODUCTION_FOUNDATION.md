# BOCA technical production foundation

## Current scope

BOCA is an English-first Unity 6000.5.3f1 project with a shared pixel-game presentation framework. The current milestone is technical greyboxing only. Literary writing and visual authorship remain with the student creators.

The title screen and settings screen are not part of this milestone. Runtime language selection remains available so a later Settings screen can switch between English and Simplified Chinese without duplicating chapter scenes.

## Chapter status

| Unity scene | Technical status | Creative content |
| --- | --- | --- |
| `Chapter01_OilTide` | Seven scene slots, portrait test, A/B branch, merge, state check, completion | Not loaded |
| `Chapter02_SpermWhaleRescue` | Seven scene slots, portrait test, A/B branch, merge, state check, completion | Not loaded |
| `Chapter03_ReefWarning` | Seven scene slots, portrait test, A/B branch, merge, state check, completion | Not loaded |
| `Chapter04_HawksbillGuzheng` | Seven scene slots, portrait test, A/B branch, merge, state check, completion | Not loaded |

The scene filenames are stable project identifiers. They do not mean the technical team has authority to edit the corresponding story.

## Runtime structure

- `LocalizationService` owns the selected language and stores it in `PlayerPrefs` under `boca.settings.language`.
- `ChapterFlowFile` stores stable beat IDs, branch links, flags, and art keys.
- `NarrativeSession` advances beats and preserves route and flag state.
- `ChapterSceneController` connects flow data, localization, art catalogs, and the scene view.
- `ChapterView` owns the shared 16:9 background area, dialogue panel, portrait, continue control, and up to three choices.
- `ArtCatalog` maps stable asset keys to imported sprites.
- Missing art displays a labelled placeholder instead of failing silently.

## Creator-approved integration process

1. Receive an approved script and a creator-supplied content map.
2. Record each line exactly as supplied; do not rewrite it during import.
3. Map the supplied lines to stable beat IDs or add technical beats when requested.
4. Connect creator-supplied art keys without changing the artwork.
5. Return a playable build to the creator for content approval.
6. Apply only approved corrections, then run the automated validator and route checks.

## Quality gates

- all four chapter scenes open without compiler errors;
- both branches in every greybox reach the merge and completion states;
- branch flags persist and display the correct state-check slot;
- all referenced localization keys and branch targets exist;
- no screen presents more than three choices;
- missing images display labelled placeholders;
- unavailable Chinese text falls back safely to English;
- runtime JSON contains no unapproved creative prose;
- the project validates in Unity 6000.5.3f1.

## Next technical priorities

1. Test all four greyboxes at the target aspect ratios.
2. Add chapter selection, save progress, title screen, settings, and language controls after the chapter loops are stable.
3. Prepare a non-destructive import checklist for creator-approved scripts and art.
4. Complete keyboard navigation, text-size, contrast, reduced-motion, and text-speed accessibility passes.
