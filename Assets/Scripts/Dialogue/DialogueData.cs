using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;   // 说话人
        [TextArea(2, 5)]         // Inspector 中显示多行文本框
        public string content;   // 台词内容
    }

    public DialogueLine[] lines;
}

