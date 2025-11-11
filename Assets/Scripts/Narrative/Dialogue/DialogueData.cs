using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public enum Trigger
    {
        Start,      
        Talk,       
        ShowPrompt, 
        Clue        
    }

    [Serializable]
    public class Line
    {
        public Trigger trigger;
        public string key; 
        [TextArea(2, 5)] public string content;
        public int order = 0;               
    }

    [Header("lines")]
    public Line[] lines;

    // get start line
    public string GetStartText()
    {
        var l = lines?.FirstOrDefault(x => x.trigger == Trigger.Start);
        return l != null ? l.content : string.Empty;
    }

    // get show prompt line
    public string GetShowPrompt()
    {
        var l = lines?.FirstOrDefault(x => x.trigger == Trigger.ShowPrompt);
        return l != null ? l.content : "What do you want to show me?";
    }

    // get all talk lines ordered by 'order' field
    public Line[] GetTalkLinesOrdered()
    {
        if (lines == null) return Array.Empty<Line>();
        return lines.Where(x => x.trigger == Trigger.Talk)
                    .OrderBy(x => x.order)
                    .ThenBy(x =>
                    {
                        // keep original order for same order value
                        for (int i = 0; i < lines.Length; i++)
                            if (lines[i] == x) return i;
                        return int.MaxValue;
                    })
                    .ToArray();
    }

    // search clue lines by key
    public Line[] GetClueLines(string clueKey)
    {
        if (lines == null || string.IsNullOrEmpty(clueKey)) return Array.Empty<Line>();
        return lines.Where(x => x.trigger == Trigger.Clue && x.key == clueKey).ToArray();
    }
}
