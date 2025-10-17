using UnityEngine;

[CreateAssetMenu(fileName = "InventoryEvidence", menuName = "Scriptable Objects/InventoryEvidence")]
public class InventoryEvidence : ScriptableObject
{
    public string evidenceName;
    public string description;
    public Sprite image;
}