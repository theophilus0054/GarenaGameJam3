using UnityEngine;
using System.Collections;

public class Cat : MonoBehaviour, ILevelEvent
{
    private SwipeDetector detector;

    [SerializeField] private int requiredSwipes = 3;
    [SerializeField] private float timeLimit = 10f;

    private int swipeCount;
    private bool isActive;
    private Coroutine timerCoroutine;

    // 🔑 expose ke LevelManager
    public bool IsActive => isActive;

    void Awake()
    {
        detector = GetComponent<SwipeDetector>();
    }

    public void Activate()
    {
        if (isActive) return; // safety

        isActive = true;
        swipeCount = 0;

        gameObject.SetActive(true);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(SelfDestructTimer());
    }

    IEnumerator SelfDestructTimer()
    {
        yield return new WaitForSeconds(timeLimit);
        Debug.Log("[Cat] Self-destructed FAILED");
        Destroy(gameObject);
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

        gameObject.SetActive(false);

        // 🔥 lapor ke LevelManager (dia yang mutusin resume atau tidak)
        LevelManager.Instance.NotifyEventCompleted(this);
    }

    void OnEnable()
    {
        if (detector != null)
            detector.OnSwipe += HandleSwipe;
    }

    void OnDisable()
    {
        if (detector != null)
            detector.OnSwipe -= HandleSwipe;
    }

    void HandleSwipe(Vector2 dir)
    {
        if (!isActive) return;

        swipeCount++;

        if (swipeCount >= requiredSwipes)
            Complete();
    }
}
