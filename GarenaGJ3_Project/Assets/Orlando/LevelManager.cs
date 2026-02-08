using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    // ================= TIMER =================
    [Header("Timer")]
    public float levelDuration = 120f;

    [Header("UI")]
    public Image fillBar;

    // ================= MILESTONE EVENTS =================
    [Header("Milestone Events (PAUSE)")]
    public MonoBehaviour event25;
    public MonoBehaviour event50;
    public MonoBehaviour event75;

    [Header("Milestone Timeout")]
    public float milestoneTimeout = 10f;

    // ================= RANDOM EVENTS =================
    [Header("Random Events (NO PAUSE)")]
    public List<MonoBehaviour> randomEvents;
    public float randomEventInterval = 3f;

    // ================= EVENT COOLDOWN =================
    [Header("Event Cooldown")]
    public float eventCooldownTime = 5f;

    // ================= OBJECT STATE =================
    [Header("Object State")]
    public GameObject minyakObject;
    public GameObject garnishObject;

    // ================= INTERNAL =================
    private ILevelEvent levelEvent25;
    private ILevelEvent levelEvent50;
    private ILevelEvent levelEvent75;
    private List<ILevelEvent> randomLevelEvents = new();

    private ILevelEvent activeMilestoneEvent;
    private Coroutine milestoneTimeoutRoutine;

    private Dictionary<ILevelEvent, float> eventCooldowns = new();

    private float currentTime;
    private bool isPaused;
    private bool levelFinished;

    private bool reached25;
    private bool reached50;
    private bool reached75;

    // ================= UNITY =================

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        levelEvent25 = event25 as ILevelEvent;
        levelEvent50 = event50 as ILevelEvent;
        levelEvent75 = event75 as ILevelEvent;

        minyakObject?.SetActive(false);
        garnishObject?.SetActive(false);

        foreach (var evt in randomEvents)
            if (evt is ILevelEvent e)
                randomLevelEvents.Add(e);
    }

    void Start()
    {
        if (randomLevelEvents.Count > 0)
            StartCoroutine(RandomEventLoop());
    }

    void Update()
    {
        if (levelFinished) return;
        if (isPaused) return;

        currentTime += Time.deltaTime;
        float progress = currentTime / levelDuration;

        if (fillBar != null)
            fillBar.fillAmount = progress;

        CheckMilestones(progress);

        // ===== WIN CONDITION =====
        if (progress >= 1f)
        {
            levelFinished = true;
            WinLoseManager.Instance.Win();
        }

        CleanupCooldowns();
    }

    // ================= MILESTONE CHECK =================

    void CheckMilestones(float progress)
    {
        if (progress >= 0.25f && !reached25)
        {
            reached25 = true;
            TriggerMilestone(levelEvent25);
        }
        else if (progress >= 0.50f && !reached50)
        {
            reached50 = true;
            TriggerMilestone(levelEvent50);
        }
        else if (progress >= 0.75f && !reached75)
        {
            reached75 = true;
            TriggerMilestone(levelEvent75);
        }
    }

    void TriggerMilestone(ILevelEvent evt)
    {
        if (evt == null) return;

        isPaused = true;
        activeMilestoneEvent = evt;
        evt.Activate();

        if (milestoneTimeoutRoutine != null)
            StopCoroutine(milestoneTimeoutRoutine);

        milestoneTimeoutRoutine = StartCoroutine(MilestoneTimeout(evt));
    }

    IEnumerator MilestoneTimeout(ILevelEvent evt)
    {
        yield return new WaitForSeconds(milestoneTimeout);

        if (evt == activeMilestoneEvent && evt.IsActive)
        {
            Debug.Log("❌ Milestone FAILED");

            if (evt == levelEvent25)
                WinLoseManager.Instance.Lose(LoseCause.LoseBland);
            else if (evt == levelEvent50)
                WinLoseManager.Instance.Lose(LoseCause.LoseRaw);
            else if (evt == levelEvent75)
                WinLoseManager.Instance.Lose(LoseCause.LoseBland);

            activeMilestoneEvent = null;
            isPaused = false;
        }
    }

    // ================= EVENT COMPLETE =================

    public void NotifyEventCompleted(ILevelEvent evt)
    {
        Transform t = ((MonoBehaviour)evt).transform;
        ParticleManager.Instance.SpawnWithSound(ParticleType.Achieve, t);

        // 🔒 cooldown event
        eventCooldowns[evt] = Time.time + eventCooldownTime;

        // only milestone can resume pause
        if (evt != activeMilestoneEvent)
        {
            Debug.Log("Event ini tidak berhak resume level");
            return;
        }

        if (evt == levelEvent25)
            minyakObject.SetActive(true);
        else if (evt == levelEvent50)
            garnishObject.SetActive(true);

        if (milestoneTimeoutRoutine != null)
        {
            StopCoroutine(milestoneTimeoutRoutine);
            milestoneTimeoutRoutine = null;
        }

        activeMilestoneEvent = null;
        isPaused = false;
    }

    // ================= RANDOM EVENT LOOP =================

    IEnumerator RandomEventLoop()
    {
        while (!levelFinished)
        {
            yield return new WaitForSeconds(randomEventInterval);

            if (randomLevelEvents.Count == 0)
                continue;

            bool eventTriggered = false;
            List<ILevelEvent> shuffled = new List<ILevelEvent>(randomLevelEvents);

            // shuffle
            for (int i = 0; i < shuffled.Count; i++)
            {
                int randIndex = Random.Range(i, shuffled.Count);
                (shuffled[i], shuffled[randIndex]) = (shuffled[randIndex], shuffled[i]);
            }

            foreach (var evt in shuffled)
            {
                if (evt == null) continue;
                if (evt.IsActive) continue;

                // cooldown check
                if (eventCooldowns.TryGetValue(evt, out float cooldownUntil))
                {
                    if (Time.time < cooldownUntil)
                        continue;
                }

                evt.Activate();
                eventTriggered = true;
                break;
            }

            if (!eventTriggered)
                Debug.Log("Semua random event aktif / cooldown");
        }
    }

    // ================= CLEAN COOLDOWN DICTIONARY =================

    void CleanupCooldowns()
    {
        var keys = new List<ILevelEvent>(eventCooldowns.Keys);
        foreach (var k in keys)
        {
            if (k == null)
                eventCooldowns.Remove(k);
        }
    }
}
