# BOCA runtime content map

This document is the technical contract between the approved chapter scripts and the Unity runtime. It does not replace or rewrite the author documents.

## Approved source files

| Chapter | Approved source | Source language | Runtime rule |
| --- | --- | --- | --- |
| 1 — Under the Oil Tide | `C:\Users\Jiang\Downloads\第一章_油潮之下.docx` (2026-09-09) | Simplified Chinese | Copy approved player-facing text verbatim. |
| 2 — Sperm Whale Rescue | `C:\Users\Jiang\Documents\xwechat_files\wxid_1wc507eee7qt12_7a96\msg\file\2026-09\第二章_抹香鲸搁浅救援(2).docx` (2026-09-10) | Simplified Chinese | Copy approved player-facing text verbatim. |
| 3 — Reef Warning | `C:\Users\Jiang\Downloads\第三章：暗礁忧鸣 (2).docx` (2026-09-11) | Simplified Chinese | Copy approved player-facing text verbatim. |
| 4 — Hawksbill Guzheng Finger Pick Case | `剧本\BOCA 游戏剧本_玳瑁古筝义甲_修改版_v3.docx` (2026-09-03) | English | Copy approved player-facing text verbatim. |

The approved Simplified Chinese source text for Chapters 1–3 and approved English source text for Chapter 4 remain unchanged in their source-language runtime tables. The project owner authorized technical translations into the other supported language. All four chapters now have complete English and Simplified Chinese tables with matching keys; translation changes do not rewrite the preserved source-language records.

## Shared interaction rules

- A choice is accepted once and the story continues. The runtime never blocks progress until the player selects a designated answer.
- Every consequential action records a persistent flag and one or more review dimensions.
- Unsafe or unsupported actions create delayed consequences in a later handoff, audit, route review, evidence review, or report review.
- The chapter-end review explains the player's record. It is not a generic pass/fail quiz.
- Players may preserve uncertainty. Blank, unknown, unverified, and insufficient-evidence states are valid when the script allows them.
- Production notes and sources guide implementation but are never presented as character dialogue.
- Medical, legal, enforcement, and animal-handling decisions remain with qualified professionals in the story.

## Chapter 1 — observation and professional handoff

1. Enter the archived case and review the limits of the player's role.
2. Observe the beach from a safe distance; record location, visible condition, people, pets, tide, and ground risk.
3. Choose how to establish the safety boundary. Unsafe proximity is remembered as a later handoff defect.
4. Build the incident record by separating direct observations, another person's account, and unknown facts.
5. Hand the original attachments and source labels to the rescue team.
6. Review three professional recovery records without turning cleaning or treatment into a player-operated minigame.
7. Prepare a release request using multi-day evidence rather than appearance alone.
8. Complete the CARE review across safety boundary, record reliability, patient handling, and recovery evidence.

Persistent review dimensions: `safety`, `record`, `care`, `recovery`.

## Chapter 2 — coordinated rescue under uncertainty

1. Verify the first report without converting estimated weight, species, injury, or cause into facts.
2. Place a public exclusion line, rescue lane, vessel waiting area, and hazard markers on the site plan.
3. Create hourly observation records while experts control treatment and medical decisions.
4. Compare tide, expert approval, rigging, route, and communications before selecting a towing plan.
5. During towing, sort time-stamped reports and decide whether to continue, pause for inspection, or stop.
6. Preserve unknown coordinates and interrupted communications instead of fabricating continuity.
7. Record the release as an observed rescue endpoint, not proof of complete recovery.
8. Audit the command log; earlier decisions surface as traceable omissions or strengths.

Persistent review dimensions: `log`, `site_order`, `animal_safety`, `tow_readiness`, `route_reliability`.

## Chapter 3 — investigation and evidence boundaries

1. Archive the original construction notice, URL, publication time, and capture time.
2. Mark public observation points, construction limits, and a safe evacuation route.
3. Record the fisher interview as attributed testimony, separating heard, seen, and inferred details.
4. Classify records as direct record, witness account, media account, official/research source, or pending verification.
5. Connect each report claim only to relevant evidence cards; unsupported claims may remain unresolved.
6. Review privacy, source, time, original link, and limitation fields before submission.
7. Assemble a four-part internal evidence package: verified facts, attributed sources, risks/questions, and unknowns.
8. Compare later institutional claims and procedural updates without presenting them as court findings.

Persistent review dimensions: `source_quality`, `claim_scope`, `privacy`, `limitations`, `package_integrity`.

## Chapter 4 — discovery, preservation, and responsible reporting

1. Search for guzheng finger picks as a normal shopper who does not initially know the meaning of “玳瑁”.
2. Choose the seller-recommended product or a synthetic product. Both routes continue and preserve their own available evidence.
3. Research the unfamiliar material name and open official protected-species sources.
4. Capture the product page, timestamps, account, address, route-appropriate messages, and official species source.
5. Route A adds the player's order, payment, logistics, packaging, and label records. Route B may use only material shared by the friend.
6. Assemble a report from verified facts and clearly attributed claims; the player cannot assert material identity, supply chain, guilt, or a guaranteed enforcement result.
7. Save the submission reference and view the later acknowledgement.
8. Show the seller and police scenes as audience-only fictionalized epilogue material, never as evidence controlled by the protagonist.

Persistent review dimensions: `source_quality`, `evidence_completeness`, `claim_scope`, `privacy`, `report_accuracy`.

## Definition of playable

A chapter is no longer a sandbox when all of the following are true:

- its approved scenes and text are present instead of writer slots;
- at least three distinct interaction patterns are used in service of the script;
- earlier actions alter later review content or available evidence;
- no branch is implemented as “wrong answer, try again”;
- completion produces a chapter-specific record and unlocks the next chapter;
- keyboard and mouse controls both work;
- all referenced art resolves; production validation fails when a referenced sprite is missing;
- automated validation can finish every route without exceptions or missing localization keys.
