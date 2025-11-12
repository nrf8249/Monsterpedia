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

    [System.Serializable]
    public class NarrativeComponent
    {
        public NarrativeType narrativeType;
        public string key;
        public string value;
        [Header("lines")]
        public Line[] lines;
    }

    [Serializable]
    public class Line
    {
        [TextArea(2, 5)] public string content;        
    }

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
    public NarrativeComponent GetTalkDialogue()
    {
        for (int i = 0; i < narrativeComponents.Length; i++)
        {
            if (narrativeComponents[i].narrativeType == NarrativeType.Talk)
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
}
