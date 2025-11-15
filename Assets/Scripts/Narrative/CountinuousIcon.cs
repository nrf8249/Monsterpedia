using UnityEngine;

public class CountinuousIcon : MonoBehaviour
{
    [Header("跳动距离")]
    public float moveDistance = 8f;

    [Header("上升时间（快）")]
    public float upTime = 0.15f;

    [Header("下降时间（慢）")]
    public float downTime = 0.35f;

    [Header("透明度范围")]
    public float minAlpha = 0.3f;   // 最淡
    public float maxAlpha = 1.0f;   // 最亮

    private Vector3 basePos;   // 固定原始位置
    private Vector3 startPos;  // 本次循环的起点
    private float timer = 0f;
    private bool goingUp = true;

    private CanvasGroup cg;
    private bool initialized = false;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (!cg)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        // 只在第一次记录原始位置
        if (!initialized)
        {
            basePos = transform.localPosition;
            initialized = true;
        }

        startPos = basePos;            // 每次启用都从原始位置开始
        transform.localPosition = basePos;

        timer = 0f;
        goingUp = true;
        cg.alpha = maxAlpha;
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (goingUp)
        {
            float t = timer / upTime;
            if (t >= 1f)
            {
                t = 1f;
                timer = 0f;
                goingUp = false;
            }

            float eased = Mathf.SmoothStep(0f, 1f, t);
            transform.localPosition = basePos + Vector3.up * moveDistance * eased;
            cg.alpha = Mathf.Lerp(maxAlpha, minAlpha, eased);
        }
        else
        {
            float t = timer / downTime;
            if (t >= 1f)
            {
                t = 1f;
                timer = 0f;
                goingUp = true;
            }

            float eased = 1f - (1f - t) * (1f - t);
            transform.localPosition = basePos + Vector3.up * moveDistance * (1f - eased);
            cg.alpha = Mathf.Lerp(minAlpha, maxAlpha, eased);
        }
    }
}
