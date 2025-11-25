using UnityEngine;

/// <summary>
/// send to NarrativeBox to start a Dialogue
/// </summary>
public class DialoguePayload
{
    public DialogueData data;     // dialogue data asset
    public int talkTimes;         // number of times talked to this character
    public Sprite portrait;       // character portrait
    public string characterName;  // character name

    // public int currentHP;

    public DialoguePayload(DialogueData data, Sprite portrait = null, string characterName = null)
    {
        this.data = data;
        this.portrait = portrait;
        this.characterName = characterName;
    }
}
