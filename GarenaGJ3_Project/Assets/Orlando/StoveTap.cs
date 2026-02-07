using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(ClickDetector))]
public class StoveTap : MonoBehaviour, ILevelEvent
{
    private ClickDetector clickDetector;

    [Header("Tap Rule")]
    [SerializeField] private int maxCounter = 5;
    [SerializeField] private float timeLimit = 8f;
    [SerializeField] private float recoverSpeed = 1.5f;

    [Header("Visual Target - SCALE")]
    [SerializeField] private Transform scaleTarget;

    [Header("Visual Target - ROTATION")]
    [SerializeField] private Transform rotationTarget;

    [Header("Scale Settings")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1f;

    [Header("Rotation Settings (Z Axis)")]
    [SerializeField] private float minRotationZ = -180f;
    [SerializeField] private float maxRotationZ = 0f;

    [Header("Tween Settings")]
    [SerializeField] private float tweenDuration = 0.15f;
    [SerializeField] private Ease tweenEase = Ease.OutQuad;

    private float counter;
    private bool isActive;
    private Coroutine timerCoroutine;

    private Tween scaleTween;
    private Tween rotationTween;

    public bool IsActive => isActive;

    void Awake()
    {
        clickDetector = GetComponent<ClickDetector>();

        if (scaleTarget == null && rotationTarget == null)
            Debug.LogWarning("[StoveTap] Tidak ada visual target yang di-assign.");
    }

    // =========================
    // ILevelEvent
    // =========================
    public void Activate()
    {
        if (isActive) return;

        isActive = true;
        counter = maxCounter;

        if (scaleTarget != null)
            scaleTarget.gameObject.SetActive(true);

        UpdateVisual(true);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(SelfDestructTimer());
    }

    IEnumerator SelfDestructTimer()
    {
        yield return new WaitForSeconds(timeLimit);
        Debug.Log("[StoveTap] FAILED");

    }

    public void Complete()
    {
        if (!isActive) return;

        isActive = false;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        scaleTween?.Kill();
        rotationTween?.Kill();

        if (scaleTarget != null)
            scaleTarget.gameObject.SetActive(false);

        LevelManager.Instance.NotifyEventCompleted(this);
    }

    // =========================
    // TAP LOGIC
    // =========================
    void OnEnable()
    {
        if (clickDetector != null)
            clickDetector.OnTapCount += HandleTap;
    }

    void OnDisable()
    {
        if (clickDetector != null)
            clickDetector.OnTapCount -= HandleTap;
    }

    void HandleTap(int tapCount)
    {
        if (!isActive) return;

        counter -= 1f;
        counter = Mathf.Clamp(counter, 0f, maxCounter);

        UpdateVisual();

        if (counter <= 0f)
            Complete();
    }

    // =========================
    // RECOVER
    // =========================
    void Update()
    {
        if (!isActive) return;

        counter += recoverSpeed * Time.deltaTime;
        counter = Mathf.Clamp(counter, 0f, maxCounter);

        UpdateVisual();
    }

    // =========================
    // VISUAL UPDATE (DOTWEEN)
    // =========================
    void UpdateVisual(bool instant = false)
    {
        float t = 1f - (counter / maxCounter);
        t = Mathf.Clamp01(t);

        // -------- SCALE --------
        if (scaleTarget != null)
        {
            float scale = Mathf.Lerp(maxScale, minScale, t);

            scaleTween?.Kill();

            if (instant)
            {
                scaleTarget.localScale = Vector3.one * scale;
            }
            else
            {
                scaleTween = scaleTarget
                    .DOScale(scale, tweenDuration)
                    .SetEase(tweenEase);
            }
        }

        // -------- ROTATION --------
        if (rotationTarget != null)
        {
            float rotZ = Mathf.Lerp(minRotationZ, maxRotationZ, t);

            rotationTween?.Kill();

            if (instant)
            {
                rotationTarget.localRotation = Quaternion.Euler(0f, 0f, rotZ);
            }
            else
            {
                rotationTween = rotationTarget
                    .DOLocalRotate(
                        new Vector3(0f, 0f, rotZ),
                        tweenDuration,
                        RotateMode.Fast
                    )
                    .SetEase(tweenEase);
            }
        }
    }
}
