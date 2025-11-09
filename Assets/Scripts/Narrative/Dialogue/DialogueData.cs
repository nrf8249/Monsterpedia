using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public enum Trigger
    {
        Start,      // 进入生命周期显示的开场文字（startText），通常 1 条
        Talk,       // 点 Talk 播放的主线对白，可多条，按 order 或数组顺序
        ShowPrompt, // 点 Show 后显示的一句提示（如“你要给我看什么？”），通常 1 条
        Clue        // 由 ShowClue(clueKey) 触发，key 匹配
    }

    [Serializable]
    public class Line
    {
        public Trigger trigger;
        public string key; 
        [TextArea(2, 5)] public string content;
        public int order = 0;               // 当 trigger=Talk 时用于排序；同序按数组顺序
    }

    [Header("台词（带触发条件）")]
    public Line[] lines;

    // —— 便捷查询接口（NarrativeBox 会用到）——
    public string GetStartText()
    {
        var l = lines?.FirstOrDefault(x => x.trigger == Trigger.Start);
        return l != null ? l.content : string.Empty;
    }

    public string GetShowPrompt()
    {
        var l = lines?.FirstOrDefault(x => x.trigger == Trigger.ShowPrompt);
        return l != null ? l.content : "你要给我看什么？";
    }

    public Line[] GetTalkLinesOrdered()
    {
        if (lines == null) return Array.Empty<Line>();
        return lines.Where(x => x.trigger == Trigger.Talk)
                    .OrderBy(x => x.order)
                    .ThenBy(x =>
                    {
                        // 数组稳定性：同 order 时按原始索引
                        for (int i = 0; i < lines.Length; i++)
                            if (lines[i] == x) return i;
                        return int.MaxValue;
                    })
                    .ToArray();
    }

    public Line[] GetClueLines(string clueKey)
    {
        if (lines == null || string.IsNullOrEmpty(clueKey)) return Array.Empty<Line>();
        return lines.Where(x => x.trigger == Trigger.Clue && x.key == clueKey).ToArray();
    }
}
