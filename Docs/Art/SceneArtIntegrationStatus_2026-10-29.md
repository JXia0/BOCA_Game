# Scene art integration status — completed 2026-10-30

This document records the production art bindings for the four chapter greyboxes. The project owner authorised completion of the remaining technical mappings on 2026-10-30. This pass changes art keys only; it does not rewrite student-authored dialogue, branches, or source images.

## Completion rules

- Every chapter beat now uses an exact production background key. No `GB_C0*_BG_*` or `GB_C0*_POR_*` art key remains in a flow.
- Background and portrait sprites preserve their source aspect ratio.
- Pixel art uses the largest whole-number scale that fits when enlarging; oversized art is reduced proportionally.
- Chapter fallback colour fills unused space in the common 16:9 presentation area.
- The three character-dialogue slots in every chapter (`gb.02`, `gb.choice`, and `gb.merge`) use exact production portrait keys. System and pure-narration beats may intentionally omit portraits.
- Art lookup remains exact and case-sensitive through `ArtCatalog.Resolve`.
- `BOCAProductionValidator` rejects future greybox art keys, rejects an empty portrait in any required character-dialogue slot, and rejects Chapter 3 `PlayerFullbody` concept art as a production binding.
- Props, character animation frames, and UI overlays remain registered in their catalogs. They are not required by the current dialogue-and-image presentation contract and are not flattened into backgrounds.

## Chapter 1 — Oil Tide

**Status: 7/7 production backgrounds and 3/3 dialogue portraits are integrated.**

| Greybox slot | Production binding | Presentation decision |
| --- | --- | --- |
| `GB_C01_BG_01` | `C01_BG_UNDERSEA_ARCHIVE_OPENING` | Opening archive. |
| `GB_C01_BG_02` | `C01_BG_OIL_TIDE_BEACH` | Oil-tide discovery. |
| `GB_C01_BG_03` | `C01_BG_RESCUE_HANDOFF` | Player decision point. |
| `GB_C01_BG_04` | `C01_BG_WILDLIFE_CARE_CENTER` | Shared branch location. |
| `GB_C01_BG_05` | `C01_BG_RECOVERY_RECORDS` | Branch merge. |
| `GB_C01_BG_06` | `C01_BG_SEASIDE_RELEASE` | Shared state check. |
| `GB_C01_BG_07` | `C01_BG_UNDERSEA_ARCHIVE_RETURN` | Chapter ending. |
| `GB_C01_POR_NPC_01` | `C01_POR_MARINA_NEUTRAL` | Existing neutral supporting-character portrait for the first character dialogue slot. |
| `GB_C01_POR_PLAYER` | `C01_POR_PLAYER_NEUTRAL` | Neutral expression avoids inventing an emotional beat before final dialogue. |
| `GB_C01_POR_NPC_02` | `C01_POR_RESCUEWORKER01_TALK_01` | Existing rescue-worker speaking portrait for the merged rescue beat. |

## Chapter 2 — Sperm Whale Rescue

**Status: 7/7 production backgrounds and 3/3 dialogue portraits are integrated.**

| Greybox slot | Production binding | Presentation decision |
| --- | --- | --- |
| `GB_C02_BG_01` | `C02_BG_UNDERSEA_ARCHIVE` | Chapter opening. |
| `GB_C02_BG_02` | `C02_BG_FUJIAN_FISHING_VILLAGE` | Fisherman contact. |
| `GB_C02_BG_03` | `C02_BG_WHALE_MUDFLAT_LOW_TIDE` | Rescue decision point. |
| `GB_C02_BG_04` | `C02_BG_COMMAND_VEHICLE_INTERIOR` | Shared technical branch location. |
| `GB_C02_BG_05` | `C02_BG_NIGHT_TOWING` | Branch merge. |
| `GB_C02_BG_06` | `C02_BG_DEEP_WATER_ROUTE` | Shared state check. |
| `GB_C02_BG_07` | `C02_BG_DAWN_RELEASE` | Successful release ending. |
| `GB_C02_POR_NPC_01` | `C02_POR_FISHERMAN_NEUTRAL` | Existing Fisherman dialogue portrait. |
| `GB_C02_POR_PLAYER` | `C02_POR_PLAYER_WORRIED` | Conservative choice for an active rescue decision. |
| `GB_C02_POR_NPC_02` | `C02_POR_FISHERMAN_TALK` | Existing Fisherman speaking portrait. |

The remaining backgrounds and ten props stay available in the catalog for later student-authored beats. The current `NarrativeBeatFile` has no independent prop anchor, scale, or draw-order contract, so props are not placed arbitrarily.

