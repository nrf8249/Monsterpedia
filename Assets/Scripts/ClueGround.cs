using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClueGround : MonoBehaviour
{
    [Header("�������ݣ�ScriptableObject��")]
    public ClueData data;

    [Header("������ʾ��������Сͼ���World Space Canvas��")]
    public GameObject interactHint;
    public Vector3 hintOffset = new Vector3(0, 1.0f, 0.0f);
    public bool faceCamera = true;

    [Header("�ɼ����������Զ��ö���")]
    public string sortingLayerName = "UI"; // û�о��� Tags & Layers ���½�һ�� UI
    public int sortingOrder = 100;

    [Header("����")]
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
            Debug.LogWarning($"{name} | interactHint δ��ֵ�����뷶ΧҲ������ʾͼ�꣩��", this);
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
        if (debugLogs) Debug.Log($"{name} | ����뿪������Χ��������ʾ��", this);
    }

    // ����������·�ɣ�����B�������Ҳ�ܽ�
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!(ctx.started || ctx.performed)) return;
        if (!playerInRange) return;
        TriggerMonologue();
    }

    private void TryShowHint(string reason)
    {
        if (interactHint == null) { if (debugLogs) Debug.LogWarning($"{name} | {reason}: interactHint Ϊ�ա�", this); return; }

        // �ȼ���
        interactHint.SetActive(true);

        // ǿ���ö���SpriteRenderer �� World-Space Canvas��
        var sr = interactHint.GetComponentInChildren<SpriteRenderer>(true);
        if (sr)
        {
            // �����Ŀ�ﻹû����� Sorting Layer���뵽 Project Settings > Tags and Layers �����
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
        }

        var canvas = interactHint.GetComponentInChildren<Canvas>(true);
        if (canvas && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvas.sortingLayerName = sortingLayerName;
            canvas.sortingOrder = sortingOrder;
            // ȷ���ɼ�
            var cg = interactHint.GetComponentInChildren<CanvasGroup>(true);
            if (cg) cg.alpha = 1f;
        }

        // �����ڵ�ͷ��λ��
        interactHint.transform.position = transform.position + hintOffset;
        if (faceCamera && cam) interactHint.transform.forward = cam.transform.forward;

        if (debugLogs)
        {
            string active = $"activeSelf={interactHint.activeSelf}, activeInHierarchy={interactHint.activeInHierarchy}";
            string pos = $"worldPos={interactHint.transform.position}  offsetTarget={transform.position + hintOffset}";
            string srInfo = sr ? $"SR[sprite={(sr.sprite ? sr.sprite.name : "null")}, layer={sr.sortingLayerName}, order={sr.sortingOrder}, a={sr.color.a:F2}]" : "SR[null]";
            string cvInfo = canvas ? $"Canvas[mode={canvas.renderMode}, layer={canvas.sortingLayerName}, order={canvas.sortingOrder}]" : "Canvas[null]";
            Debug.Log($"{name} | ��ʾ��ʾ��{reason}�� -> {active}; {pos}\n    {srInfo}; {cvInfo}", this);
        }
    }

    private void TriggerMonologue()
    {
        if (DialogueManager.Instance == null) { Debug.LogError($"{name} | DialogueManager.Instance Ϊ�ա�", this); return; }
        if (data == null) { Debug.LogWarning($"{name} | ClueData δ�󶨡�", this); return; }

        // ���� SO�������� fallback �ı�����һ��
        if (data.monologueData && data.monologueData.lines != null && data.monologueData.lines.Length > 0)
        {
            DialogueManager.Instance.StartMonologue(data.monologueData);
            if (debugLogs) Debug.Log($"{name} | Monologue by DialogueData������={data.monologueData.lines.Length}��", this);
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
            Debug.LogWarning($"{name} | ClueData ����� monologueData Ҳ�� fallback �ı���", this);
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
