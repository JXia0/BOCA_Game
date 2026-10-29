# Scene art integration status — 2026-10-29

This document records the technical scene-wiring status for the four chapter greyboxes. It does not assign story meaning to an image. A production sprite is connected only when the creator has supplied an unambiguous greybox-slot-to-stable-key mapping.

## Shared presentation layer

- Background and portrait sprites preserve their source aspect ratio.
- Pixel art uses the largest whole-number scale that fits when enlarging; oversized art is reduced proportionally.
- Chapter fallback colour fills the unused part of the common 16:9 presentation area.
- Source images are not resampled, stretched, redrawn, or overwritten.
- Art lookup remains exact and case-sensitive through `ArtCatalog.Resolve`.

## Chapter 1 — Oil Tide

**Status: blocked on creator mapping and missing backgrounds. No Chapter 1 flow key has been changed.**

The chapter catalog contains 62 production sprites: 48 scene-character sprites and 14 dialogue portraits. It contains no Background or CG sprite. The current greybox requires seven background slots, one player portrait slot, and two supporting portrait slots.

| Greybox key | Production binding | Status | Evidence and required decision |
| --- | --- | --- | --- |
| `GB_C01_BG_01` | none | missing | No Chapter 1 Background or CG asset exists. |
| `GB_C01_BG_02` | none | missing | No Chapter 1 Background or CG asset exists. |
| `GB_C01_BG_03` | none | missing | No Chapter 1 Background or CG asset exists. |
| `GB_C01_BG_04` | none | missing | No Chapter 1 Background or CG asset exists. |
| `GB_C01_BG_05` | none | missing | No Chapter 1 Background or CG asset exists. |
| `GB_C01_BG_06` | none | missing | No Chapter 1 Background or CG asset exists. |
| `GB_C01_BG_07` | none | missing | No Chapter 1 Background or CG asset exists. |
| `GB_C01_POR_NPC_01` | none | unsuitable / ambiguous | The approved Chapter 1 source document places a tourist and the rescue hotline in the early beach dialogue. Tourist art exists only as 50 × 50 scene-character sprites, not dialogue portraits, and there is no hotline portrait. A scene-character sprite will not be substituted into the portrait slot. |
| `GB_C01_POR_PLAYER` | none | ambiguous | Four exact player portrait keys exist (`NEUTRAL`, `SURPRISED`, `TALK_01`, `TALK_02`), but no approved beat-to-expression map selects one. |
| `GB_C01_POR_NPC_02` | none | ambiguous | Archivist, Marina, and RescueWorker01 portraits all exist and all characters appear in the chapter source. The final slot identity and expression are not specified. |

All Chapter 1 portraits are 32 × 32 RGBA sprites. The upload guide recommends 64 × 64 portraits, but the submitted pixels are preserved and shown with integer-friendly scaling rather than being resized.

### Creator handoff needed

Provide one exact stable key for every `GB_C01_*` slot intended to display production art. Supply seven approved Background or CG assets before the background slots can be completed. If the early dialogue must show a tourist, supply a dialogue portrait or explicitly approve a named alternative; do not rely on folder order.

## Chapter 2 — Sperm Whale Rescue

Pending chapter-by-chapter review.

## Chapter 3 — Reef Warning

Pending chapter-by-chapter review. `Characters/PlayerFullbody` remains concept art and is excluded from automatic production-player binding.

## Chapter 4 — Hawksbill Guzheng

Pending chapter-by-chapter review.
