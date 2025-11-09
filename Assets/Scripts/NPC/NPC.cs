using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("基础信息")]
    public DialogueData talkData;
    public Sprite portrait;
    [Tooltip("NPC 名字（显示在对话框上）")]
    public string npcName;

    [Header("交互提示（挂在 NPC 身上）")]
    public GameObject interactHint;                 // 图标/气泡
    public Vector3 hintOffset = new Vector3(0, 1.2f, 0);
    public bool faceCamera = true;

    [Header("调试")]
    public bool debugLogs = false;                  // 需要时打开
    public Color gizmoColor = new Color(1f, 0.85f, 0f, 0.8f);

    private bool playerInRange = false;
    private Camera mainCam;
    private Transform hintTransform;

    private void Awake()
    {
        mainCam = Camera.main;

        if (interactHint != null)
        {
            hintTransform = interactHint.transform;
            interactHint.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (interactHint != null && interactHint.activeSelf)
        {
            hintTransform.position = transform.position + hintOffset;
            if (faceCamera && mainCam) hintTransform.forward = mainCam.transform.forward;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (interactHint) interactHint.SetActive(true);
        if (debugLogs) Debug.Log($"{name} | 玩家进入交互范围");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactHint) interactHint.SetActive(false);
        if (NarrativeBoxManager.Instance != null) NarrativeBoxManager.Instance.StopDialogue();
        if (debugLogs) Debug.Log($"{name} | 玩家离开交互范围，隐藏提示 + 关闭对话框");
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;
        StartDialogue();
    }

    private void StartDialogue()
    {
        // debugging checks
        if (talkData == null)
        {
            if (debugLogs) Debug.LogWarning($"{name} | talkData 为空，无法开始对话");
            return;
        }
        if (NarrativeBoxManager.Instance == null)
        {
            Debug.LogError($"{name} | DialogueManager.Instance 为空（场景里没挂或被禁用）");
            return;
        }

        // start dialogue
        var payload = new DialoguePayload(
            data: talkData,
            portrait: portrait,
            characterName: string.IsNullOrEmpty(npcName) ? null : npcName
        );
        if (NarrativeBoxManager.Instance.IsNarrating)
            return;
        NarrativeBoxManager.Instance.StartDialogue(payload);
        if (debugLogs) Debug.Log($"{name} | 已开始对话（行数={talkData.lines?.Length ?? 0}）");
    }

    // Gizmos: notice the interact hint position in Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Vector3 p = transform.position + hintOffset;
        Gizmos.DrawWireSphere(p, 0.1f);
        Gizmos.DrawLine(transform.position, p);
    }
}

