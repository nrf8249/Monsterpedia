using UnityEngine;

/// <summary>
/// ͳһ�ⲿ������ڣ����ݳ������Ͻ����� NarrativeBox ����ʾ Intro / Dialogue / Monologue��
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private NarrativeBox narrativeBox; // ��Inspector�� DialogueGroup �ϵ� NarrativeBox

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ���� Intro��NPC����ʱ��ť�Ϸ�����ʾ�ı�������

    public void ShowIntro(string introText, bool showButtons = true)
        => narrativeBox.ShowIntro(introText, showButtons);

    public void ShowIntro(DialogueData introData, bool showButtons = true, int useLineIndex = 0)
        => narrativeBox.ShowIntro(introData, showButtons, useLineIndex);

    // ���� ��ʽ�Ի� ���������ִ�ӡ narrativeText��

    public void StartDialogue(DialogueData data)
        => narrativeBox.StartDialogue(data);

    // ���� ����/������ʾ ���������ִ�ӡ narrativeText���ް�ť��

    public void StartMonologue(DialogueData data)
        => narrativeBox.StartMonologue(data);

    public void StartMonologue(string[] lines)
        => narrativeBox.StartMonologue(lines);

    // ���� ֹͣ������ ���� 
    public void StopDialogue()
        => narrativeBox.StopAllNarrative();
}
