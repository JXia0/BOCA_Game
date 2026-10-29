# Incoming art migration — 2026-10-29

This document records the technical migration of the four student art deliveries into the production Unity folder structure. It does not modify or reinterpret the students' creative work.

## Result

- 138 image files were moved into chapter-owned `Backgrounds`, `Characters`, `Portraits`, `CG`, `Props`, or `UI` folders.
- Every move was checked against the pre-migration SHA-256 hash. No image bytes changed.
- Apple metadata (`.DS_Store`, `._*`, and `__MACOSX`) was removed and is now ignored by Git.
- Mojibake, spaces, Chinese punctuation, and timestamp-only filenames were replaced with stable ASCII art keys.
- No two incoming images had the same SHA-256 hash and no stable art key is duplicated.

| Chapter | Images |
| --- | ---: |
| Chapter 1 — Oil Tide | 62 |
| Chapter 2 — Sperm Whale Rescue | 33 |
| Chapter 3 — Reef Warning | 37 |
| Chapter 4 — Hawksbill Guzheng | 6 |
| **Total** | **138** |

## Integration warnings

- **Unity import is complete.** Unity 6000.5.3f1 imported all 138 images through `BOCAArtTexturePostprocessor`. A second independent check confirmed that every image is a Point-filtered, uncompressed Single Sprite at 16 PPU, with mipmaps disabled and max texture size set to 8192.
- **Chapter 2 has mixed source resolutions.** Backgrounds range from 320 × 180 to 2730 × 1535, while props range from 24 × 24 to 508 × 552. Fit them in Unity without resampling the source files.
- **Chapter 3 contains two incompatible player sets.** `Characters/Player` is the 32–50 px production pixel set. `Characters/PlayerFullbody` contains 2048 px full-body concept images and must not overwrite the production player binding.
- **Chapter 3 backgrounds are oversized.** Several are 2048 × 1152 or 2720 × 1536. Keep them as source material but use camera fit/letterboxing until the art team supplies 320 × 180 exports.
- **Chapter 4 backgrounds are small but not identical in size.** They range from 136 × 76 to 228 × 128. Use integer-friendly scaling and a common 16:9 presentation area; do not stretch each one independently.
- **Do not auto-bind by first file found.** Bind by the exact stable key in the tables below.

## Original-to-production mapping

### Chapter 1 — Oil Tide

