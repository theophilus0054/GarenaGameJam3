using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Timer")]
    public float levelDuration = 120f;

    [Header("UI")]
    public Image fillBar;

    [Header("Milestone Events (PAUSE)")]
    public MonoBehaviour event25;
    public MonoBehaviour event50;
    public MonoBehaviour event75;

    [Header("Random Events (NO PAUSE)")]
    public List<MonoBehaviour> randomEvents;
    public float randomEventInterval = 3f;

    private ILevelEvent levelEvent25;
    private ILevelEvent levelEvent50;
    private ILevelEvent levelEvent75;
    private List<ILevelEvent> randomLevelEvents = new();

    private ILevelEvent activeMilestoneEvent; // 🔥 KUNCI UTAMA

    private float currentTime;
    private bool isPaused;

    private bool reached25;
    private bool reached50;
    private bool reached75;

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
        if (isPaused) return;

        currentTime += Time.deltaTime;
        float progress = currentTime / levelDuration;

        if (fillBar != null)
            fillBar.fillAmount = progress;

        CheckMilestones(progress);
    }

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
        activeMilestoneEvent = evt; // 🔒 lock siapa yang boleh resume
        evt.Activate();
    }

    // 🔥 SATU-SATUNYA JALUR RESUME
    public void NotifyEventCompleted(ILevelEvent evt)
    {
        if (evt != activeMilestoneEvent)
        {
            Debug.Log("Event ini tidak berhak resume level");
            return;
        }

        activeMilestoneEvent = null;
        isPaused = false;
    }

    IEnumerator RandomEventLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(randomEventInterval);

            if (randomLevelEvents.Count == 0)
                continue;

            bool eventTriggered = false;

            // Kita shuffle index supaya random tapi tetap bounded
            List<ILevelEvent> shuffled = new List<ILevelEvent>(randomLevelEvents);

            for (int i = 0; i < shuffled.Count; i++)
            {
                int randIndex = Random.Range(i, shuffled.Count);
                (shuffled[i], shuffled[randIndex]) = (shuffled[randIndex], shuffled[i]);
            }

            // Coba satu-satu sampai nemu yang belum aktif
            foreach (var evt in shuffled)
            {
                if (!evt.IsActive)
                {
                    evt.Activate();
                    eventTriggered = true;
                    break;
                }
            }

            // Kalau semua aktif → diam saja
            if (!eventTriggered)
            {
                Debug.Log("Semua random event sedang aktif, skip interval ini");
            }
        }
    }

}
