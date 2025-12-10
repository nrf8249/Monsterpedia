using UnityEngine;

/// <summary>
/// store monologue data
/// </summary>
[CreateAssetMenu(fileName = "NewClue", menuName = "Clues/ClueData")]
public class MonologueData : ScriptableObject
{
    [System.Serializable]
    public class MonologueLine
    {
        [TextArea(2, 5)]         
        public string content;   
    }
    public string clueName;
    public MonologueLine[] lines;
}
