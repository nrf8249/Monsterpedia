using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClueGround : MonoBehaviour
{
    [Header("线索数据（ScriptableObject）")]
    public ClueData data;

    [Header("交互提示（世界内小图标或World Space Canvas）")]
    public GameObject interactHint;
    public Vector3 hintOffset = new Vector3(0, 1.0f, 0.0f);
    public bool faceCamera = true;

    [Header("可见性与排序（自动置顶）")]
    public string sortingLayerName = "UI"; // 没有就在 Tags & Layers 里新建一个 UI
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
    }

    // 若你用输入路由（方案B），这个也能接
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!(ctx.started || ctx.performed)) return;
        if (!playerInRange) return;
        TriggerMonologue();
    }

    private void TryShowHint(string reason)
    {
        if (interactHint == null) { if (debugLogs) Debug.LogWarning($"{name} | {reason}: interactHint 为空。", this); return; }

        // 先激活
        interactHint.SetActive(true);

        // 强制置顶（SpriteRenderer 或 World-Space Canvas）
        var sr = interactHint.GetComponentInChildren<SpriteRenderer>(true);
        if (sr)
        {
            // 如果项目里还没有这个 Sorting Layer，请到 Project Settings > Tags and Layers 里添加
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
        }

        var canvas = interactHint.GetComponentInChildren<Canvas>(true);
        if (canvas && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvas.sortingLayerName = sortingLayerName;
            canvas.sortingOrder = sortingOrder;
            // 确保可见
            var cg = interactHint.GetComponentInChildren<CanvasGroup>(true);
            if (cg) cg.alpha = 1f;
        }

        // 立即摆到头顶位置
        interactHint.transform.position = transform.position + hintOffset;
        if (faceCamera && cam) interactHint.transform.forward = cam.transform.forward;

        if (debugLogs)
        {
            string active = $"activeSelf={interactHint.activeSelf}, activeInHierarchy={interactHint.activeInHierarchy}";
            string pos = $"worldPos={interactHint.transform.position}  offsetTarget={transform.position + hintOffset}";
            string srInfo = sr ? $"SR[sprite={(sr.sprite ? sr.sprite.name : "null")}, layer={sr.sortingLayerName}, order={sr.sortingOrder}, a={sr.color.a:F2}]" : "SR[null]";
            string cvInfo = canvas ? $"Canvas[mode={canvas.renderMode}, layer={canvas.sortingLayerName}, order={canvas.sortingOrder}]" : "Canvas[null]";
            Debug.Log($"{name} | 显示提示（{reason}） -> {active}; {pos}\n    {srInfo}; {cvInfo}", this);
        }
    }

    private void TriggerMonologue()
    {
        if (DialogueManager.Instance == null) { Debug.LogError($"{name} | DialogueManager.Instance 为空。", this); return; }
        if (data == null) { Debug.LogWarning($"{name} | ClueData 未绑定。", this); return; }

        // 优先 SO；否则用 fallback 文本包成一行
        if (data.monologueData && data.monologueData.lines != null && data.monologueData.lines.Length > 0)
        {
            DialogueManager.Instance.StartMonologue(data.monologueData);
            if (debugLogs) Debug.Log($"{name} | Monologue by DialogueData（行数={data.monologueData.lines.Length}）", this);
        }
        else if (!string.IsNullOrWhiteSpace(data.monologueFallbackText))
        {
            var oneLine = new DialogueData
            {
                lines = new DialogueData.DialogueLine[]
                {
                    new DialogueData.DialogueLine{ speaker="", content=data.monologueFallbackText }
                }
            };
            DialogueManager.Instance.StartMonologue(oneLine);
            if (debugLogs) Debug.Log($"{name} | Monologue by FallbackText", this);
        }
        else
        {
            Debug.LogWarning($"{name} | ClueData 里既无 monologueData 也无 fallback 文本。", this);
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
