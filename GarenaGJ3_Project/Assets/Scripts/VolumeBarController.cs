using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class VolumeBarController : MonoBehaviour
{
    [Header("Fall (Show)")]
    [SerializeField] float fallDuration = 0.4f;
    [SerializeField] Ease fallEase = Ease.InQuad;

    [Header("Bounce (Show)")]
    [SerializeField] float bounceHeight = 20f;   // pixel
    [SerializeField] float bounceDuration = 0.15f;
    [SerializeField] Ease bounceUpEase = Ease.OutQuad;
    [SerializeField] Ease bounceDownEase = Ease.InQuad;

    [Header("Hide (Up)")]
    [SerializeField] float hideDuration = 0.25f;
    [SerializeField] Ease hideEase = Ease.OutQuad;

    [Header("Target Y (UI)")]
    [SerializeField] RectTransform targetY;

    RectTransform rect;
    Tween currentTween;
    float startY;
    bool isActive;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        startY = rect.anchoredPosition.y; // simpan Y awal saja
    }

    public void Toggle()
    {
        if (isActive)
            HideSettings();
        else
            ShowSettings();

        isActive = !isActive;
    }

    public void ShowSettings()
    {
        PlayShowAnimation();
    }

    public void HideSettings()
    {
        PlayHideAnimation();
    }

    // ===================== ANIMATIONS =====================

    void PlayShowAnimation()
    {
        currentTween?.Kill();

        float targetYPos = targetY.anchoredPosition.y;

        currentTween = DOTween.Sequence()

            // JATUH (Y saja)
            .Append(rect.DOAnchorPosY(targetYPos, fallDuration)
                .SetEase(fallEase))

            // BOUNCE UP
            .Append(rect.DOAnchorPosY(targetYPos + bounceHeight, bounceDuration)
                .SetEase(bounceUpEase))

            // BOUNCE DOWN
            .Append(rect.DOAnchorPosY(targetYPos, bounceDuration)
                .SetEase(bounceDownEase));
    }

    void PlayHideAnimation()
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
