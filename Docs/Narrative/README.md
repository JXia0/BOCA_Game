# Student-authored narrative boundary

The four Unity chapters now use the approved chapter scripts listed in `RUNTIME_CONTENT_MAP.md`. The runtime contains production dialogue, images, consequential interactions, chapter-end reviews, save progress, and chapter unlocking. It is no longer a narrative greybox.

## Ownership rule

- Student creators own the literary content and its revisions.
- The technical team must not rewrite, expand, shorten, translate, or reinterpret submitted narrative text without explicit creator approval.
- Source DOCX files remain the author documents and are not automatically imported.
- The technical team may copy approved text verbatim into localization data and connect it to stable flow IDs.
- Requested text corrections return to the creator for approval before runtime implementation.

## Runtime files

- `Assets/Resources/BOCA/Flows` controls sequence, interaction modes, evidence, state flags, and review effects.
- `Assets/Resources/BOCA/Localization/en-US` contains the approved source-language chapter text plus English system UI.
- `Assets/Resources/BOCA/Localization/zh-CN` contains Simplified Chinese system UI. Missing creator-approved chapter translations deliberately fall back to the approved source text.
- Unity scenes contain generated presentation objects and asset references; they do not duplicate editable story prose.

Chapters 1–3 currently use their approved Simplified Chinese source scripts. Chapter 4 uses its approved English source script. Full language switching of narrative text remains blocked until creator-approved translations are supplied.