## Chapter 3 — Reef Warning

**Status: 7/7 production backgrounds and 3/3 dialogue portraits are integrated.**

| Greybox slot | Production binding | Presentation decision |
| --- | --- | --- |
| `GB_C03_BG_01` | `C03_BG_VESSEL_DECK` | Chapter opening. |
| `GB_C03_BG_02` | `C03_BG_BEACH_SURVEY` | Coastal evidence discovery. |
| `GB_C03_BG_03` | `C03_BG_INVESTIGATION_DESK_INITIAL` | Player decision point. |
| `GB_C03_BG_04` | `C03_BG_INVESTIGATION_DESK_MAP` | Shared branch investigation. |
| `GB_C03_BG_05` | `C03_CG_EVIDENCE_REVIEW` | Branch merge and review. |
| `GB_C03_BG_06` | `C03_BG_NIGHT_OFFICE` | Shared state check. |
| `GB_C03_BG_07` | `C03_CG_SUNSET_04` | Chapter ending. |
| `GB_C03_POR_NPC_01` | `C03_POR_PENGUIN_CONCERNED` | Added transparent pixel portrait; concerned state for the first penguin dialogue slot. |
| `GB_C03_POR_PLAYER` | `C03_POR_PLAYER_NEUTRAL` | Neutral expression avoids inventing a final dialogue emotion. |
| `GB_C03_POR_NPC_02` | `C03_POR_PENGUIN_RELIEVED` | Added matching transparent pixel portrait; relieved state for the merged beat. |

`Characters/Player` remains the production pixel player set. `Characters/PlayerFullbody` remains concept art and cannot pass production validation. The two new penguin portraits are separate dialogue-frame assets; existing full-character penguin states and `C03_UI_PENGUIN_STATUS` remain catalogued for a future overlay contract.

## Chapter 4 — Hawksbill Guzheng

**Status: 7/7 production backgrounds and 3/3 dialogue portraits are integrated. The seven background slots use six supplied visuals and one explicit investigation-montage reuse.**

| Greybox slot | Production binding | Presentation decision |
| --- | --- | --- |
| `GB_C04_BG_01` | `C04_BG_PLAYER_BEDROOM_DAY` | Daytime opening. |
| `GB_C04_BG_02` | `C04_BG_PLAYER_BEDROOM_NIGHT` | Later message/dialogue beat. |
| `GB_C04_BG_03` | `C04_CG_INVESTIGATION_MONTAGE` | Player investigation choice. |
| `GB_C04_BG_04` | `C04_BG_SELLER_PACKING_ROOM` | Shared branch view of the seller. |
| `GB_C04_BG_05` | `C04_CG_INVESTIGATION_MONTAGE` | Explicit reuse for the merged investigation result. |
| `GB_C04_BG_06` | `C04_CG_PACKING_ROOM_POLICE_ARRIVAL` | Shared consequence/state check. |
| `GB_C04_BG_07` | `C04_BG_CONCERT_STAGE_NIGHT` | Chapter ending. |
| `GB_C04_POR_NPC_01` | `C04_POR_FRIEND_NEUTRAL` | Added friendly, non-suspicious pixel portrait for the friend's casual recommendation. |
| `GB_C04_POR_PLAYER` | `C04_POR_PLAYER_NEUTRAL` | Chapter-local copy of the established player portrait, preserving the shared protagonist design. |
| `GB_C04_POR_NPC_02` | `C04_POR_SELLER_NEUTRAL` | Added restrained, guarded seller portrait without caricatured villain styling. |

## Final integration boundary

The technical art integration is complete for the current greybox flows. All four chapters have 7/7 production background bindings and 3/3 required dialogue portraits. Every populated art key resolves to a real sprite, every required portrait slot is non-empty, and no greybox art key remains.

Unity 6000.5.3f1 rebuilt `Chapter03ArtCatalog.asset` and `Chapter04ArtCatalog.asset` after importing the five new portrait files. `BOCAProductionValidator` then opened and validated all four chapter scenes successfully at `2026-10-30T11:45:23+08:00` (`BOCA_PRODUCTION_VALIDATION_PASS: chapters=4`). Static checks also reported zero missing art keys and zero `GB_*` art keys.

The added Chapter 3 and Chapter 4 portraits are production-ready technical completion assets. The art team may redraw them later without code or Flow changes by preserving the current file names, sprite names, transparency, and portrait framing.

The English localization still contains explicit writer-slot text for Chapters 1–4. That content is owned by the student writers and is intentionally unchanged by this technical integration pass. When final dialogue arrives, art keys can be adjusted beat-by-beat without changing the shared presentation system or source image files.
