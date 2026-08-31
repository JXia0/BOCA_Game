using System;
using System.Collections.Generic;
using UnityEngine;

namespace BOCA.Chapter1
{
    /// <summary>
    /// A self-contained, asset-free playable prototype for Chapter 1.
    /// It is intentionally built with IMGUI so the narrative and interaction loop
    /// can be evaluated before final art, animation, audio, and UI prefabs exist.
    /// </summary>
    public sealed class Chapter1PlayablePrototype : MonoBehaviour
    {
        private const float DesignWidth = 1920f;
        private const float DesignHeight = 1080f;

        private enum Stage
        {
            Title,
            Archive,
            BeachObserve,
            BeachDecision,
            QuietPerimeter,
            Handoff,
            Rehabilitation,
            RecoveryCheck,
            Release,
            Results
        }

        private sealed class RehabCase
        {
            public string Title;
            public string Status;
            public string[] Choices;
            public int CorrectChoice;
            public string Explanation;
        }

        private Stage stage = Stage.Title;
        private Font uiFont;
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle headingStyle;
        private GUIStyle bodyStyle;
        private GUIStyle smallStyle;
        private GUIStyle buttonStyle;
        private GUIStyle dangerButtonStyle;
        private GUIStyle cardStyle;
        private GUIStyle centeredStyle;
        private GUIStyle tagStyle;

        private readonly HashSet<int> observedClues = new HashSet<int>();
        private readonly HashSet<int> clearedDisturbances = new HashSet<int>();
        private readonly List<int> handoffOrder = new List<int>();
        private readonly string[] clueNames = { "虚弱的企鹅", "羽毛上的油污", "正在上涨的潮水", "围观的人和宠物" };
        private readonly string[] clueDetails =
        {
            "它趴在潮线附近，呼吸较快，没有主动逃跑。保持距离，不要把‘不反抗’误认为亲近。",
            "胸腹羽毛结成深色小束，失去正常蓬松感。这可能影响防水和保温能力。",
            "浪线正在靠近。地点和潮汐是救援人员判断风险的重要信息。",
            "人声、手机闪光和宠物都在增加压力。先控制环境，不要围着动物拍照。"
        };

        private readonly string[] handoffSteps =
        {
            "降低现场干扰",
            "记录地点与可见状态",
            "联系专业救助机构",
            "保持距离并等待交接"
        };

        private readonly RehabCase[] rehabCases =
        {
            new RehabCase
            {
                Title = "护理记录 A",
                Status = "工作人员正在进行初步评估。企鹅仍然虚弱，环境需要保持安静。",
                Choices = new[] { "催促立刻清洗", "记录观察结果并减少干扰", "拿鱼靠近它试试" },
                CorrectChoice = 1,
                Explanation = "正确。玩家只协助记录和环境管理；具体治疗由专业人员决定。"
            },
            new RehabCase
            {
                Title = "护理记录 B",
                Status = "外观比昨天干净，但它的活动力和羽毛状态仍在评估。",
                Choices = new[] { "因为看起来干净就放生", "打开围栏让它自己选择", "继续记录，等待专业评估" },
                CorrectChoice = 2,
                Explanation = "正确。‘看起来干净’不等于已经具备野外生存条件。"
            },
            new RehabCase
            {
                Title = "护理记录 C",
                Status = "团队准备转移企鹅。器材清单已经由负责人确认。",
                Choices = new[] { "按清单搬运密闭物资", "私自添加食物", "为了拍照打开运输箱" },
                CorrectChoice = 0,
                Explanation = "正确。只完成被明确分配的支持工作，不擅自接触动物。"
            }
        };

        private int archivePage;
        private int selectedClue = -1;
        private int mistakes;
        private int careScore;
        private string feedback = string.Empty;
        private float feedbackUntil;
        private float noise = 78f;
        private float perimeterStart;
        private int rehabIndex;
        private int recoveryRound;
        private bool releaseGateReady;
        private Vector2 waveOffset;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindFirstObjectByType<Chapter1PlayablePrototype>() != null)
            {
                return;
            }

