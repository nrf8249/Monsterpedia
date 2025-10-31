using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    [Header("对话数据")]
    public DialogueData talkData;

    [Header("肖像立绘（可选）")]
    public Sprite portrait;

    [Header("名字")]
    [Tooltip("NPC 名字（显示在对话框里）")]
    public string npcName;   // ← 改名，避免遮蔽 MonoBehaviour.name

    [Header("可选：靠近提示UI")]
    public GameObject interactHint;

    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (interactHint) interactHint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactHint) interactHint.SetActive(false);
        DialogueManager.Instance.StopDialogue();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started || !playerInRange) return;

        if (talkData == null)
        {
            Debug.LogWarning($"{gameObject.name}: 未设置 talkData，对话无法开始。");
            return;
        }

        var payload = new NarrativePayload(
            data: talkData,
            portrait: portrait,
            characterName: string.IsNullOrEmpty(npcName) ? null : npcName,
            asMonologue: false
        );

        DialogueManager.Instance.StartDialogue(payload);
    }
}
