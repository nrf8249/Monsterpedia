using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public enum NarrativeType
    {
        Start,
        Talk,
        ShowPrompt,
        ShowNothing,
        Clue,
        Accuse
    }
    public enum ConditionType
    {
        None,
        GetClue,            // 拿到了某个线索
        InvestigatedClue,   // 已经检查过某线索
        TalkedToNpcAtLeast, // 与某个 NPC 对话次数 >= X
        TalkTimesRange,     // 当前和这个 NPC 的对话次数在某个区间
        HasFlag             // 任意自定义 bool（例如 已经获得“Bartender's Alibi”）
    }

    [System.Serializable]
    public class NarrativeComponent
    {
        public NarrativeType narrativeType;
        public ConditionType conditionType;
        public string key;
        public int value;
        public int talkTimes;
        [Header("lines")]
        public Line[] lines;
    }

    [Serializable]
    public class Line
    {
        public bool isPlayerSpeak;
        [TextArea(2, 5)] public string content;
    }

    public Sprite speaker;
    public NarrativeComponent[] narrativeComponents;

    // get start line
    public NarrativeComponent GetStartText()
    {
        for (int i = 0; i < narrativeComponents.Length; i++)
        {
            if (narrativeComponents[i].narrativeType == NarrativeType.Start && narrativeComponents[i].lines.Length > 0)
            {
                return narrativeComponents[i];
            }
        }
        return null;
    }

    // get show prompt line
    public NarrativeComponent GetShowPrompt()
    {
        for (int i = 0; i < narrativeComponents.Length; i++)
        {
            if (narrativeComponents[i].narrativeType == NarrativeType.ShowPrompt &&  narrativeComponents[i].lines.Length > 0)
            {
                return  narrativeComponents[i];
            }
        }
        return null;
    }

    public NarrativeComponent GetShowNothing()
    {
        for (int i = 0; i < narrativeComponents.Length; i++)
        {
            if (narrativeComponents[i].narrativeType == NarrativeType.ShowNothing && narrativeComponents[i].lines.Length > 0)
            {
                return narrativeComponents[i];
            }
        }
        return null;
    }

    // get all talk lines ordered by 'order' field
    public NarrativeComponent GetTalkDialogue(int talkTimes)
    {
        NarrativeComponent[] talks = narrativeComponents
            .Where(c => c.narrativeType == NarrativeType.Talk && c.lines != null && c.lines.Length > 0)
            .ToArray();

        if (talks.Length == 0) return null;

        // 找到最大的 talkTimes（比如 2 表示“2+”）
        int maxTalkTimes = talks.Max(c => c.talkTimes);
        int clamped = Mathf.Clamp(talkTimes, 0, maxTalkTimes);

        // 先找完全匹配的
        var exact = talks.FirstOrDefault(c => c.talkTimes == clamped);
        if (exact != null) return exact;

        // 没匹配到就找 <= clamped 中最大的那个
        NarrativeComponent best = null;
        int bestTT = int.MinValue;
        foreach (var c in talks)
        {
            if (c.talkTimes <= clamped && c.talkTimes > bestTT)
            {
                bestTT = c.talkTimes;
                best = c;
            }
        }

        return best ?? talks[0];
    }

    // search clue lines by key
    public NarrativeComponent GetClueDialogue(string clueKey)
    {
        for (int i = 0; i < narrativeComponents.Length; i++)
        {
            if (narrativeComponents[i].narrativeType == NarrativeType.Clue && narrativeComponents[i].key == clueKey)
            {
                return narrativeComponents[i];
            }
        }
        return null;
    }
}
