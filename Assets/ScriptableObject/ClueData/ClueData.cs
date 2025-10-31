using UnityEngine;

/// <summary>
/// 地面线索的数据定义：把“这是什么 + 交互后的独白文本”等放到资源里配置。
/// 在 Project 里右键：Create > Clues > ClueData 创建每个线索的 .asset。
/// </summary>
[CreateAssetMenu(fileName = "NewClue", menuName = "Clues/ClueData")]
public class ClueData : ScriptableObject
{
    [Header("标识 & 展示")]
    public string clueId;                 // 线索唯一ID（可用于解锁/状态判断）
    public string displayName;            // 展示名称（可用于背包或日志）
    public Sprite icon;                   // 小图标（可选）

    [Header("与 Narrative 相关")]
    [Tooltip("和线索交互后要显示的独白（推荐）")]
    public DialogueData monologueData;    // 交互后显示的一段独白（ScriptableObject）

    [TextArea(2, 6)]
    [Tooltip("如果不想做 DialogueData，也可直接写一段文本（备用兜底）。")]
    public string monologueFallbackText;  // 备用：直接写一段文本（会自动转成一行对话播放）

    [Header("拾取/背包（可选，先留占位）")]
    public bool addToInventory;           // 勾上则交互后加入背包（代码里预留 TODO）
    public string inventoryDescription;   // 背包中的描述
    public Sprite inventoryImage;         // 背包中的图片
}
