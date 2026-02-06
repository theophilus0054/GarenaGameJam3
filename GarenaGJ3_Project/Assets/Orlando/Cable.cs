using UnityEngine;

public class Cable : MonoBehaviour
{
    [SerializeField] private SwipeDetector swipeDetector;

    [Header("Swipe Down Rule")]
    [Range(0f, 1f)]
    public float minDownDot = 0.75f;

    [Range(0f, 1f)]
    public float maxHorizontalRatio = 0.5f;

    private bool armed = false;

    void Awake()
    {
        if (swipeDetector == null)
            swipeDetector = GetComponent<SwipeDetector>();

        if (swipeDetector == null)
            Debug.LogError("[Cable] SwipeDetector2D not found on this GameObject.");
    }

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
    }

    void Arm()
    {
        armed = true;
    }

    void Disarm()
    {
        armed = false;
    }

    void HandleSwipe(Vector2 dir)
    {
        if (!armed) return;

        float downDot = Vector2.Dot(dir, Vector2.down);
        float horizontalRatio = Mathf.Abs(dir.x);

        if (downDot >= minDownDot && horizontalRatio <= maxHorizontalRatio)
        {
            Destroy(gameObject);
        }
    }
}