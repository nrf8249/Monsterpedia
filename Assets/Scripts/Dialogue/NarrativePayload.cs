using UnityEngine;

/// <summary>
/// NarrativePayload���� NPC ���ϵ���Ϣ������� UI��NarrativeBox��
/// ���ڰ������Ի����ݣ�DialogueData��+ Ф�����棨Sprite��+ �Ƿ���������ʾ��
/// ��Ԥ����Ѫ�������Խ�����ֱ�Ӽ��ֶβ��� UI ����ʾ��
/// </summary>
public class NarrativePayload
{
    public DialogueData data;     // ��Ҫ��ʾ��̨������
    public Sprite portrait;       // Ф�����棨��Ϊ�գ�
    public string characterName; // ��ɫ���ƣ���ѡ�����ȼ����� data �ڵ����ƣ�
    public bool asMonologue;      // �Ƿ���������ʾ�����׳�����������ʾ��

    // ��Ԥ�����Ժ��ս��/״̬�á�
    // public int currentHP;

    public NarrativePayload(DialogueData data, Sprite portrait = null, string characterName = null, bool asMonologue = false)
    {
        this.data = data;
        this.portrait = portrait;
        this.characterName = characterName;
        this.asMonologue = asMonologue;
    }
}
