using UnityEngine;

/// <summary>
/// 统一外部调用入口：根据场景里拖进来的 NarrativeBox 来显示 Intro / Dialogue / Monologue。
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private NarrativeBox narrativeBox; // 在Inspector拖 DialogueGroup 上的 NarrativeBox

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // —— Intro（NPC交互时按钮上方的提示文本）——

    public void ShowIntro(string introText, bool showButtons = true)
        => narrativeBox.ShowIntro(introText, showButtons);

    public void ShowIntro(DialogueData introData, bool showButtons = true, int useLineIndex = 0)
        => narrativeBox.ShowIntro(introData, showButtons, useLineIndex);

    // —— 正式对话 ——（逐字打印 narrativeText）

    public void StartDialogue(DialogueData data)
        => narrativeBox.StartDialogue(data);

    // —— 独白/线索提示 ——（逐字打印 narrativeText，无按钮）

    public void StartMonologue(DialogueData data)
        => narrativeBox.StartMonologue(data);

    public void StartMonologue(string[] lines)
        => narrativeBox.StartMonologue(lines);

    // —— 停止并隐藏 —— 
    public void StopDialogue()
        => narrativeBox.StopAllNarrative();
}
