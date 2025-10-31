using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NarrativeBox：统一控制整套对话UI的脚本。
/// 本版只关注两块文本：
/// 1) narrativeStartText：与NPC交互后的“开场/提示”文本（显示在按钮上方）
/// 2) narrativeText：真正开始对话后（或与线索交互的独白）显示的正文
///
/// 说明：
/// - 不再需要“说话人姓名面板”；因此不包含任何“姓名/头像”引用。
/// - 暂时忽略 Show / Accuse 两个按钮，只保留可选的按钮组父物体（用于放 Talk 按钮或你已有的按钮容器）。
/// - 提供三个主要入口：
///     ShowIntro(...)     —— 进入“开场提示”阶段，仅显示 narrativeStartText（和可选按钮组）
///     StartDialogue(...) —— 进入“正式对话”阶段，逐字打印 narrativeText
///     StartMonologue(...)—— 进入“独白/线索提示”阶段，逐字打印 narrativeText（不开按钮组）
/// - 用 OnAdvance(...) 绑定输入，实现“下一句/快进”
/// </summary>
public class NarrativeBox : MonoBehaviour
{
    [Header("对话框根节点（整个面板的父物体）")]
    [SerializeField] private GameObject dialogueGroup;     // 整个对话框UI的根（比如“DialogueGroup”）
    [SerializeField] private GameObject portrait;

    [Header("文本引用")]
    [SerializeField] private TextMeshProUGUI narrativeStartText; // 开始时（按钮上方）的提示文本
    [SerializeField] private TextMeshProUGUI narrativeText;      // 正式对话/独白正文

    [Header("按钮容器（可选）")]
    [SerializeField] private GameObject buttonGroup;       // 仅用于显示/隐藏整组按钮（本版忽略具体 Show/Accuse）

    [Header("打字机参数")]
    [SerializeField] private float textSpeed = 0.02f;      // 正文逐字速度

    // —— 数据缓存 —— //
    private DialogueData curData;          // 当前播放的 DialogueData（对话或独白）
    private string[] legacyLines;          // 兼容旧的 string[] 入口（可当作独白）
    private bool usingSO = false;          // 是否使用 DialogueData
    private int index = 0;                 // 正在播放的行号
    private Coroutine typingRoutine;       // 打字协程

    // —— 简单状态机 —— //
    private enum Mode { Hidden, Intro, Narrative }
    private Mode mode = Mode.Hidden;

    private void Reset()
    {
        // 把脚本拖到 DialogueGroup 上时，自动猜引用
        if (dialogueGroup == null) dialogueGroup = gameObject;
    }

    private void Awake()
    {
        HideAll();
    }

    // ========================== 公共 API ===========================

    /// <summary>
    /// 显示“开场/提示”文本（常用于与NPC刚对话时，按钮上方的一句提示）。
    /// - 仅显示 narrativeStartText（可选显示按钮组）
    /// - 不进入逐字剧情，不响应 OnAdvance。
    /// </summary>
    /// <param name="introText">要显示的开场字符串</param>
    /// <param name="showButtons">是否显示按钮容器（本版通常只放Talk按钮）</param>
    public void ShowIntro(string introText, bool showButtons = true)
    {
        StopTypingIfAny();

        mode = Mode.Intro;
        EnsureGroupVisible(true);

        // 开场区可见，正文区清空并隐藏
        SetStartText(introText);
        SetNarrativeText("");
        SetNarrativeVisible(false);

        SetButtonsVisible(showButtons);
    }

    /// <summary>
    /// 显示“开场/提示”文本（用 DialogueData，默认取第1行内容）。
    /// 便于你把提示内容也做成 SO。
    /// </summary>
    public void ShowIntro(DialogueData introData, bool showButtons = true, int useLineIndex = 0)
    {
        if (introData == null || introData.lines == null || introData.lines.Length == 0)
        {
            ShowIntro("", showButtons);
            return;
        }
        var line = Mathf.Clamp(useLineIndex, 0, introData.lines.Length - 1);
        ShowIntro(introData.lines[line].content, showButtons);
    }

