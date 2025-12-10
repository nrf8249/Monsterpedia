using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private Inventory inventory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (inventory == null)
            Debug.LogError("InventoryManager：Inventory 未赋值，请在 Inspector 中拖入 Inventory 上的组件。");
    }
    
    public void OpenClueBook()
    {
        inventory.OpenClueBook();
    }
    public void InCheck()
    {
        inventory.InCheck();
    }

    public void InDetail()
    {
        inventory.InDetail();
    }

    public void InCommon()
    {
        inventory.InCommon();
    }

    public void InShow()
    {
        inventory.InShow();
    }

    public void InAccuse()
    {
        inventory.InAccuse();
    }

    public void InHidden()
    {
        inventory.InHidden();
    }

    public void GetClue(string clueKey)
    {
        inventory.GetClue(clueKey);
    }

    public bool HasClue(string clueKey)
    {
        return inventory.HasClue(clueKey);
    }

    public bool IsHasAnyClue()
    {
        return inventory.IsHasAnyClue();
    }

    public void GetNewClue(bool isUpdated)
    {
        inventory.GetNewClue(isUpdated);
    }

}
