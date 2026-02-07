using UnityEngine;
using System.Collections;

public class Jar : MonoBehaviour, ILevelEvent
{
    private HoldDetector detector;

    [Header("Hold Settings")]
    [SerializeField] private float holdToBreakTime = 3f;

    [Header("Time Limit")]
    [SerializeField] private float timeLimit = 10f;

    private float holdAccumulated;
    private bool isHolding;
    private bool isActive;
    private Coroutine timerCoroutine;

    public bool IsActive => isActive;

    void Awake()
    {
        detector = GetComponent<HoldDetector>();
    }

    // =========================
    // ILevelEvent
    // =========================
    public void Activate()
    {
        if (isActive) return;

        isActive = true;
        isHolding = false;
        holdAccumulated = 0f;

        gameObject.SetActive(true);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(SelfDestructTimer());
    }

    IEnumerator SelfDestructTimer()
    {
        yield return new WaitForSeconds(timeLimit);
        Debug.Log("[Jar] Self-destructed FAILED");
        Destroy(gameObject);
    }

    public void Complete()
    {
        if (!isActive) return;

        isActive = false;
        isHolding = false;
        holdAccumulated = 0f;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        gameObject.SetActive(false);

        LevelManager.Instance.NotifyEventCompleted(this);
    }

    // =========================
    // Hold Logic
    // =========================
    void OnEnable()
    {
        if (detector != null)
        {
            detector.OnHold += HandleHoldStart;
            detector.OnHoldRelease += HandleHoldRelease;
        }
    }

    void OnDisable()
    {
        if (detector != null)
        {
            detector.OnHold -= HandleHoldStart;
            detector.OnHoldRelease -= HandleHoldRelease;
        }

        isHolding = false;
        holdAccumulated = 0f;
    }

    void Update()
    {
        if (!isActive || !isHolding)
            return;

        holdAccumulated += Time.deltaTime;

        if (holdAccumulated >= holdToBreakTime)
        {
            Complete();
        }
    }

    void HandleHoldStart()
    {
        if (!isActive) return;

        isHolding = true;
        holdAccumulated = 0f;
    }

    void HandleHoldRelease()
    {
        if (!isActive) return;

        isHolding = false;
        holdAccumulated = 0f;
    }
}
