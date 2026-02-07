using UnityEngine;
using System.Collections;

public class Cable : MonoBehaviour, ILevelEvent
{
    [SerializeField] private SwipeDetector swipeDetector;

    [Header("Swipe Down Rule")]
    [Range(0f, 1f)] public float minDownDot = 0.75f;
    [Range(0f, 1f)] public float maxHorizontalRatio = 0.5f;

    [Header("Time Limit")]
    [SerializeField] private float timeLimit = 10f;

    private bool armed;
    private bool isActive;
    private Coroutine timerCoroutine;

    public bool IsActive => isActive;

    void Awake()
    {
        if (swipeDetector == null)
            swipeDetector = GetComponent<SwipeDetector>();
    }

    // =========================
    // ILevelEvent
    // =========================
    public void Activate()
    {
        if (isActive) return;

        isActive = true;
        armed = false;

        gameObject.SetActive(true);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(SelfDestructTimer());
    }

    IEnumerator SelfDestructTimer()
    {
        yield return new WaitForSeconds(timeLimit);
        Debug.Log("[Cable] Self-destructed FAILED");
        Destroy(gameObject);
    }

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

        gameObject.SetActive(false);

        LevelManager.Instance.NotifyEventCompleted(this);
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
