using UnityEngine;
using DG.Tweening;

public class UISlideFromTop : MonoBehaviour
{
    public RectTransform rect;

    [Header("Positions")]
    public float startY = 1200f; // off-screen above
    public float targetY = 0f;   // final position

    [Header("Timing")]
    public float fallDuration = 0.5f;
    public float bounceDuration = 0.15f;
    public float hideDuration = 0.4f;

    [Header("Bounce")]
    public float bounceHeight = 40f;

    [Header("Ease")]
    public Ease fallEase = Ease.OutCubic;
    public Ease bounceUpEase = Ease.OutQuad;
    public Ease bounceDownEase = Ease.InQuad;
    public Ease hideEase = Ease.InCubic;

    Tween currentTween;

    void Awake()
    {
        if (!rect) rect = GetComponent<RectTransform>();

        // Start hidden above screen
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, startY);
    }

    // ===================== SHOW =====================
    public void PlayShowAnimation()
    {
        currentTween?.Kill();

        float targetYPos = targetY;

        currentTween = DOTween.Sequence()

            // Slide down
            .Append(rect.DOAnchorPosY(targetYPos, fallDuration)
                .SetEase(fallEase))

            // Bounce up
            .Append(rect.DOAnchorPosY(targetYPos + bounceHeight, bounceDuration)
                .SetEase(bounceUpEase))

            // Bounce down
            .Append(rect.DOAnchorPosY(targetYPos, bounceDuration)
                .SetEase(bounceDownEase));
    }

    // ===================== HIDE =====================
    public void PlayHideAnimation()
    {
        currentTween?.Kill();

        currentTween = rect.DOAnchorPosY(startY, hideDuration)
            .SetEase(hideEase);
    }

    void OnDisable()
    {
        currentTween?.Kill();
    }
}
