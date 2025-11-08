using UnityEngine;

/// <summary>
/// 地面线索的数据定义：把“这是什么 + 交互后的独白文本”等放到资源里配置。
/// 在 Project 里右键：Create > Clues > ClueData 创建每个线索的 .asset。
/// </summary>
[CreateAssetMenu(fileName = "NewClue", menuName = "Clues/ClueData")]
public class MonologueData : ScriptableObject
{
    [System.Serializable]
    public class MonologueLine
    {
        [TextArea(2, 5)]         // Inspector 中显示多行文本框
        public string content;   // 台词内容
    }

    public MonologueLine[] lines;
}
