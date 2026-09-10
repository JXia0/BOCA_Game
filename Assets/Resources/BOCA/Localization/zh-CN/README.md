# Chinese localization placeholder

The game currently ships English narrative tables only. When the Chinese script is approved, add files with the same names and keys as `../en-US/`. The runtime automatically falls back to English for any missing Chinese key.

The future title-screen Settings menu should call `LocalizationService.SetLanguage(GameLanguage.ChineseSimplified)` or `LocalizationService.SetLanguage(GameLanguage.English)`.
