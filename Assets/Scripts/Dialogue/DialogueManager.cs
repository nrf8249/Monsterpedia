using UnityEngine;

/// <summary>
/// DialogueManager��ͳһ�ĶԻ�ϵͳ��ڣ��ڲ�ת���� NarrativeBox��
/// ���� NPC/��� ��ͨ��������ʾ Intro / Dialogue / Monologue��
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private NarrativeBox narrativeBox; // �� Inspector ��� DialogueGroup �ϵ� NarrativeBox �Ͻ���

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (narrativeBox == null)
            Debug.LogError("DialogueManager��NarrativeBox δ��ֵ������ Inspector ������ DialogueGroup �ϵ������");
    }

    // ���� Intro ����
    public void ShowIntro(string introText, bool showButtons = true)
        => narrativeBox.ShowIntro(introText, showButtons);

    public void ShowIntro(DialogueData introData, bool showButtons = true, int useLineIndex = 0)
        => narrativeBox.ShowIntro(introData, showButtons, useLineIndex);

    // ���� Dialogue ����
    public void StartDialogue(DialogueData data)
        => narrativeBox.StartDialogue(data);

    public void StartDialogue(NarrativePayload payload)
        => narrativeBox.StartDialogue(payload);

    // ���� Monologue ����
    public void StartMonologue(DialogueData data)
        => narrativeBox.StartMonologue(data);

    public void StartMonologue(string[] lines)
        => narrativeBox.StartMonologue(lines);

    // ���� Stop ����
    public void StopDialogue()
        => narrativeBox.StopAllNarrative();
}
