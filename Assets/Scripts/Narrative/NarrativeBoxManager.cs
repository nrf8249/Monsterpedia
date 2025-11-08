using UnityEngine;

/// <summary>
/// DialogueManager：统一的对话系统入口，内部转发给 NarrativeBox。
/// 所有 NPC/物件 都通过它来显示 Intro / Dialogue / Monologue。
/// </summary>
public class NarrativeBoxManager : MonoBehaviour
{
    public static NarrativeBoxManager Instance;

    [SerializeField] private NarrativeBox narrativeBox; // 在 Inspector 里把 DialogueGroup 上的 NarrativeBox 拖进来

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (narrativeBox == null)
            Debug.LogError("NarrativeBoxManager：NarrativeBox 未赋值，请在 Inspector 中拖入 DialogueGroup 上的组件。");
    }


    // —— Dialogue ——
    public void StartDialogue(DialoguePayload payload)
    {
        narrativeBox.StartDialogue(payload);
    }

    // —— Monologue ——
    public void StartMonologue(MonologuePayload payload)
    {
        narrativeBox.StartMonologue(payload);
    }

    // —— Stop ——
    public void StopDialogue()
    {
        narrativeBox.StopAllNarrative();
    }
}