| Original delivery path | Stable key | Production path |
| --- | --- | --- |
| `游戏资产/0_c1ch5 档案管理员/档案管理员_待机1.png` | `C01_CHAR_ARCHIVIST_IDLE_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Archivist/C01_CHAR_ARCHIVIST_IDLE_01.png` |
| `游戏资产/0_c1ch5 档案管理员/档案管理员_待机2.png` | `C01_CHAR_ARCHIVIST_IDLE_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Archivist/C01_CHAR_ARCHIVIST_IDLE_02.png` |
| `游戏资产/0_c1ch5 档案管理员/走路1（左1）.png` | `C01_CHAR_ARCHIVIST_WALK_LEFT_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Archivist/C01_CHAR_ARCHIVIST_WALK_LEFT_01.png` |
| `游戏资产/0_c1ch5 档案管理员/走路2_左2.png` | `C01_CHAR_ARCHIVIST_WALK_LEFT_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Archivist/C01_CHAR_ARCHIVIST_WALK_LEFT_02.png` |
| `游戏资产/0_c1ch5 档案管理员/走路3（右1）.png` | `C01_CHAR_ARCHIVIST_WALK_RIGHT_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Archivist/C01_CHAR_ARCHIVIST_WALK_RIGHT_01.png` |
| `游戏资产/0_c1ch5 档案管理员/走路4（右2）.png` | `C01_CHAR_ARCHIVIST_WALK_RIGHT_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Archivist/C01_CHAR_ARCHIVIST_WALK_RIGHT_02.png` |
| `游戏资产/1_c1ch3 玛丽娜/9_动作1_打招呼.png` | `C01_CHAR_MARINA_GREET` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_GREET.png` |
| `游戏资产/1_c1ch3 玛丽娜/1_待机1.png` | `C01_CHAR_MARINA_IDLE_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_IDLE_01.png` |
| `游戏资产/1_c1ch3 玛丽娜/2_待机2.png` | `C01_CHAR_MARINA_IDLE_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_IDLE_02.png` |
| `游戏资产/1_c1ch3 玛丽娜/10_动作2_搜索.png` | `C01_CHAR_MARINA_SEARCH` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_SEARCH.png` |
| `游戏资产/1_c1ch3 玛丽娜/3_说话1.png` | `C01_CHAR_MARINA_TALK_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_TALK_01.png` |
| `游戏资产/1_c1ch3 玛丽娜/4_说话2.png` | `C01_CHAR_MARINA_TALK_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_TALK_02.png` |
| `游戏资产/1_c1ch3 玛丽娜/11_走路1（左1）.png` | `C01_CHAR_MARINA_WALK_LEFT_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_WALK_LEFT_01.png` |
| `游戏资产/1_c1ch3 玛丽娜/12_走路2（左2）.png` | `C01_CHAR_MARINA_WALK_LEFT_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_WALK_LEFT_02.png` |
| `游戏资产/1_c1ch3 玛丽娜/13_走路3（右1）.png` | `C01_CHAR_MARINA_WALK_RIGHT_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_WALK_RIGHT_01.png` |
| `游戏资产/1_c1ch3 玛丽娜/14_走路4（右2）.png` | `C01_CHAR_MARINA_WALK_RIGHT_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Marina/C01_CHAR_MARINA_WALK_RIGHT_02.png` |
| `游戏资产/1_玩家/2_3_设备2相机.png` | `C01_CHAR_PLAYER_CAMERA` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_CAMERA.png` |
| `游戏资产/1_玩家/1_2_待机1.png` | `C01_CHAR_PLAYER_IDLE_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_IDLE_01.png` |
| `游戏资产/1_玩家/1_3_待机2.png` | `C01_CHAR_PLAYER_IDLE_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_IDLE_02.png` |
| `游戏资产/1_玩家/2_2_设备1手机.png` | `C01_CHAR_PLAYER_PHONE` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_PHONE.png` |
| `游戏资产/1_玩家/1_1_基础设计_说话1.png` | `C01_CHAR_PLAYER_TALK_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_TALK_01.png` |
| `游戏资产/1_玩家/2_1_说话2.png` | `C01_CHAR_PLAYER_TALK_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_TALK_02.png` |
| `游戏资产/1_玩家/3_1_侧走（左1）.png` | `C01_CHAR_PLAYER_WALK_LEFT_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_WALK_LEFT_01.png` |
| `游戏资产/1_玩家/3_2_侧走（左2）.png` | `C01_CHAR_PLAYER_WALK_LEFT_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_WALK_LEFT_02.png` |
| `游戏资产/1_玩家/3_3_侧走3（右1）.png` | `C01_CHAR_PLAYER_WALK_RIGHT_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_WALK_RIGHT_01.png` |
| `游戏资产/1_玩家/3_4_侧走4（右2）.png` | `C01_CHAR_PLAYER_WALK_RIGHT_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Player/C01_CHAR_PLAYER_WALK_RIGHT_02.png` |
| `游戏资产/0_1_c1ch4 救援工作人员x2/工作人员1_待机1.png` | `C01_CHAR_RESCUEWORKER01_IDLE_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/RescueWorker01/C01_CHAR_RESCUEWORKER01_IDLE_01.png` |
| `游戏资产/0_1_c1ch4 救援工作人员x2/工作人员1_待机2.png` | `C01_CHAR_RESCUEWORKER01_IDLE_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/RescueWorker01/C01_CHAR_RESCUEWORKER01_IDLE_02.png` |
| `游戏资产/0_1_c1ch4 救援工作人员x2/工作人员2_待机1.png` | `C01_CHAR_RESCUEWORKER02_IDLE_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/RescueWorker02/C01_CHAR_RESCUEWORKER02_IDLE_01.png` |
| `游戏资产/0_1_c1ch4 救援工作人员x2/工作人员2_待机2.png` | `C01_CHAR_RESCUEWORKER02_IDLE_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/RescueWorker02/C01_CHAR_RESCUEWORKER02_IDLE_02.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客1_待機1.png` | `C01_CHAR_TOURIST01_IDLE_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist01/C01_CHAR_TOURIST01_IDLE_01.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客1_待機2.png` | `C01_CHAR_TOURIST01_IDLE_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist01/C01_CHAR_TOURIST01_IDLE_02.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客1_走路1.png` | `C01_CHAR_TOURIST01_WALK_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist01/C01_CHAR_TOURIST01_WALK_01.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客1_走路2.png` | `C01_CHAR_TOURIST01_WALK_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist01/C01_CHAR_TOURIST01_WALK_02.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客1_走路3.png` | `C01_CHAR_TOURIST01_WALK_03` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist01/C01_CHAR_TOURIST01_WALK_03.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客1_走路4.png` | `C01_CHAR_TOURIST01_WALK_04` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist01/C01_CHAR_TOURIST01_WALK_04.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客2_待機1.png` | `C01_CHAR_TOURIST02_IDLE_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist02/C01_CHAR_TOURIST02_IDLE_01.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客2_待機2.png` | `C01_CHAR_TOURIST02_IDLE_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist02/C01_CHAR_TOURIST02_IDLE_02.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客2_走路1.png` | `C01_CHAR_TOURIST02_WALK_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist02/C01_CHAR_TOURIST02_WALK_01.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客2_走路2.png` | `C01_CHAR_TOURIST02_WALK_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist02/C01_CHAR_TOURIST02_WALK_02.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客2_走路3.png` | `C01_CHAR_TOURIST02_WALK_03` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist02/C01_CHAR_TOURIST02_WALK_03.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客2_走路4.png` | `C01_CHAR_TOURIST02_WALK_04` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist02/C01_CHAR_TOURIST02_WALK_04.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客3_待機1.png` | `C01_CHAR_TOURIST03_IDLE_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist03/C01_CHAR_TOURIST03_IDLE_01.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客3_待機2.png` | `C01_CHAR_TOURIST03_IDLE_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist03/C01_CHAR_TOURIST03_IDLE_02.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客3_走路1.png` | `C01_CHAR_TOURIST03_WALK_01` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist03/C01_CHAR_TOURIST03_WALK_01.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客3_走路2.png` | `C01_CHAR_TOURIST03_WALK_02` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist03/C01_CHAR_TOURIST03_WALK_02.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客3_走路3.png` | `C01_CHAR_TOURIST03_WALK_03` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist03/C01_CHAR_TOURIST03_WALK_03.png` |
| `游戏资产/0_1_c1ch2 游客x3/遊客3_走路4.png` | `C01_CHAR_TOURIST03_WALK_04` | `Assets/Art/Chapters/Chapter01_OilTide/Characters/Tourist03/C01_CHAR_TOURIST03_WALK_04.png` |
| `游戏资产/0_c1ch5 档案管理员/档案管理员_头像3_微笑.png` | `C01_POR_ARCHIVIST_SMILE` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Archivist/C01_POR_ARCHIVIST_SMILE.png` |
| `游戏资产/0_c1ch5 档案管理员/档案管理员_头像1_说话1.png` | `C01_POR_ARCHIVIST_TALK_01` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Archivist/C01_POR_ARCHIVIST_TALK_01.png` |
| `游戏资产/0_c1ch5 档案管理员/档案管理员_头像2_说话2.png` | `C01_POR_ARCHIVIST_TALK_02` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Archivist/C01_POR_ARCHIVIST_TALK_02.png` |
| `游戏资产/1_c1ch3 玛丽娜/5_头像1_无表情.png` | `C01_POR_MARINA_NEUTRAL` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Marina/C01_POR_MARINA_NEUTRAL.png` |
| `游戏资产/1_c1ch3 玛丽娜/6_头像2_微笑.png` | `C01_POR_MARINA_SMILE` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Marina/C01_POR_MARINA_SMILE.png` |
| `游戏资产/1_c1ch3 玛丽娜/7_头像3_说话1_大吃一惊.png` | `C01_POR_MARINA_TALK_01_SURPRISED` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Marina/C01_POR_MARINA_TALK_01_SURPRISED.png` |
| `游戏资产/1_c1ch3 玛丽娜/8_头像4_说话2_疑惑.png` | `C01_POR_MARINA_TALK_02_CONFUSED` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Marina/C01_POR_MARINA_TALK_02_CONFUSED.png` |
| `游戏资产/1_玩家/2_7_头像4_无表情.png` | `C01_POR_PLAYER_NEUTRAL` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Player/C01_POR_PLAYER_NEUTRAL.png` |
| `游戏资产/1_玩家/2_6_头像3_大吃一惊.png` | `C01_POR_PLAYER_SURPRISED` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Player/C01_POR_PLAYER_SURPRISED.png` |
| `游戏资产/1_玩家/2_4_头像1_说话1.png` | `C01_POR_PLAYER_TALK_01` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Player/C01_POR_PLAYER_TALK_01.png` |
| `游戏资产/1_玩家/2_5_头像2_说话2.png` | `C01_POR_PLAYER_TALK_02` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/Player/C01_POR_PLAYER_TALK_02.png` |
| `游戏资产/0_1_c1ch4 救援工作人员x2/工作人员1_头像1_说话1.png` | `C01_POR_RESCUEWORKER01_TALK_01` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/RescueWorker01/C01_POR_RESCUEWORKER01_TALK_01.png` |
| `游戏资产/0_1_c1ch4 救援工作人员x2/工作人员1_头像2_说话2.png` | `C01_POR_RESCUEWORKER01_TALK_02` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/RescueWorker01/C01_POR_RESCUEWORKER01_TALK_02.png` |
| `游戏资产/0_1_c1ch4 救援工作人员x2/工作人员1_头像3_思考.png` | `C01_POR_RESCUEWORKER01_THINK` | `Assets/Art/Chapters/Chapter01_OilTide/Portraits/RescueWorker01/C01_POR_RESCUEWORKER01_THINK.png` |