    /// <summary>
    /// 开始“正式对话”（逐字打印 narrativeText）。
    /// - 隐藏开场区域，显示正文区域
    /// - 支持按键快进/下一句（OnAdvance）
    /// </summary>
    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.lines == null || data.lines.Length == 0)
        {
            // 无内容则直接隐藏
            StopAllNarrative();
            return;
        }

        StopTypingIfAny();
        usingSO = true;
        curData = data;
        index = 0;

        mode = Mode.Narrative;
        EnsureGroupVisible(true);

        // 隐藏开场，显示正文
        SetStartVisible(false);
        SetNarrativeVisible(true);
        SetButtonsVisible(false); // 你要也可以改成 true

        // 开始逐字
        StartTyping(curData.lines[index].content);
    }

    /// <summary>
    /// 开始“独白/线索提示”（逐字打印 narrativeText，无按钮）。
    /// - 常用于与线索交互后的自言自语
    /// </summary>
    public void StartMonologue(DialogueData data)
    {
        StartDialogue(data);
        SetButtonsVisible(false); // 独白一般不显示按钮
    }

    /// <summary>
    /// 兼容旧版：用 string[] 启动（视为“独白”）
    /// </summary>
    public void StartMonologue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            StopAllNarrative();
            return;
        }

        StopTypingIfAny();
        usingSO = false;
        legacyLines = lines;
        index = 0;

        mode = Mode.Narrative;
        EnsureGroupVisible(true);

        SetStartVisible(false);
        SetNarrativeVisible(true);
        SetButtonsVisible(false);

        StartTyping(legacyLines[index]);
    }

    /// <summary>
    /// 停止并隐藏全部对话UI。
    /// </summary>
    public void StopAllNarrative()
    {
        StopTypingIfAny();
        HideAll();
    }

    /// <summary>
    /// 输入回调：推进对话（快进/下一句）。仅在“正式对话/独白”模式下有效。
    /// </summary>
    public void OnAdvance(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (mode != Mode.Narrative) return;              // Intro 阶段不响应
        if (!dialogueGroup || !dialogueGroup.activeSelf) return;

        if (usingSO)
        {
            var full = curData.lines[index].content;
            if (narrativeText.text == full)
            {
                // 下一句
                if (index < curData.lines.Length - 1)
                {
                    index++;
                    StartTyping(curData.lines[index].content);
                }
                else
                {
                    StopAllNarrative(); // 结束
                }
            }
            else
            {
                // 快进到整句
                SkipTypingTo(full);
            }
        }
        else
        {
            var full = legacyLines[index];
            if (narrativeText.text == full)
            {
                if (index < legacyLines.Length - 1)
                {
                    index++;
                    StartTyping(legacyLines[index]);
                }
                else
                {
                    StopAllNarrative();
                }
            }
            else
            {
                SkipTypingTo(full);
            }
        }
    }

    // ========================== 内部方法 ===========================

    private void EnsureGroupVisible(bool show)
    {
        if (dialogueGroup) dialogueGroup.SetActive(show);
    }

    private void HideAll()
    {
        mode = Mode.Hidden;
        EnsureGroupVisible(false);
        SetStartText("");
        SetNarrativeText("");
        SetButtonsVisible(false);
    }

    private void SetStartText(string t)
    {
        if (narrativeStartText) narrativeStartText.text = t ?? "";
        SetStartVisible(!string.IsNullOrEmpty(t));
    }

    private void SetNarrativeText(string t)
    {
        if (narrativeText) narrativeText.text = t ?? "";
    }

    private void SetStartVisible(bool show)
    {
        if (narrativeStartText) narrativeStartText.gameObject.SetActive(show);
    }

    private void SetNarrativeVisible(bool show)
    {
        if (narrativeText) narrativeText.gameObject.SetActive(show);
    }

    private void SetButtonsVisible(bool show)
    {
        if (buttonGroup) buttonGroup.SetActive(show);
    }

    private void StartTyping(string content)
    {
        StopTypingIfAny();
        typingRoutine = StartCoroutine(TypeRoutine(content));
    }

    private void StopTypingIfAny()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }
    }

    private IEnumerator TypeRoutine(string content)
    {
        SetNarrativeText("");
        foreach (char c in content)
        {
            narrativeText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        typingRoutine = null;
    }

    private void SkipTypingTo(string full)
    {
        StopTypingIfAny();
        SetNarrativeText(full);
    }
}
