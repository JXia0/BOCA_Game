# Superseded greybox handoff

This file records that the original four-chapter greybox milestone has been replaced by the production runtime. Do not use the former `GB_C##_...` placeholder key scheme for new assets.

## Current playable structure

- `Assets/Scenes/MainMenu.unity` provides Start/Continue, chapter selection, settings, language selection, volume, and quit.
- Four chapter scenes use approved scripts, production art catalogs, mouse/keyboard controls, evidence state, delayed review effects, and final record reviews.
- Completing a chapter saves progress and unlocks the next chapter.
- Every referenced background and portrait must resolve before production validation passes.

## Scene files

- `Assets/Scenes/Chapters/Chapter01_OilTide.unity`
- `Assets/Scenes/Chapters/Chapter02_SpermWhaleRescue.unity`
- `Assets/Scenes/Chapters/Chapter03_ReefWarning.unity`
- `Assets/Scenes/Chapters/Chapter04_HawksbillGuzheng.unity`

## Asset upload locations

Each chapter directory under `Assets/Art/Chapters` retains stable category folders such as `Backgrounds`, `Portraits`, `Characters`, `Props`, `CG`, and `UI`. Creators upload source-quality art to the matching chapter/category; the technical team handles import settings, catalog registration, validation, and scene connection without altering the supplied artwork.

## Acceptance test

Run every chapter to completion, including named routes. Confirm that interactions record consequences, no choice blocks progress as a forced quiz, art resolves, text remains readable at 16:9, completion writes progress, and the next chapter unlocks. Unity production validation and a Windows QA build must both pass before handoff.
