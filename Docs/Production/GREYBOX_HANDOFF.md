# Four-chapter greybox handoff

## What is complete

Each chapter has a playable technical loop with the same verification path:

1. background-only opening slot;
2. dialogue slot with a supporting portrait;
3. player portrait and two choice buttons;
4. separate Route A and Route B result slots;
5. shared merge slot;
6. route-flag state check;
7. end slot and chapter-complete screen.

The visible text explains only what the technical slot is testing. It is not draft narrative.

## Scene files

- `Assets/Scenes/Chapters/Chapter01_OilTide.unity`
- `Assets/Scenes/Chapters/Chapter02_SpermWhaleRescue.unity`
- `Assets/Scenes/Chapters/Chapter03_ReefWarning.unity`
- `Assets/Scenes/Chapters/Chapter04_HawksbillGuzheng.unity`

## Asset upload locations

Each chapter directory under `Assets/Art/Chapters` contains these stable subfolders:

- `Backgrounds`
- `Portraits`
- `Characters`
- `Props`
- `CG`
- `UI`

Creators should upload source-quality files to the matching chapter and category. The technical team will handle Unity import settings, sprite slicing, compression, catalog registration, and scene connection.

## Current placeholder key pattern

- Backgrounds: `GB_C##_BG_01` through `GB_C##_BG_07`
- Player portrait: `GB_C##_POR_PLAYER`
- Supporting portraits: `GB_C##_POR_NPC_01` and `GB_C##_POR_NPC_02`

These keys may be remapped after the creator supplies the final shot list. Renaming a key does not authorize changing the submitted art.

## Acceptance test

For each scene, run Route A and Route B separately. Confirm that the selected branch appears, both branches reconnect, only the matching state check appears, and the chapter reaches the completion screen. Missing artwork is acceptable during greybox review; missing buttons, broken links, incorrect state, clipped text, or a blocked completion screen are not.
