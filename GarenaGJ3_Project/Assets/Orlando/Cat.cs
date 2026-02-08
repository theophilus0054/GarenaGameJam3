using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cat : MonoBehaviour, ILevelEvent
{
    private SwipeDetector detector;
    private SpriteRenderer sr;
    private Animator animator;

    [Header("Cat Visual Data Pool")]
    [SerializeField] private List<CatVisualData> catDatas;

    private CatVisualData currentCat;

    [Header("Gameplay Settings")]
    [SerializeField] private int requiredSwipes = 3;
    [SerializeField] private float timeLimit = 10f;

    private int swipeCount;
    private bool isActive;
    private Coroutine timerCoroutine;

    private bool hasTriggeredMad;


    public bool IsActive => isActive;

    void Awake()
    {
        detector = GetComponent<SwipeDetector>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // ================= ACTIVATE ==================
    public void Activate()
    {
        if (isActive) return;

        ParticleManager.Instance.SpawnWithSound(ParticleType.Appear, transform);

        isActive = true;
        swipeCount = 0;
        hasTriggeredMad = false; // RESET FLAG
        gameObject.SetActive(true);

        RandomizeCat();

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(SelfDestructTimer());
    }


    // ================= RANDOMIZE CAT DATA ==================
    void RandomizeCat()
    {
        if (catDatas.Count == 0)
        {
            Debug.LogError("NO CAT DATA ASSIGNED!");
            return;
        }

        currentCat = catDatas[Random.Range(0, catDatas.Count)];
        sr.sprite = currentCat.normalSprite;
        sr.color = Color.white;

        animator.ResetTrigger("IsMad");
        animator.SetTrigger("IsReset");
    }

    // ================= TIMER ==================
    IEnumerator SelfDestructTimer()
    {
        yield return new WaitForSeconds(timeLimit);
        Debug.Log("[Cat] FAILED - Time out");
        WinLoseManager.Instance.Lose(LoseCause.LoseCat);
    }

    // ================= COMPLETE ==================
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
        animator.SetTrigger("IsReset");
        LevelManager.Instance.NotifyEventCompleted(this);
    }

    // ================= SWIPE LISTENER ==================
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

    // ================= SWIPE LOGIC ==================
    void HandleSwipe(Vector2 dir)
    {
        if (!isActive) return;

        swipeCount++;

        Debug.Log($"Swipe {swipeCount}");

        // Swipe pertama → ganti sprite mad (optional)
        if (swipeCount == 1)
        {
            sr.sprite = currentCat.madSprite;
        }

        // Swipe ke-3 → trigger MAD ONCE
        if (swipeCount >= requiredSwipes && !hasTriggeredMad)
        {
            hasTriggeredMad = true; // LOCK
            animator.SetTrigger("IsMad");
        }
    }


    // ================= ANIMATION EVENT ==================
    // Dipanggil di akhir animasi marah
    public void OnMadAnimationFinished()
    {
        ParticleManager.Instance.SpawnWithSound(ParticleType.Appear, transform);
        Complete();
    }
}