### Chapter 2 — Sperm Whale Rescue

| Original delivery path | Stable key | Production path |
| --- | --- | --- |
| `fanjiaming_Assets/Background_Assets/巴西街道.jpeg` | `C02_BG_BRAZIL_STREET` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_BRAZIL_STREET.jpeg` |
| `fanjiaming_Assets/Background_Assets/指挥车内部.png` | `C02_BG_COMMAND_VEHICLE_INTERIOR` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_COMMAND_VEHICLE_INTERIOR.png` |
| `fanjiaming_Assets/Background_Assets/黎明放归.png` | `C02_BG_DAWN_RELEASE` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_DAWN_RELEASE.png` |
| `fanjiaming_Assets/Background_Assets/深水航线.png` | `C02_BG_DEEP_WATER_ROUTE` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_DEEP_WATER_ROUTE.png` |
| `fanjiaming_Assets/Background_Assets/应急值班席.jpeg` | `C02_BG_EMERGENCY_DESK` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_EMERGENCY_DESK.jpeg` |
| `fanjiaming_Assets/Background_Assets/福建传统渔村.jpeg` | `C02_BG_FUJIAN_FISHING_VILLAGE` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_FUJIAN_FISHING_VILLAGE.jpeg` |
| `fanjiaming_Assets/Background_Assets/夜间海面拖带.png` | `C02_BG_NIGHT_TOWING` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_NIGHT_TOWING.png` |
| `fanjiaming_Assets/Background_Assets/办公室背景.jpeg` | `C02_BG_OFFICE` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_OFFICE.jpeg` |
| `fanjiaming_Assets/Background_Assets/室内背景（关灯）.jpeg` | `C02_BG_ROOM_LIGHTS_OFF` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_ROOM_LIGHTS_OFF.jpeg` |
| `fanjiaming_Assets/Background_Assets/室内背景（开灯）.jpeg` | `C02_BG_ROOM_LIGHTS_ON` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_ROOM_LIGHTS_ON.jpeg` |
| `fanjiaming_Assets/Background_Assets/日落海面.png` | `C02_BG_SUNSET_SEA` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_SUNSET_SEA.png` |
| `fanjiaming_Assets/Background_Assets/海底档案馆.jpeg` | `C02_BG_UNDERSEA_ARCHIVE` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_UNDERSEA_ARCHIVE.jpeg` |
| `fanjiaming_Assets/Background_Assets/抹香鲸滩涂（有水）.jpeg` | `C02_BG_WHALE_MUDFLAT_FLOODED` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_WHALE_MUDFLAT_FLOODED.jpeg` |
| `fanjiaming_Assets/Background_Assets/抹香鲸滩涂（退潮）.jpeg` | `C02_BG_WHALE_MUDFLAT_LOW_TIDE` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Backgrounds/C02_BG_WHALE_MUDFLAT_LOW_TIDE.jpeg` |
| `fanjiaming_Assets/渔民NPC_美术资产/渔民NPC待机1.png` | `C02_CHAR_FISHERMAN_IDLE_01` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Characters/Fisherman/C02_CHAR_FISHERMAN_IDLE_01.png` |
| `fanjiaming_Assets/渔民NPC_美术资产/渔民NPC待机2.png` | `C02_CHAR_FISHERMAN_IDLE_02` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Characters/Fisherman/C02_CHAR_FISHERMAN_IDLE_02.png` |
| `fanjiaming_Assets/渔民NPC_美术资产/渔民NPC张嘴.png` | `C02_CHAR_FISHERMAN_TALK` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Characters/Fisherman/C02_CHAR_FISHERMAN_TALK.png` |
| `fanjiaming_Assets/渔民NPC_美术资产/渔民NPC走路1.png` | `C02_CHAR_FISHERMAN_WALK_01` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Characters/Fisherman/C02_CHAR_FISHERMAN_WALK_01.png` |
| `fanjiaming_Assets/渔民NPC_美术资产/渔民NPC走路2.png` | `C02_CHAR_FISHERMAN_WALK_02` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Characters/Fisherman/C02_CHAR_FISHERMAN_WALK_02.png` |
| `fanjiaming_Assets/玩家表情大头像/坚定表情.png` | `C02_POR_PLAYER_DETERMINED` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Portraits/Player/C02_POR_PLAYER_DETERMINED.png` |
| `fanjiaming_Assets/玩家表情大头像/普通表情.png` | `C02_POR_PLAYER_NEUTRAL` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Portraits/Player/C02_POR_PLAYER_NEUTRAL.png` |
| `fanjiaming_Assets/玩家表情大头像/思考表情.png` | `C02_POR_PLAYER_THINKING` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Portraits/Player/C02_POR_PLAYER_THINKING.png` |
| `fanjiaming_Assets/玩家表情大头像/担忧表情.png` | `C02_POR_PLAYER_WORRIED` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Portraits/Player/C02_POR_PLAYER_WORRIED.png` |
| `fanjiaming_Assets/道具_美术资产/道具-竹篓.png` | `C02_PROP_BAMBOO_BASKET` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_BAMBOO_BASKET.png` |
| `fanjiaming_Assets/道具_美术资产/道具-浮标.png` | `C02_PROP_BUOYS` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_BUOYS.png` |
| `fanjiaming_Assets/道具_美术资产/道具-记录板.png` | `C02_PROP_CLIPBOARD` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_CLIPBOARD.png` |
| `fanjiaming_Assets/道具_美术资产/道具-文件夹.png` | `C02_PROP_FILE_FOLDER` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_FILE_FOLDER.png` |
| `fanjiaming_Assets/道具_美术资产/道具-鱼架.png` | `C02_PROP_FISH_RACK` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_FISH_RACK.png` |
| `fanjiaming_Assets/道具_美术资产/道具-渔网.png` | `C02_PROP_FISHING_NET` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_FISHING_NET.png` |
| `fanjiaming_Assets/道具_美术资产/道具-杯子.png` | `C02_PROP_MUG` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_MUG.png` |
| `fanjiaming_Assets/道具_美术资产/道具-打印机.png` | `C02_PROP_PRINTER` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_PRINTER.png` |
| `fanjiaming_Assets/道具_美术资产/道具-舢板.png` | `C02_PROP_SAMPAN` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_SAMPAN.png` |
| `fanjiaming_Assets/道具_美术资产/道具-白板.png` | `C02_PROP_WHITEBOARD` | `Assets/Art/Chapters/Chapter02_SpermWhaleRescue/Props/C02_PROP_WHITEBOARD.png` |

