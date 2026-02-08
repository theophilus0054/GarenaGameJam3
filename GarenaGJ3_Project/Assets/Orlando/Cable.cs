using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Cable : MonoBehaviour, ILevelEvent
{
    [SerializeField] private SwipeDetector swipeDetector;

    [Header("Swipe Down Rule")]
    [Range(0f, 1f)] public float minDownDot = 0.75f;
    [Range(0f, 1f)] public float maxHorizontalRatio = 0.5f;

    [Header("Time Limit")]
    [SerializeField] private float timeLimit = 10f;

    [Header("Cable Animation")]
    [SerializeField] private float moveDownDistance = 2f;
    [SerializeField] private float moveDuration = 0.4f;
    [SerializeField] private float resetDelay = 3f;

    private bool armed;
    private bool isActive;
    private Coroutine timerCoroutine;
    private Coroutine particleCoroutine;

    private Vector3 startPos;
    private Tween moveTween;

    public bool IsActive => isActive;

    void Awake()
    {
        if (swipeDetector == null)
            swipeDetector = GetComponent<SwipeDetector>();

        startPos = transform.position;
    }

    // =========================
    // ACTIVATE
    // =========================
    public void Activate()
    {
        if (isActive) return;

        isActive = true;
        armed = false;

        gameObject.SetActive(true);
        transform.position = startPos;

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(SelfDestructTimer());

        if (particleCoroutine != null)
            StopCoroutine(particleCoroutine);

        particleCoroutine = StartCoroutine(SummonParticlesLoop());
    }

    IEnumerator SelfDestructTimer()
    {
        yield return new WaitForSeconds(timeLimit);
        Debug.Log("[Cable] Self-destructed FAILED");
        WinLoseManager.Instance.Lose(LoseCause.LosePlug);
    }

    // =========================
    // COMPLETE
    // =========================
    public void Complete()
    {
        if (!isActive) return;

        isActive = false;
        armed = false;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (particleCoroutine != null)
        {
            StopCoroutine(particleCoroutine);
            particleCoroutine = null;
        }

        // MOVE DOWN
        moveTween?.Kill();
        moveTween = transform.DOMoveY(startPos.y - moveDownDistance, moveDuration)
            .SetEase(Ease.OutBack);

        // RESET AFTER DELAY
        StartCoroutine(ResetCableAfterDelay());

        LevelManager.Instance.NotifyEventCompleted(this);
    }

    IEnumerator ResetCableAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);

        moveTween?.Kill();
        transform.DOMove(startPos, moveDuration).SetEase(Ease.OutBack);
    }

    // =========================
    // PARTICLE LOOP
    // =========================
    IEnumerator SummonParticlesLoop()
    {
        while (isActive)
        {
            ParticleManager.Instance.SpawnWithSound(ParticleType.Spark, transform);
            yield return new WaitForSeconds(0.25f);
            for(int i = 4; i > 0; i--)
            {
                ParticleManager.Instance.Spawn(ParticleType.Spark, transform);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    // =========================
    // Swipe Logic
    // =========================
    void OnEnable()
    {
        if (swipeDetector == null) return;

        swipeDetector.OnPointerDownOnThis += Arm;
        swipeDetector.OnPointerUp += Disarm;
        swipeDetector.OnSwipe += HandleSwipe;
    }

    void OnDisable()
    {
        if (swipeDetector == null) return;

        swipeDetector.OnPointerDownOnThis -= Arm;
        swipeDetector.OnPointerUp -= Disarm;
        swipeDetector.OnSwipe -= HandleSwipe;

        armed = false;
    }

    void Arm()
    {
        if (!isActive) return;
        armed = true;
    }

    void Disarm()
    {
        armed = false;
    }

    void HandleSwipe(Vector2 dir)
    {
        if (!isActive || !armed)
            return;

        float downDot = Vector2.Dot(dir, Vector2.down);
        float horizontalRatio = Mathf.Abs(dir.x);

        if (downDot >= minDownDot && horizontalRatio <= maxHorizontalRatio)
        {
            Complete();
        }
    }
}
