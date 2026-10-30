# BOCA technical production foundation

## Current scope

BOCA is a Unity 6000.5.3f1 pixel-art educational game built around dialogue, images, evidence handling, and consequential choices rather than hard-core mechanics. The four approved chapter scripts are integrated into a shared production runtime. Literary writing and visual authorship remain with the student creators.

## Playable structure

| Scene | Production status |
| --- | --- |
| `MainMenu` | Start/Continue, chapter selection, progress states, Settings, language, volume, quit |
| `Chapter01_OilTide` | Approved script, production art, seven consequential interactions, final CARE review |
| `Chapter02_SpermWhaleRescue` | Approved script, production art, coordinated rescue decisions, final command-log review |
| `Chapter03_ReefWarning` | Approved script, production art, evidence classification and claim-scope review |
| `Chapter04_HawksbillGuzheng` | Approved script, two continuing routes, preservation/reporting decisions, final evidence review |

## Runtime structure

- `LocalizationService` stores the selected language in `PlayerPrefs`.
- `GameProgressService` stores chapter completion and unlocks the next chapter.
- `GameSettingsService` owns master volume.
- `SceneNavigator` provides menu, chapter, replay, and quit transitions.
- `NarrativeSession` records decisions, evidence, flags, routes, and review scores.
- `ChapterSceneController` connects approved flow data, localization, art catalogs, save progress, and the view.
- `ChapterView` owns the shared 16:9 artwork, dialogue, pixel controls, up to six choices, and completion review.
- `ArtCatalog` maps stable asset keys to creator-supplied sprites.

Missing production art is a validation failure. The runtime no longer displays technical asset-key placeholders to players.

## Creator-approved integration process

1. Receive an approved script or creator-approved revision.
2. Copy player-facing text exactly; do not rewrite it during import.
3. Map approved text to stable beats and technical interactions.
4. Connect creator-supplied art without modifying the artwork.
5. Return a playable build for content approval.
6. Apply only approved corrections, then run production validation and Windows visual QA.

## Quality gates

- main menu is the first enabled Build Settings scene;
- every production scene has exactly one required controller/view and opens without compiler errors;
- every route reaches completion without forced-correct quiz loops;
- localization keys, beat links, expected choices, evidence IDs, and review dimensions resolve;
- every referenced production sprite resolves;
- keyboard and mouse controls both work;
- completion saves progress and unlocks the next chapter;
- Windows development build and representative visual captures pass;
- runtime JSON contains no unapproved translation or narrative rewrite.

## Known external-content gap

No audio files are currently present under `Assets`. The volume setting and Unity audio path are ready, but music, ambience, and UI sound cannot be considered complete until the team supplies approved audio assets and usage rights.