### Chapter 3 — Reef Warning

| Original delivery path | Stable key | Production path |
| --- | --- | --- |
| `美术资产/image_1792245819048.png` | `C03_BG_BEACH_SURVEY` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_BEACH_SURVEY.png` |
| `美术资产/image_1792245823557.png` | `C03_BG_INVESTIGATION_DESK_INITIAL` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_INVESTIGATION_DESK_INITIAL.png` |
| `美术资产/image_1792245847298.png` | `C03_BG_INVESTIGATION_DESK_MAP` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_INVESTIGATION_DESK_MAP.png` |
| `美术资产/image_1792245850000.png` | `C03_BG_INVESTIGATION_DESK_REPORT` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_INVESTIGATION_DESK_REPORT.png` |
| `美术资产/image_1792245827288.png` | `C03_BG_INVESTIGATION_DESK_SPECIMEN` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_INVESTIGATION_DESK_SPECIMEN.png` |
| `美术资产/image_1791660749880.jpg` | `C03_BG_LAB_INTERIOR` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_LAB_INTERIOR.jpg` |
| `美术资产/image_1792245834473.jpg` | `C03_BG_LAB_RESEARCHER` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_LAB_RESEARCHER.jpg` |
| `美术资产/image_1792245841022.png` | `C03_BG_NIGHT_OFFICE` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_NIGHT_OFFICE.png` |
| `美术资产/image_1792245843738.png` | `C03_BG_VESSEL_DECK` | `Assets/Art/Chapters/Chapter03_ReefWarning/Backgrounds/C03_BG_VESSEL_DECK.png` |
| `美术资产/image_1792245837896.png` | `C03_CG_COASTAL_OBSERVATION` | `Assets/Art/Chapters/Chapter03_ReefWarning/CG/C03_CG_COASTAL_OBSERVATION.png` |
| `美术资产/image_1792245830798.png` | `C03_CG_EVIDENCE_REVIEW` | `Assets/Art/Chapters/Chapter03_ReefWarning/CG/C03_CG_EVIDENCE_REVIEW.png` |
| `美术资产/image_1791655205990.png` | `C03_CG_SUNSET_01` | `Assets/Art/Chapters/Chapter03_ReefWarning/CG/C03_CG_SUNSET_01.png` |
| `美术资产/image_1791655209399.png` | `C03_CG_SUNSET_02` | `Assets/Art/Chapters/Chapter03_ReefWarning/CG/C03_CG_SUNSET_02.png` |
| `美术资产/image_1791655212208.png` | `C03_CG_SUNSET_03` | `Assets/Art/Chapters/Chapter03_ReefWarning/CG/C03_CG_SUNSET_03.png` |
| `美术资产/image_1791655215792.png` | `C03_CG_SUNSET_04` | `Assets/Art/Chapters/Chapter03_ReefWarning/CG/C03_CG_SUNSET_04.png` |
| `美术资产/image_1791655193376.png` | `C03_CHAR_PENGUIN_STATE_01` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Penguin/C03_CHAR_PENGUIN_STATE_01.png` |
| `美术资产/image_1791655196708.png` | `C03_CHAR_PENGUIN_STATE_02` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Penguin/C03_CHAR_PENGUIN_STATE_02.png` |
| `美术资产/image_1791655199444.png` | `C03_CHAR_PENGUIN_STATE_03` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Penguin/C03_CHAR_PENGUIN_STATE_03.png` |
| `游戏资产/3_玩家/1_4_待机1（提水桶）.png` | `C03_CHAR_PLAYER_IDLE_BUCKET` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_IDLE_BUCKET.png` |
| `游戏资产/3_玩家/1_5_待机2（招手_制止）.png` | `C03_CHAR_PLAYER_STOP_GESTURE` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_STOP_GESTURE.png` |
| `游戏资产/3_玩家/1_1_基础设计_说话1.png` | `C03_CHAR_PLAYER_TALK_01` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_TALK_01.png` |
| `游戏资产/3_玩家/1_2_说话2.png` | `C03_CHAR_PLAYER_TALK_02` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_TALK_02.png` |
| `游戏资产/3_玩家/6_走路1（左1）.png` | `C03_CHAR_PLAYER_WALK_LEFT_01` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_WALK_LEFT_01.png` |
| `游戏资产/3_玩家/7_走路2（左2）.png` | `C03_CHAR_PLAYER_WALK_LEFT_02` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_WALK_LEFT_02.png` |
| `游戏资产/3_玩家/8_走路3（右1）.png` | `C03_CHAR_PLAYER_WALK_RIGHT_01` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_WALK_RIGHT_01.png` |
| `游戏资产/3_玩家/9_走路4（右2）.png` | `C03_CHAR_PLAYER_WALK_RIGHT_02` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_WALK_RIGHT_02.png` |
| `游戏资产/3_玩家/1_3_担忧.png` | `C03_CHAR_PLAYER_WORRIED` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/Player/C03_CHAR_PLAYER_WORRIED.png` |
| `美术资产/image_1791661014776.png` | `C03_CHAR_PLAYER_FULLBODY_BLINK` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/PlayerFullbody/C03_CHAR_PLAYER_FULLBODY_BLINK.png` |
| `美术资产/image_1791661001444.png` | `C03_CHAR_PLAYER_FULLBODY_IDLE` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/PlayerFullbody/C03_CHAR_PLAYER_FULLBODY_IDLE.png` |
| `美术资产/image_1791661020520.png` | `C03_CHAR_PLAYER_FULLBODY_SMILE` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/PlayerFullbody/C03_CHAR_PLAYER_FULLBODY_SMILE.png` |
| `美术资产/image_1791661004960.png` | `C03_CHAR_PLAYER_FULLBODY_WALK_01` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/PlayerFullbody/C03_CHAR_PLAYER_FULLBODY_WALK_01.png` |
| `美术资产/image_1791661009290.png` | `C03_CHAR_PLAYER_FULLBODY_WALK_02` | `Assets/Art/Chapters/Chapter03_ReefWarning/Characters/PlayerFullbody/C03_CHAR_PLAYER_FULLBODY_WALK_02.png` |
| `游戏资产/3_玩家/2_头像1_无表情.png` | `C03_POR_PLAYER_NEUTRAL` | `Assets/Art/Chapters/Chapter03_ReefWarning/Portraits/Player/C03_POR_PLAYER_NEUTRAL.png` |
| `游戏资产/3_玩家/5_头像4_撇嘴.png` | `C03_POR_PLAYER_POUT` | `Assets/Art/Chapters/Chapter03_ReefWarning/Portraits/Player/C03_POR_PLAYER_POUT.png` |
| `游戏资产/3_玩家/3_头像2_说话1_疑惑.png` | `C03_POR_PLAYER_TALK_01_CONFUSED` | `Assets/Art/Chapters/Chapter03_ReefWarning/Portraits/Player/C03_POR_PLAYER_TALK_01_CONFUSED.png` |
| `游戏资产/3_玩家/4_头像3_说话2_吃惊.png` | `C03_POR_PLAYER_TALK_02_SURPRISED` | `Assets/Art/Chapters/Chapter03_ReefWarning/Portraits/Player/C03_POR_PLAYER_TALK_02_SURPRISED.png` |
| `美术资产/image_1791660741709.png` | `C03_UI_PENGUIN_STATUS` | `Assets/Art/Chapters/Chapter03_ReefWarning/UI/C03_UI_PENGUIN_STATUS.png` |

