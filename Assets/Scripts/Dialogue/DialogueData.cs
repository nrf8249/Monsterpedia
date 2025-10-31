using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;   // ˵����
        [TextArea(2, 5)]         // Inspector ����ʾ�����ı���
        public string content;   // ̨������
    }

    public DialogueLine[] lines;
}

