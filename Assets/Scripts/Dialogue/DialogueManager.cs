using UnityEngine;

/// <summary>
/// DialogueManager：统一的对话系统入口，内部转发给 NarrativeBox。
/// 所有 NPC/物件 都通过它来显示 Intro / Dialogue / Monologue。
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private NarrativeBox narrativeBox; // 在 Inspector 里把 DialogueGroup 上的 NarrativeBox 拖进来

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (narrativeBox == null)
            Debug.LogError("DialogueManager：NarrativeBox 未赋值，请在 Inspector 中拖入 DialogueGroup 上的组件。");
    }

    // —— Intro ——
    public void ShowIntro(string introText, bool showButtons = true)
        => narrativeBox.ShowIntro(introText, showButtons);

    public void ShowIntro(DialogueData introData, bool showButtons = true, int useLineIndex = 0)
        => narrativeBox.ShowIntro(introData, showButtons, useLineIndex);

    // —— Dialogue ——
    public void StartDialogue(DialogueData data)
        => narrativeBox.StartDialogue(data);

    public void StartDialogue(NarrativePayload payload)
        => narrativeBox.StartDialogue(payload);

    // —— Monologue ——
    public void StartMonologue(DialogueData data)
        => narrativeBox.StartMonologue(data);

    public void StartMonologue(string[] lines)
        => narrativeBox.StartMonologue(lines);

    // —— Stop ——
    public void StopDialogue()
        => narrativeBox.StopAllNarrative();
}