### Chapter 4 — Hawksbill Guzheng

| Original delivery path | Stable key | Production path |
| --- | --- | --- |
| `第四章场景assets/音乐厅舞台（晚上）.png` | `C04_BG_CONCERT_STAGE_NIGHT` | `Assets/Art/Chapters/Chapter04_HawksbillGuzheng/Backgrounds/C04_BG_CONCERT_STAGE_NIGHT.png` |
| `第四章场景assets/玩家卧室（白天）.png` | `C04_BG_PLAYER_BEDROOM_DAY` | `Assets/Art/Chapters/Chapter04_HawksbillGuzheng/Backgrounds/C04_BG_PLAYER_BEDROOM_DAY.png` |
| `第四章场景assets/玩家卧室（晚上）.png` | `C04_BG_PLAYER_BEDROOM_NIGHT` | `Assets/Art/Chapters/Chapter04_HawksbillGuzheng/Backgrounds/C04_BG_PLAYER_BEDROOM_NIGHT.png` |
| `第四章场景assets/卖家打包间.png` | `C04_BG_SELLER_PACKING_ROOM` | `Assets/Art/Chapters/Chapter04_HawksbillGuzheng/Backgrounds/C04_BG_SELLER_PACKING_ROOM.png` |
| `第四章场景assets/调查蒙太奇三个小画面.png` | `C04_CG_INVESTIGATION_MONTAGE` | `Assets/Art/Chapters/Chapter04_HawksbillGuzheng/CG/C04_CG_INVESTIGATION_MONTAGE.png` |
| `第四章场景assets/打包间·警察到场.png` | `C04_CG_PACKING_ROOM_POLICE_ARRIVAL` | `Assets/Art/Chapters/Chapter04_HawksbillGuzheng/CG/C04_CG_PACKING_ROOM_POLICE_ARRIVAL.png` |

## Unity verification

1. Unity 6000.5.3f1 opened successfully through Unity Hub with a valid Personal license.
2. `BOCA > Art > Reimport Production Art` reimported all 138 images and rebuilt all four chapter catalogs.
3. The four catalogs contain exactly 62, 33, 37, and 6 entries respectively, matching the source inventory.
4. No duplicate-key, C# compilation, or asset-import error was reported.

The production art is ready for scene wiring. Use the stable keys above and follow the integration warnings rather than selecting assets by folder order.
