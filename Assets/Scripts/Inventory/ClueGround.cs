using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClueGround : MonoBehaviour
{
    [Header("线索数据（ScriptableObject）")]
    public MonologueData data;

    [Header("交互提示（世界内小图标或World Space Canvas）")]
    public GameObject interactHint;
    public Vector3 hintOffset = new Vector3(0, 1.0f, 0.0f);
    public bool faceCamera = true;

    [Header("可见性与排序（自动置顶）")]
    public string sortingLayerName = "UI";
    public int sortingOrder = 100;

    [Header("调试")]
    public bool debugLogs = true;

    private bool playerInRange = false;
    private Camera cam;
    private Transform hintTf;

    private void Awake()
    {
        cam = Camera.main;

        if (interactHint != null)
        {
            hintTf = interactHint.transform;
            interactHint.SetActive(false);
        }
        else if (debugLogs)
        {
            Debug.LogWarning($"{name} | interactHint 未赋值（进入范围也不会显示图标）。", this);
        }
    }

    private void Update()
    {
        // 仍保留键盘E交互（方便调试）
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TriggerMonologue();
        }
    }

    private void LateUpdate()
    {
        if (interactHint && interactHint.activeSelf)
        {
            hintTf.position = transform.position + hintOffset;
            if (faceCamera && cam) hintTf.forward = cam.transform.forward;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        TryShowHint("OnTriggerEnter2D");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactHint) interactHint.SetActive(false);
        if (debugLogs) Debug.Log($"{name} | 玩家离开线索范围，隐藏提示。", this);

        // ✅ 新增：统一通过 DialogueManager 关闭 UI
        if (NarrativeBoxManager.Instance != null)
        {
            NarrativeBoxManager.Instance.StopDialogue();
            if (debugLogs) Debug.Log($"{name} | 玩家离开线索范围，关闭 Monologue UI。", this);
        }
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!(ctx.started || ctx.performed)) return;
        if (!playerInRange) return;
        TriggerMonologue();
    }

    private void TryShowHint(string reason)
    {
        if (interactHint == null)
        {
            if (debugLogs) Debug.LogWarning($"{name} | {reason}: interactHint 为空。", this);
            return;
        }

        interactHint.SetActive(true);

        // 置顶层级
        var sr = interactHint.GetComponentInChildren<SpriteRenderer>(true);
        if (sr)
        {
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
        }

        var canvas = interactHint.GetComponentInChildren<Canvas>(true);
        if (canvas && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvas.sortingLayerName = sortingLayerName;
            canvas.sortingOrder = sortingOrder;
            var cg = interactHint.GetComponentInChildren<CanvasGroup>(true);
            if (cg) cg.alpha = 1f;
        }

        interactHint.transform.position = transform.position + hintOffset;
        if (faceCamera && cam) interactHint.transform.forward = cam.transform.forward;
    }

    private void TriggerMonologue()
    {
        if (NarrativeBoxManager.Instance == null)
        {
            Debug.LogError($"{name} | DialogueManager.Instance 为空。", this);
            return;
        }

        if (data == null)
        {
            Debug.LogWarning($"{name} | ClueData 未绑定。", this);
            return;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0f, 0.8f);
        var p = transform.position + hintOffset;
        Gizmos.DrawWireSphere(p, 0.1f);
        Gizmos.DrawLine(transform.position, p);
    }
}