            var host = new GameObject("Chapter 1 - Oil on the Tide (Playable Prototype)");
            host.AddComponent<Chapter1PlayablePrototype>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            uiFont = Font.CreateDynamicFontFromOSFont(
                new[] { "Microsoft YaHei UI", "Microsoft YaHei", "SimHei", "Arial" }, 28);
            EnterStage(Stage.Title);
        }

        private void Update()
        {
            waveOffset.x += Time.deltaTime * 28f;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (stage == Stage.Title)
                {
                    return;
                }

                ResetChapter();
            }
        }

        private void OnGUI()
        {
            if (titleStyle == null)
            {
                BuildStyles();
            }

            var scale = Mathf.Min(Screen.width / DesignWidth, Screen.height / DesignHeight);
            var offsetX = (Screen.width - DesignWidth * scale) * 0.5f;
            var offsetY = (Screen.height - DesignHeight * scale) * 0.5f;
            GUI.matrix = Matrix4x4.TRS(new Vector3(offsetX, offsetY, 0f), Quaternion.identity, Vector3.one * scale);

            DrawBackdrop();
            DrawTopBar();

            switch (stage)
            {
                case Stage.Title: DrawTitle(); break;
                case Stage.Archive: DrawArchive(); break;
                case Stage.BeachObserve: DrawBeachObserve(); break;
                case Stage.BeachDecision: DrawBeachDecision(); break;
                case Stage.QuietPerimeter: DrawQuietPerimeter(); break;
                case Stage.Handoff: DrawHandoff(); break;
                case Stage.Rehabilitation: DrawRehabilitation(); break;
                case Stage.RecoveryCheck: DrawRecoveryCheck(); break;
                case Stage.Release: DrawRelease(); break;
                case Stage.Results: DrawResults(); break;
            }

            if (!string.IsNullOrEmpty(feedback) && Time.unscaledTime < feedbackUntil)
            {
                DrawToast(feedback);
            }
        }

        private void BuildStyles()
        {
            titleStyle = MakeStyle(68, FontStyle.Bold, new Color(0.94f, 0.98f, 1f), TextAnchor.MiddleLeft);
            subtitleStyle = MakeStyle(30, FontStyle.Normal, new Color(0.75f, 0.9f, 0.95f), TextAnchor.MiddleLeft);
            headingStyle = MakeStyle(38, FontStyle.Bold, new Color(0.08f, 0.19f, 0.24f), TextAnchor.MiddleLeft);
            bodyStyle = MakeStyle(27, FontStyle.Normal, new Color(0.11f, 0.18f, 0.2f), TextAnchor.UpperLeft);
            bodyStyle.wordWrap = true;
            bodyStyle.padding = new RectOffset(18, 18, 14, 14);
            smallStyle = MakeStyle(21, FontStyle.Normal, new Color(0.25f, 0.36f, 0.39f), TextAnchor.UpperLeft);
            smallStyle.wordWrap = true;
            centeredStyle = MakeStyle(28, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            tagStyle = MakeStyle(20, FontStyle.Bold, new Color(0.1f, 0.32f, 0.37f), TextAnchor.MiddleCenter);

            buttonStyle = MakeStyle(26, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            buttonStyle.normal.background = MakeTexture(new Color(0.06f, 0.43f, 0.52f));
            buttonStyle.hover.background = MakeTexture(new Color(0.08f, 0.55f, 0.62f));
            buttonStyle.active.background = MakeTexture(new Color(0.04f, 0.34f, 0.41f));
            buttonStyle.padding = new RectOffset(22, 22, 14, 14);

            dangerButtonStyle = new GUIStyle(buttonStyle);
            dangerButtonStyle.normal.background = MakeTexture(new Color(0.55f, 0.25f, 0.18f));
            dangerButtonStyle.hover.background = MakeTexture(new Color(0.68f, 0.3f, 0.2f));

            cardStyle = new GUIStyle(bodyStyle);
            cardStyle.normal.background = MakeTexture(new Color(0.94f, 0.96f, 0.94f, 0.97f));
            cardStyle.padding = new RectOffset(28, 28, 24, 24);
        }

        private GUIStyle MakeStyle(int size, FontStyle fontStyle, Color color, TextAnchor alignment)
        {
            return new GUIStyle(GUI.skin.label)
            {
                font = uiFont,
                fontSize = size,
                fontStyle = fontStyle,
                normal = { textColor = color },
                alignment = alignment
            };
        }

        private static Texture2D MakeTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void DrawBackdrop()
        {
            DrawRect(new Rect(0, 0, DesignWidth, DesignHeight), new Color(0.84f, 0.92f, 0.91f));
            DrawRect(new Rect(0, 560, DesignWidth, 520), new Color(0.77f, 0.67f, 0.48f));
            DrawRect(new Rect(0, 405, DesignWidth, 210), new Color(0.17f, 0.52f, 0.62f));
            DrawRect(new Rect(0, 505, DesignWidth, 95), new Color(0.32f, 0.68f, 0.72f));

            for (var i = 0; i < 12; i++)
            {
                var x = Mathf.Repeat(i * 180f + waveOffset.x, DesignWidth + 200f) - 100f;
                DrawRect(new Rect(x, 495 + Mathf.Sin(i + Time.time) * 7f, 90, 5), new Color(0.88f, 0.97f, 0.96f));
            }

            DrawRect(new Rect(80, 85, 160, 160), new Color(1f, 0.78f, 0.32f, 0.72f));
            DrawPenguin(new Rect(1515, 520, 190, 270), stage >= Stage.Release);
        }

        private void DrawTopBar()
        {
            DrawRect(new Rect(0, 0, DesignWidth, 70), new Color(0.035f, 0.12f, 0.16f, 0.94f));
            GUI.Label(new Rect(38, 12, 980, 46), "BOCA｜第一章：油潮之下｜可玩灰盒", smallStyle);

            if (stage != Stage.Title)
            {
                GUI.Label(new Rect(1310, 12, 360, 46), $"CARE  {careScore:000}", centeredStyle);
                if (GUI.Button(new Rect(1720, 10, 160, 48), "重新开始", dangerButtonStyle))
                {
                    ResetChapter();
                }
            }
        }

        private void DrawTitle()
        {
            DrawRect(new Rect(0, 70, DesignWidth, 1010), new Color(0.02f, 0.11f, 0.16f, 0.66f));
            GUI.Label(new Rect(170, 250, 1300, 100), "油潮之下", titleStyle);
            GUI.Label(new Rect(175, 355, 1250, 70), "第一章｜企鹅救助与耐心的关怀", subtitleStyle);
            GUI.Label(new Rect(175, 470, 980, 170),
                "这不是‘亲手治好动物’的英雄故事。\n你的任务是观察、报告、降低干扰，并把救助交给专业人员。",
                MakeLightBodyStyle());

            if (GUI.Button(new Rect(175, 700, 390, 88), "开始体验", buttonStyle))
            {
                EnterStage(Stage.Archive);
            }

            GUI.Label(new Rect(175, 825, 950, 80), "操作：鼠标点击｜Esc：返回标题｜建议全屏 16:9", subtitleStyle);
        }

        private void DrawArchive()
        {
            DrawPanel(new Rect(180, 150, 1050, 760));
            GUI.Label(new Rect(240, 195, 900, 60), archivePage == 0 ? "档案：一只获救的企鹅" : "事实与游戏边界", headingStyle);

            var text = archivePage == 0
                ? "2011 年，巴西海岸附近发现了一只满身油污、身体虚弱的麦哲伦企鹅。它后来被称为 Dindim。经过专业救助、恢复和放归后，它再次出现在救助者附近的故事被广泛报道。\n\n本章以这一事件为事实锚点，但玩家角色、对白、地点细节和时间跨度均为游戏化改编。"
                : "现实中的受污染野生动物救助需要训练、设备、评估和长期记录。\n\n游戏不会教你自行清洗、喂食、抓捕或放生。遇到真实动物时：保持距离、减少干扰、记录地点，并联系当地专业救助机构。";
            GUI.Label(new Rect(235, 285, 900, 420), text, bodyStyle);

            if (archivePage == 0)
            {
                if (GUI.Button(new Rect(760, 760, 360, 78), "继续阅读", buttonStyle)) archivePage = 1;
            }
            else if (GUI.Button(new Rect(760, 760, 360, 78), "前往海滩", buttonStyle))
            {
                EnterStage(Stage.BeachObserve);
            }

            DrawObjective("目标：先观察，不接触。", "章节 01 / 06");
        }

        private void DrawBeachObserve()
        {
            DrawPanel(new Rect(90, 100, 610, 835));
            GUI.Label(new Rect(135, 145, 520, 60), "潮线上的异常", headingStyle);
            GUI.Label(new Rect(135, 220, 515, 140), "点击场景中的四个观察点，建立给救援人员的现场记录。", bodyStyle);

            for (var i = 0; i < clueNames.Length; i++)
            {
                var found = observedClues.Contains(i);
                GUI.Label(new Rect(145, 380 + i * 65, 500, 50), $"{(found ? "✓" : "○")}  {clueNames[i]}", bodyStyle);
            }

            if (selectedClue >= 0)
            {
                GUI.Label(new Rect(130, 670, 530, 170), clueDetails[selectedClue], cardStyle);
            }

            DrawClueButton(0, new Rect(1490, 545, 240, 250), "观察企鹅");
            DrawClueButton(1, new Rect(1370, 770, 290, 90), "检查可见油污");
            DrawClueButton(2, new Rect(920, 450, 300, 90), "记录潮线");
            DrawClueButton(3, new Rect(730, 700, 360, 105), "观察人群与宠物");

            if (observedClues.Count == clueNames.Length && GUI.Button(new Rect(1310, 900, 420, 82), "完成观察记录", buttonStyle))
            {
                careScore += 20;
                EnterStage(Stage.BeachDecision);
            }

            DrawObjective($"观察点：{observedClues.Count} / {clueNames.Length}", "章节 02 / 06");
        }

        private void DrawClueButton(int index, Rect rect, string label)
        {
            var oldColor = GUI.color;
            GUI.color = observedClues.Contains(index) ? new Color(0.65f, 1f, 0.75f) : Color.white;
            if (GUI.Button(rect, label, buttonStyle))
            {
                selectedClue = index;
                if (observedClues.Add(index)) careScore += 3;
            }
            GUI.color = oldColor;
        }

        private void DrawBeachDecision()
        {
            DrawPanel(new Rect(240, 150, 1030, 760));
            GUI.Label(new Rect(295, 200, 900, 60), "你现在应该怎么做？", headingStyle);
            GUI.Label(new Rect(295, 285, 900, 120), "企鹅虚弱、羽毛可能被油污染，潮水正在上涨，周围还有围观者。", bodyStyle);

            if (GUI.Button(new Rect(320, 460, 860, 90), "A｜把企鹅抱到阴凉处，再自己清洗", dangerButtonStyle))
            {
                WrongChoice("这会增加应激，也可能让你和动物接触污染物。不要自行抓捕或清洗。", 8);
            }
            if (GUI.Button(new Rect(320, 575, 860, 90), "B｜把它推回海里，让它游走", dangerButtonStyle))
            {
                WrongChoice("虚弱或失去防水能力的企鹅可能无法在海里生存。外观不能代替专业评估。", 8);
            }
            if (GUI.Button(new Rect(320, 690, 860, 90), "C｜保持距离，报告地点和观察结果", buttonStyle))
            {
                careScore += 20;
                EnterStage(Stage.QuietPerimeter);
                ShowFeedback("正确。救援人员已收到位置、潮汐和动物状态。", 2.3f);
            }

            DrawObjective("选择安全、可执行的第一步。", "章节 02 / 06");
        }

        private void DrawQuietPerimeter()
        {
            DrawPanel(new Rect(80, 115, 550, 820));
            GUI.Label(new Rect(125, 160, 450, 60), "建立安静边界", headingStyle);
            GUI.Label(new Rect(125, 235, 450, 165), "救援人员正在赶来。点击四个干扰源，引导人群后退、关闭闪光灯，并让宠物离开潮线。", bodyStyle);

            GUI.Label(new Rect(125, 445, 250, 45), "环境压力", smallStyle);
            DrawMeter(new Rect(125, 495, 430, 40), noise / 100f, noise < 35f ? new Color(0.15f, 0.65f, 0.44f) : new Color(0.82f, 0.36f, 0.22f));
            GUI.Label(new Rect(125, 550, 430, 100), $"已处理：{clearedDisturbances.Count} / 4\n目标：压力降到 35 以下", bodyStyle);

            DrawDisturbance(0, new Rect(785, 660, 230, 110), "闪光灯");
            DrawDisturbance(1, new Rect(1050, 710, 245, 110), "靠近拍照");
            DrawDisturbance(2, new Rect(720, 820, 255, 110), "未牵引宠物");
            DrawDisturbance(3, new Rect(1100, 845, 250, 110), "大声呼喊");

            if (clearedDisturbances.Count == 4 && noise <= 35f && GUI.Button(new Rect(1430, 850, 350, 85), "等待救援队", buttonStyle))
            {
                careScore += Mathf.Max(8, 24 - Mathf.FloorToInt(Time.time - perimeterStart));
                EnterStage(Stage.Handoff);
            }

            DrawObjective("降低干扰，不围观。", "章节 03 / 06");
        }

        private void DrawDisturbance(int index, Rect rect, string label)
        {
            if (clearedDisturbances.Contains(index))
            {
                GUI.Label(rect, "✓ 已处理", centeredStyle);
                return;
            }

            if (GUI.Button(rect, label, dangerButtonStyle))
            {
                clearedDisturbances.Add(index);
                noise = Mathf.Max(18f, 78f - clearedDisturbances.Count * 15f);
                careScore += 3;
                ShowFeedback(index == 2 ? "请主人牵引宠物并离开潮线。" : "用平静、明确的话请对方后退。", 1.8f);
            }
        }

        private void DrawHandoff()
        {
            DrawPanel(new Rect(180, 130, 1120, 800));
            GUI.Label(new Rect(230, 175, 1010, 60), "把现场信息交给救援人员", headingStyle);
            GUI.Label(new Rect(230, 250, 980, 100), "按正确顺序点击行动卡。错误不会失败，但会重置顺序。", bodyStyle);

            for (var i = 0; i < handoffSteps.Length; i++)
            {
                var selected = handoffOrder.Contains(i);
                var rect = new Rect(260, 390 + i * 105, 950, 78);
                var old = GUI.color;
                if (selected) GUI.color = new Color(0.65f, 1f, 0.75f);
                if (GUI.Button(rect, handoffSteps[i], buttonStyle) && !selected)
                {
                    SelectHandoffStep(i);
                }
                GUI.color = old;
            }

            if (handoffOrder.Count == handoffSteps.Length && GUI.Button(new Rect(1410, 770, 340, 90), "完成专业交接", buttonStyle))
            {
                careScore += 18;
                EnterStage(Stage.Rehabilitation);
            }

            DrawObjective($"交接步骤：{handoffOrder.Count} / 4", "章节 04 / 06");
        }

        private void SelectHandoffStep(int index)
        {
            if (index == handoffOrder.Count)
            {
                handoffOrder.Add(index);
                careScore += 2;
                return;
            }

            mistakes++;
            handoffOrder.Clear();
            ShowFeedback("顺序需要重来：先控制风险并记录，再联系和等待专业人员。", 2.4f);
        }

        private void DrawRehabilitation()
        {
            var current = rehabCases[rehabIndex];
            DrawPanel(new Rect(175, 130, 1170, 810));
            GUI.Label(new Rect(230, 180, 1020, 60), current.Title, headingStyle);
            GUI.Label(new Rect(230, 255, 1010, 130), current.Status, cardStyle);
            GUI.Label(new Rect(230, 420, 780, 50), "选择你能安全完成的协助：", bodyStyle);

            for (var i = 0; i < current.Choices.Length; i++)
            {
                if (GUI.Button(new Rect(255, 500 + i * 112, 1030, 86), current.Choices[i], i == current.CorrectChoice ? buttonStyle : dangerButtonStyle))
                {
                    if (i == current.CorrectChoice)
                    {
                        careScore += 10;
                        ShowFeedback(current.Explanation, 2.1f);
                        rehabIndex++;
                        if (rehabIndex >= rehabCases.Length) EnterStage(Stage.RecoveryCheck);
                    }
                    else
                    {
                        WrongChoice("这超出了志愿者职责，可能干扰专业治疗。请选择记录、环境或后勤支持。", 5);
                    }
                }
            }

            DrawObjective($"护理协助：{rehabIndex + 1} / {rehabCases.Length}", "章节 05 / 06");
        }

        private void DrawRecoveryCheck()
        {
            DrawPanel(new Rect(235, 145, 1050, 760));
            GUI.Label(new Rect(290, 195, 930, 60), "是否已经适合放归？", headingStyle);

            var status = recoveryRound == 0
                ? "记录第 6 天：羽毛表面较干净，但活动力仍不稳定；专业评估尚未完成。"
                : "记录第 18 天：专业团队完成健康、行为与羽毛状态评估；放归地点和天气条件已经确认。";
            GUI.Label(new Rect(290, 295, 930, 150), status, cardStyle);

            if (GUI.Button(new Rect(320, 520, 405, 90), "现在放归", dangerButtonStyle))
            {
                if (recoveryRound == 0)
                {
                    WrongChoice("太早了。外观改善不等于能够在野外保温、觅食和生存。", 6);
                }
                else
                {
                    careScore += 14;
                    EnterStage(Stage.Release);
                }
            }
            if (GUI.Button(new Rect(785, 520, 405, 90), "继续专业评估", buttonStyle))
            {
                if (recoveryRound == 0)
                {
                    careScore += 12;
                    recoveryRound = 1;
                    ShowFeedback("正确。决定依据是一组专业指标，不是玩家的感觉。", 2.2f);
                }
                else
                {
                    ShowFeedback("评估已经完成；现在可以由专业团队执行放归。", 2f);
                }
            }

            DrawObjective("外观不是唯一指标。", "章节 05 / 06");
        }

        private void DrawRelease()
        {
            DrawPanel(new Rect(110, 125, 590, 810));
            GUI.Label(new Rect(155, 175, 500, 60), "回到海边", headingStyle);
            GUI.Label(new Rect(155, 255, 490, 210),
                "专业团队完成最终检查，并选择了合适的地点和条件。\n\n你的最后任务仍然不是触碰动物，而是保持通道安静、清晰。",
                bodyStyle);

            if (!releaseGateReady)
            {
                if (GUI.Button(new Rect(165, 555, 460, 90), "清空放归通道", buttonStyle))
                {
                    releaseGateReady = true;
                    careScore += 10;
                    ShowFeedback("通道已清空。专业人员正在开启运输箱。", 2f);
                }
            }
            else if (GUI.Button(new Rect(165, 555, 460, 90), "安静观察放归", buttonStyle))
            {
                careScore += 10;
                EnterStage(Stage.Results);
            }

            GUI.Label(new Rect(785, 250, 880, 120), releaseGateReady
                ? "箱门打开。企鹅停了一会儿，然后朝浪线走去。"
                : "海风、潮水、远处的鸟鸣。没有告别台词，也没有回头致谢。", MakeLightBodyStyle());
            DrawObjective("让野生动物回到野外。", "章节 06 / 06");
        }

        private void DrawResults()
        {
            DrawRect(new Rect(0, 70, DesignWidth, 1010), new Color(0.02f, 0.11f, 0.16f, 0.7f));
            GUI.Label(new Rect(210, 180, 1400, 90), "关怀不是占有，而是知道何时后退。", titleStyle);
            GUI.Label(new Rect(215, 320, 1100, 75), "第一章完成｜CARE 行动记录", subtitleStyle);

            DrawResultCard(new Rect(215, 450, 310, 210), "C", "CALM\n降低干扰");
            DrawResultCard(new Rect(555, 450, 310, 210), "A", "ASSESS\n先观察记录");
            DrawResultCard(new Rect(895, 450, 310, 210), "R", "REPORT\n联系专业救助");
            DrawResultCard(new Rect(1235, 450, 310, 210), "E", "EXIT\n保持距离");

            GUI.Label(new Rect(215, 720, 780, 90), $"CARE 分数：{careScore}　错误尝试：{mistakes}", subtitleStyle);
            if (GUI.Button(new Rect(215, 840, 380, 85), "再次体验", buttonStyle)) ResetChapter();
            GUI.Label(new Rect(650, 830, 950, 100), "现实中请联系当地野生动物救助机构；不要自行抓捕、清洗、喂食或放生。", MakeLightBodyStyle());
        }

        private void DrawResultCard(Rect rect, string letter, string description)
        {
            DrawRect(rect, new Color(0.94f, 0.97f, 0.94f, 0.95f));
            var darkCentered = new GUIStyle(centeredStyle) { normal = { textColor = new Color(0.05f, 0.28f, 0.34f) } };
            darkCentered.fontSize = 58;
            GUI.Label(new Rect(rect.x, rect.y + 18, rect.width, 75), letter, darkCentered);
            var darkSmall = new GUIStyle(centeredStyle) { normal = { textColor = new Color(0.12f, 0.27f, 0.3f) } };
            darkSmall.fontSize = 22;
            GUI.Label(new Rect(rect.x + 12, rect.y + 105, rect.width - 24, 85), description, darkSmall);
        }

        private void DrawObjective(string objective, string chapter)
        {
            DrawRect(new Rect(0, 1000, DesignWidth, 80), new Color(0.03f, 0.12f, 0.15f, 0.94f));
            GUI.Label(new Rect(45, 1016, 1150, 48), objective, centeredStyle);
            GUI.Label(new Rect(1570, 1016, 290, 48), chapter, tagStyle);
        }

        private void DrawPanel(Rect rect)
        {
            DrawRect(rect, new Color(0.96f, 0.97f, 0.93f, 0.96f));
            DrawRect(new Rect(rect.x, rect.y, 12, rect.height), new Color(0.08f, 0.45f, 0.52f));
        }

        private void DrawMeter(Rect rect, float value, Color color)
        {
            DrawRect(rect, new Color(0.1f, 0.16f, 0.17f, 0.3f));
            DrawRect(new Rect(rect.x + 5, rect.y + 5, (rect.width - 10) * Mathf.Clamp01(value), rect.height - 10), color);
        }

        private void DrawPenguin(Rect rect, bool upright)
        {
            var yShift = upright ? -25f : 15f;
            DrawRect(new Rect(rect.x + 42, rect.y + 45 + yShift, rect.width - 84, rect.height - 55), new Color(0.08f, 0.11f, 0.12f));
            DrawRect(new Rect(rect.x + 64, rect.y + 92 + yShift, rect.width - 128, rect.height - 120), new Color(0.9f, 0.89f, 0.78f));
            DrawRect(new Rect(rect.x + 75, rect.y + 42 + yShift, 17, 17), Color.white);
            DrawRect(new Rect(rect.x + 80, rect.y + 47 + yShift, 7, 7), Color.black);
            DrawRect(new Rect(rect.x + 92, rect.y + 65 + yShift, 48, 14), new Color(0.9f, 0.55f, 0.18f));
            if (!upright)
            {
                DrawRect(new Rect(rect.x + 45, rect.y + 160, 95, 22), new Color(0.2f, 0.16f, 0.1f, 0.8f));
            }
        }

        private void DrawToast(string message)
        {
            DrawRect(new Rect(440, 86, 1040, 90), new Color(0.04f, 0.18f, 0.22f, 0.97f));
            GUI.Label(new Rect(470, 101, 980, 60), message, centeredStyle);
        }

        private GUIStyle MakeLightBodyStyle()
        {
            var style = new GUIStyle(bodyStyle);
            style.normal.textColor = new Color(0.91f, 0.97f, 0.97f);
            return style;
        }

        private static void DrawRect(Rect rect, Color color)
        {
            var oldColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        private void WrongChoice(string message, int penalty)
        {
            mistakes++;
            careScore = Mathf.Max(0, careScore - penalty);
            ShowFeedback(message, 3.3f);
        }

        private void ShowFeedback(string message, float duration)
        {
            feedback = message;
            feedbackUntil = Time.unscaledTime + duration;
        }

        private void EnterStage(Stage next)
        {
            stage = next;
            feedback = string.Empty;
            if (stage == Stage.QuietPerimeter)
            {
                perimeterStart = Time.time;
            }
        }

        private void ResetChapter()
        {
            observedClues.Clear();
            clearedDisturbances.Clear();
            handoffOrder.Clear();
            archivePage = 0;
            selectedClue = -1;
            mistakes = 0;
            careScore = 0;
            feedback = string.Empty;
            noise = 78f;
            rehabIndex = 0;
            recoveryRound = 0;
            releaseGateReady = false;
            EnterStage(Stage.Title);
        }
    }
}
