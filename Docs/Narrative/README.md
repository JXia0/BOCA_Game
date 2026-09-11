# Student-authored narrative boundary

The four Unity chapters currently contain technical greybox placeholders only. No final story, dialogue, factual claim, character interpretation, or dramatic direction is implemented in the runtime.

## Ownership rule

- Student creators own the literary content and its revisions.
- The technical team must not rewrite, expand, shorten, translate, or reinterpret submitted narrative text without explicit creator approval.
- Source DOCX files are retained as author documents and are not automatically imported.
- When a chapter script is approved, the technical team may copy the approved text verbatim into localization data and connect it to stable scene slots.
- Any requested text correction must be returned to the creator for approval before implementation.

## Technical files

- `Assets/Resources/BOCA/Flows` controls sequence, branches, state flags, and asset slots.
- `Assets/Resources/BOCA/Localization/en-US` currently contains visible greybox labels only.
- `Assets/Resources/BOCA/Localization/zh-CN` remains reserved for later creator-approved localization.
- Unity scenes contain reusable presentation objects and do not contain final prose.

The placeholder IDs are technical contracts, not story instructions. Writers may change the number or function of scenes later; the technical team will adapt the flow only after receiving an approved content map.
