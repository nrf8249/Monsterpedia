using System;
using System.Diagnostics;
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
        ShowClue,
        ShowNothing,
        Clue,
        Accuse
    }
    public enum ConditionType
    {
        None,
        AlreadyGetClue,
    }

    [System.Serializable]
    public class NarrativeComponent
    {
        public NarrativeType narrativeType;
        public ConditionType conditionType;
        public string key;
        public int value;
        public int talkTimes;
        public bool givesClue;
        public string[] clueKey;
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
        for (int i = 0; i < narrativeComponents.Length; i++)
        {
            if (narrativeComponents[i].narrativeType == NarrativeType.Talk && narrativeComponents[i].talkTimes == talkTimes)
            {
                if (SatisfyCondition(narrativeComponents[i]))
                {
                    return narrativeComponents[i];
                }
            }
            else if (narrativeComponents[i].narrativeType == NarrativeType.Talk && narrativeComponents[i].talkTimes == -1)
            {
                return narrativeComponents[i];
            }
        } 
        return null;
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

    public bool SatisfyCondition(NarrativeComponent component)
    {
        switch (component.conditionType)
        {
            case ConditionType.None:
                return true;
            case ConditionType.AlreadyGetClue:
                return Inventory.instance.HasClue(component.key);
            default:
                return false;
        }
    }
}
