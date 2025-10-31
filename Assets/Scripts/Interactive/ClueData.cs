using UnityEngine;

/// <summary>
/// �������������ݶ��壺�ѡ�����ʲô + ������Ķ����ı����ȷŵ���Դ�����á�
/// �� Project ���Ҽ���Create > Clues > ClueData ����ÿ�������� .asset��
/// </summary>
[CreateAssetMenu(fileName = "NewClue", menuName = "Clues/ClueData")]
public class ClueData : ScriptableObject
{
    [Header("��ʶ & չʾ")]
    public string clueId;                 // ����ΨһID�������ڽ���/״̬�жϣ�
    public string displayName;            // չʾ���ƣ������ڱ�������־��
    public Sprite icon;                   // Сͼ�꣨��ѡ��

    [Header("�� Narrative ���")]
    [Tooltip("������������Ҫ��ʾ�Ķ��ף��Ƽ���")]
    public DialogueData monologueData;    // ��������ʾ��һ�ζ��ף�ScriptableObject��

    [TextArea(2, 6)]
    [Tooltip("��������� DialogueData��Ҳ��ֱ��дһ���ı������ö��ף���")]
    public string monologueFallbackText;  // ���ã�ֱ��дһ���ı������Զ�ת��һ�жԻ����ţ�

    [Header("ʰȡ/��������ѡ������ռλ��")]
    public bool addToInventory;           // �����򽻻�����뱳����������Ԥ�� TODO��
    public string inventoryDescription;   // �����е�����
    public Sprite inventoryImage;         // �����е�ͼƬ
}
