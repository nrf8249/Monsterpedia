using UnityEngine;

/// <summary>
/// NarrativePayload：把 NPC 身上的信息打包交给 UI（NarrativeBox）
/// 现在包含：对话数据（DialogueData）+ 肖像立绘（Sprite）+ 是否当作独白显示。
/// 【预留】血量等属性将来可直接加字段并在 UI 中显示。
/// </summary>
public class DialoguePayload
{
    public DialogueData data;     // 需要显示的台词数据
    public Sprite portrait;       // 肖像立绘（可为空）
    public string characterName; // 角色名称（可选，优先级高于 data 内的名称）

    // 【预留：以后接战斗/状态用】
    // public int currentHP;

    public DialoguePayload(DialogueData data, Sprite portrait = null, string characterName = null)
    {
        this.data = data;
        this.portrait = portrait;
        this.characterName = characterName;
    }
}
