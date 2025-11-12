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

    private Vector3 startPos;
    private float timer = 0f;
    private bool goingUp = true;

    private CanvasGroup cg;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (!cg)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        startPos = transform.localPosition;
        timer = 0f;
        goingUp = true;

        transform.localPosition = startPos;
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

            // --- 上升位置：快速 SmoothStep ---
            float eased = Mathf.SmoothStep(0f, 1f, t);
            transform.localPosition = startPos + Vector3.up * moveDistance * eased;

            // --- 上升透明度：慢慢变淡 ---
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

            // --- 下落位置：缓慢 easeOut ---
            float eased = 1f - (1f - t) * (1f - t);
            transform.localPosition = startPos + Vector3.up * moveDistance * (1f - eased);

            // --- 下落透明度：慢慢变亮 ---
            cg.alpha = Mathf.Lerp(minAlpha, maxAlpha, eased);
        }
    }
}
