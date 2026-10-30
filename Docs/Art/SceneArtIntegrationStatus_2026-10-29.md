# Scene art integration status — updated 2026-10-30

This document records the technical scene-wiring status for the four chapter greyboxes. It does not assign story meaning to an image. A production sprite is connected only when the creator has supplied an unambiguous greybox-slot-to-stable-key mapping.

## Shared presentation layer

- Background and portrait sprites preserve their source aspect ratio.
- Pixel art uses the largest whole-number scale that fits when enlarging; oversized art is reduced proportionally.
- Chapter fallback colour fills the unused part of the common 16:9 presentation area.
- Existing submitted source images are not resampled, stretched, redrawn, or overwritten. The newly generated Chapter 1 backgrounds were intentionally centre-cropped and nearest-neighbour normalised to 320 × 180 as their final game-ready masters.
- Art lookup remains exact and case-sensitive through `ArtCatalog.Resolve`.

## Chapter 1 — Oil Tide

**Status: all seven generated backgrounds are integrated. Portrait slots still require creator mapping.**

The chapter catalog contains 69 production sprites: 7 generated backgrounds, 48 scene-character sprites, and 14 dialogue portraits. The seven backgrounds were generated with the built-in image generator from the approved Chapter 1 source-document sequence, then imported at 320 × 180 with point filtering and no mipmaps.

| Greybox key | Production binding | Status | Evidence and required decision |
| --- | --- | --- | --- |
| `GB_C01_BG_01` | `C01_BG_UNDERSEA_ARCHIVE_OPENING` | integrated | Opening underwater archive and incomplete medal display. |
| `GB_C01_BG_02` | `C01_BG_OIL_TIDE_BEACH` | integrated | Dawn beach with a restrained oil-streaked tide line. |
| `GB_C01_BG_03` | `C01_BG_RESCUE_HANDOFF` | integrated | Organised coastal rescue handoff area. |
| `GB_C01_BG_04` | `C01_BG_WILDLIFE_CARE_CENTER` | integrated | Wildlife care-centre treatment room, shared by both choice branches. |
| `GB_C01_BG_05` | `C01_BG_RECOVERY_RECORDS` | integrated | Recovery-records station inside the same care centre. |
| `GB_C01_BG_06` | `C01_BG_SEASIDE_RELEASE` | integrated | Calm dawn release route to the sea. |
| `GB_C01_BG_07` | `C01_BG_UNDERSEA_ARCHIVE_RETURN` | integrated | The opening archive composition with a stronger memory-fragment glow. |
| `GB_C01_POR_NPC_01` | none | unsuitable / ambiguous | The approved Chapter 1 source document places a tourist and the rescue hotline in the early beach dialogue. Tourist art exists only as 50 × 50 scene-character sprites, not dialogue portraits, and there is no hotline portrait. A scene-character sprite will not be substituted into the portrait slot. |
| `GB_C01_POR_PLAYER` | none | ambiguous | Four exact player portrait keys exist (`NEUTRAL`, `SURPRISED`, `TALK_01`, `TALK_02`), but no approved beat-to-expression map selects one. |
| `GB_C01_POR_NPC_02` | none | ambiguous | Archivist, Marina, and RescueWorker01 portraits all exist and all characters appear in the chapter source. The final slot identity and expression are not specified. |

All Chapter 1 portraits are 32 × 32 RGBA sprites. The upload guide recommends 64 × 64 portraits, but the submitted pixels are preserved and shown with integer-friendly scaling rather than being resized.

### Creator handoff needed

Provide exact portrait choices for `GB_C01_POR_NPC_01`, `GB_C01_POR_PLAYER`, and `GB_C01_POR_NPC_02`. If the early dialogue must show a tourist, supply a dialogue portrait or explicitly approve a named alternative; do not rely on folder order.

## Chapter 2 — Sperm Whale Rescue

**Status: blocked on creator mapping and undefined prop placement. Two generated Fisherman portraits are available but intentionally unbound. No Chapter 2 flow key has been changed.**

The chapter catalog contains 35 production sprites: 14 backgrounds, 5 Fisherman scene-character sprites, 2 generated Fisherman portraits, 4 player portraits, and 10 props. The current greybox requires seven background slots, one player portrait slot, and two supporting portrait slots. It has no independent character or prop presentation field.

| Greybox key | Production binding | Status | Evidence and required decision |
| --- | --- | --- | --- |
| `GB_C02_BG_01` … `GB_C02_BG_07` | none | ambiguous | Fourteen semantically different locations and times are available for seven slots. The repository contains no approved Chapter 2 script or shot list that defines the seven-key sequence. Alphabetical order is not narrative order. |
| `GB_C02_POR_NPC_01` | none | ambiguous | `C02_POR_FISHERMAN_NEUTRAL` and `C02_POR_FISHERMAN_TALK` now exist, but the repository still does not identify this slot's speaker. |
| `GB_C02_POR_PLAYER` | none | ambiguous | Four exact player portrait keys exist (`DETERMINED`, `NEUTRAL`, `THINKING`, `WORRIED`), but no approved beat-to-expression map selects one. |
| `GB_C02_POR_NPC_02` | none | ambiguous | The generated Fisherman portraits are valid portrait assets, but binding either one here would still invent the speaker identity. |

Background sources range from 123 × 69 to 2730 × 1535. The shared fitter handles this range without source resampling: 16:9 art is centred, enlarged at whole-number scale when possible, or reduced proportionally when oversized.

