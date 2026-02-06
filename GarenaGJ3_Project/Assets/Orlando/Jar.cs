using UnityEngine;

public class Jar : MonoBehaviour
{
    private HoldDetector detector;

    [SerializeField] private float holdToBreakTime = 3f; // detik (setelah OnHold terpanggil)

    private float holdAccumulated = 0f;
    private bool isHolding = false;

    void Awake()
    {
        detector = GetComponent<HoldDetector>();
        if (detector == null)
        {
            Debug.LogError("[Jar] HoldDetector not found on this GameObject.");
        }
    }

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

        // safety reset
        isHolding = false;
        holdAccumulated = 0f;
    }

    void Update()
    {
        if (!isHolding) return;

        holdAccumulated += Time.deltaTime;

        if (holdAccumulated >= holdToBreakTime)
        {
            Destroy(gameObject);
        }
    }

    void HandleHoldStart()
    {
        isHolding = true;
        holdAccumulated = 0f; // mulai hitung dari 0 saat hold terdeteksi
    }

    void HandleHoldRelease()
    {
        isHolding = false;
        holdAccumulated = 0f;
    }
}