# Student-authored narrative boundary

The four Unity chapters now use the approved chapter scripts listed in `RUNTIME_CONTENT_MAP.md`. The runtime contains production dialogue, images, consequential interactions, chapter-end reviews, save progress, and chapter unlocking. It is no longer a narrative greybox.

## Ownership rule

- Student creators own the literary content and its revisions.
- The technical team must not rewrite, expand, shorten, translate, or reinterpret submitted narrative text without explicit authorization from the project owner and content team.
- Source DOCX files remain the author documents and are not automatically imported.
- The technical team may copy approved text verbatim into localization data and connect it to stable flow IDs.
- Requested text corrections return to the creator for approval before runtime implementation.

## Runtime files

- `Assets/Resources/BOCA/Flows` controls sequence, interaction modes, evidence, state flags, and review effects.
- `Assets/Resources/BOCA/Localization/en-US` contains complete English system UI and chapter text.
- `Assets/Resources/BOCA/Localization/zh-CN` contains complete Simplified Chinese system UI and chapter text.
- Unity scenes contain generated presentation objects and asset references; they do not duplicate editable story prose.

The approved Simplified Chinese source text for Chapters 1–3 and approved English source text for Chapter 4 remain preserved in their source-language tables. The project owner authorized the corresponding technical translations for runtime integration. English and Simplified Chinese now switch across the system UI and all four chapters; the validator requires both locales to keep identical key sets.
