using UnityEngine;

/// <summary>
/// send to NarrativeBox to start a Monologue
/// </summary>
public class MonologuePayload
{
    public MonologueData data;     // monologue data asset
    public Sprite portrait;        // character portrait
    public string clueName;        // clue name

    // public int currentHP;

    public MonologuePayload(MonologueData data, Sprite portrait = null, string clueName = null)
    {
        this.data = data;
        this.portrait = portrait;
        this.clueName = clueName;
    }
}
