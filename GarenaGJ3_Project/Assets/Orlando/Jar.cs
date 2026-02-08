using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Jar : MonoBehaviour, ILevelEvent
{
    private HoldDetector detector;

    [Header("Hold Settings")]
    public float holdToBreakTime = 1f;

    [Header("Time Limit")]
    public float timeLimit = 10f;

    [Header("Wobble Settings")]
    public float minWobbleAngle = 2f;
    public float maxWobbleAngle = 20f;
    public float wobbleSpeed = 0.6f;
    public float maxWobbleDestroyTime = 1f;

    private bool isHolding;
    private bool isActive;
    private Tween wobbleTween;

    private float wobbleProgress; // 0 -> 1
    private float currentWobbleAngle;

    private float maxWobbleTimer;

    public bool IsActive => isActive;

    void Awake()
    {
        detector = GetComponent<HoldDetector>();
    }

    // =========================
    public void Activate()
    {
        if (isActive) return;

        isActive = true;
        isHolding = false;
        wobbleProgress = 0f;
        maxWobbleTimer = 0f;

        gameObject.SetActive(true);
        SoundManager.PlaySound(SoundType.BottleWiggle);
        StartWobble();
    }

    public void Complete()
    {
        if (!isActive) return;

        isActive = false;
        StopWobble();
        transform.rotation = Quaternion.identity;

        LevelManager.Instance.NotifyEventCompleted(this);
    }

    // =========================
    void Update()
    {
        if (!isActive) return;

        // Idle = makin wobble
        if (!isHolding)
            wobbleProgress += Time.deltaTime / timeLimit;
        // Hold = calming down
        else
            wobbleProgress -= Time.deltaTime / holdToBreakTime;

        wobbleProgress = Mathf.Clamp01(wobbleProgress);
        currentWobbleAngle = Mathf.Lerp(minWobbleAngle, maxWobbleAngle, wobbleProgress);

        // ================= SUCCESS =================
        if (wobbleProgress <= 0f)
        {
            Debug.Log("Jar stabilized - COMPLETE");
            Complete();
            return;
        }

        // ================= FAIL TIMER =================
        if (Mathf.Approximately(wobbleProgress, 1f))
        {
            maxWobbleTimer += Time.deltaTime;
            if (maxWobbleTimer >= maxWobbleDestroyTime)
            {
                Debug.Log("Jar exploded due to wobble overload");
                WinLoseManager.Instance.Lose(LoseCause.LoseBottle);
            }
        }
        else
        {
            maxWobbleTimer = 0f;
        }
    }


    // =========================
    void OnEnable()
    {
        if (detector != null)
        {
            detector.OnHold += () => isHolding = true;
            detector.OnHoldRelease += () => isHolding = false;
        }
    }

    void OnDisable()
    {
        StopWobble();
    }

    // =========================
    void StartWobble()
    {
        StopWobble();

        wobbleTween = DOTween.To(
            () => 0f,
            x =>
            {
                float angle = Mathf.Sin(x * Mathf.PI * 2f) * currentWobbleAngle;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            },
            1f,
            wobbleSpeed
        )
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear);
    }

    void StopWobble()
    {
        if (wobbleTween != null)
        {
            wobbleTween.Kill();
            wobbleTween = null;
        }
    }
}