The ten Props are registered in the catalog but the current `NarrativeBeatFile` and `ChapterView` have no independent prop key, placement, or draw-order contract. Adding a generic prop layer before the creator identifies the beat, anchor, scale, and overlap rules would invent composition. Those decisions remain pending.

### Creator handoff needed

Provide one exact stable background key for every `GB_C02_BG_*` slot, the selected player expression, and the identity/expression intended for each NPC slot. Explicitly confirm whether either NPC slot is the Fisherman before binding `C02_POR_FISHERMAN_NEUTRAL` or `C02_POR_FISHERMAN_TALK`. For each Prop intended to appear independently, provide its beat ID plus approved anchor, scale, and draw order.

## Chapter 3 — Reef Warning

**Status: blocked on creator mapping, missing NPC portraits, and undefined character/UI placement. No Chapter 3 flow key has been changed.**

The chapter catalog contains 37 production sprites: 9 backgrounds, 6 CGs, 3 Penguin character states, 9 production Player character sprites, 5 PlayerFullbody concept images, 4 player portraits, and 1 Penguin status UI image. The current greybox requires seven background slots, one player portrait slot, and two supporting portrait slots. It has no independent scene-character or UI-overlay presentation field.

| Greybox key | Production binding | Status | Evidence and required decision |
| --- | --- | --- | --- |
| `GB_C03_BG_01` … `GB_C03_BG_07` | none | ambiguous | Fifteen semantically different Background and CG images are available for seven slots. No approved shot list defines which seven images are used or their order. |
| `GB_C03_POR_NPC_01` | none | missing | No NPC Portrait asset exists. Penguin states are full scene-character images, not dialogue portraits. |
| `GB_C03_POR_PLAYER` | none | ambiguous | Four exact player portrait keys exist (`NEUTRAL`, `POUT`, `TALK_01_CONFUSED`, `TALK_02_SURPRISED`), but no approved beat-to-expression map selects one. |
| `GB_C03_POR_NPC_02` | none | missing | No NPC Portrait asset exists. |

The correct production player set is `Characters/Player`: nine 50 × 50 RGBA sprites. `Characters/PlayerFullbody` contains five incompatible 2048 × 2048 concept images. `BOCAProductionValidator` now rejects any Chapter 3 beat whose background or portrait key starts with `C03_CHAR_PLAYER_FULLBODY_`; the concept images remain safely available in the catalog for reference.

The three Penguin states are 1728 × 2304, and the Penguin status UI is 2064 × 1136. The current flow and view have no independent character/UI key, placement, or draw-order contract. They will not be flattened into a background or substituted into a portrait slot without creator direction.

Background and CG sources range from 2048 × 1152 to 2720 × 1536. The shared fitter reduces them proportionally inside the common 16:9 area and leaves unused space as chapter fallback colour; it does not resample the source files.

### Creator handoff needed

Provide one exact Background or CG stable key for each `GB_C03_BG_*` slot, the selected player expression, and two NPC portrait keys. For each Penguin state, production Player character sprite, or UI image intended to appear independently, provide its beat ID plus approved anchor, scale, and draw order. `PlayerFullbody` keys are not accepted as production player bindings.

## Chapter 4 — Hawksbill Guzheng

**Status: blocked on creator mapping, at least one missing/reuse decision, and all portrait assets. No Chapter 4 flow key has been changed.**

The chapter catalog contains six production sprites: four backgrounds and two event CGs. The current greybox requires seven background slots, one player portrait slot, and two supporting portrait slots. No Character or Portrait asset exists.

| Greybox key | Production binding | Status | Evidence and required decision |
| --- | --- | --- | --- |
| `GB_C04_BG_01` … `GB_C04_BG_07` | none | missing / ambiguous | Six semantically different visual assets are available for seven slots. The repository contains no approved Chapter 4 script or shot list, and no instruction identifies a missing seventh image or approves reusing a named stable key. The two CGs are event-specific compositions, not interchangeable room backgrounds. |
| `GB_C04_POR_NPC_01` | none | missing | No NPC Portrait asset exists. |
| `GB_C04_POR_PLAYER` | none | missing | No Player Portrait asset exists. |
| `GB_C04_POR_NPC_02` | none | missing | No NPC Portrait asset exists. |

The six visual sources range from 136 × 76 to 228 × 128 and are close to 16:9 without being identical. The shared fitter centres each source in the common 16:9 area and chooses the largest whole-number enlargement that fits. It does not stretch each image to a different aspect ratio.

Visual inspection confirms that `C04_CG_INVESTIGATION_MONTAGE` is a three-panel investigation sequence and `C04_CG_PACKING_ROOM_POLICE_ARRIVAL` is a specific event composition. They will be connected only to creator-approved beats, not used as anonymous filler backgrounds.

### Creator handoff needed

Provide one exact Background or CG stable key for every `GB_C04_BG_*` slot, including an explicit decision for the seventh slot (new asset or approved reuse). Supply a Player portrait and two NPC portrait keys, or explicitly approve named alternatives designed for the portrait frame.

## Overall integration boundary

The shared Unity presentation and exact-key validation work is complete. Actual production-art wiring remains intentionally unchanged because all four chapters lack the creator-supplied slot map required by the project handoff contract. Chapter 1 also lacks all seven backgrounds; Chapters 2 and 3 lack NPC portraits; Chapter 4 lacks all portraits and one background/reuse decision. Once those exact mappings and missing assets are supplied, the flow keys can be replaced without changing narrative text, branch structure, or source image bytes.
