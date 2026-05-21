using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressScale = 0.94f;
    [SerializeField] private float tweenDuration = 0.15f;

    private RectTransform rt;
    private Vector3 baseScale;
    private bool isHovering;
    private bool isPressed;
    private Coroutine currentTween;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        baseScale = rt.localScale;
    }

    public void OnPointerEnter(PointerEventData _)
    {
        isHovering = true;
        TweenTo(isPressed ? pressScale : hoverScale);
    }

    public void OnPointerExit(PointerEventData _)
    {
        isHovering = false;
        if (!isPressed) TweenTo(1f);
    }

    public void OnPointerDown(PointerEventData _)
    {
        isPressed = true;
        TweenTo(pressScale);
    }

    public void OnPointerUp(PointerEventData _)
    {
        isPressed = false;
        TweenTo(isHovering ? hoverScale : 1f);
    }

    private void TweenTo(float targetMultiplier)
    {
        if (currentTween != null) StopCoroutine(currentTween);
        currentTween = StartCoroutine(TweenScale(baseScale * targetMultiplier));
    }

    private IEnumerator TweenScale(Vector3 to)
    {
        Vector3 from = rt.localScale;
        float t = 0f;
        while (t < tweenDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / tweenDuration);
            float eased = EaseOutBack(k);
            rt.localScale = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }
        rt.localScale = to;
        currentTween = null;
    }

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
