using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SwipeDetector : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float minSwipeDistancePx = 60f;
    public float swipeCooldown = 0.2f;

    // Swipe event
    public event Action<Vector2> OnSwipe;

    // Pointer events (baru)
    public event Action OnPointerDownOnThis;
    public event Action OnPointerUp;

    private Camera cam;
    private Collider2D myCol;

    private Vector2 lastPos;
    private bool isPointerDown;

    private float travelSinceLastSwipe;
    private bool wasOverThis;
    private float lastSwipeTime;

    private bool startedOnThis;
    public bool StartedOnThis => startedOnThis;

    void Start()
    {
        cam = Camera.main;
        myCol = GetComponent<Collider2D>();

        if (cam == null) Debug.LogError("[SwipeDetector2D] Main Camera not found. Tag camera as MainCamera.");
        if (myCol == null) Debug.LogError("[SwipeDetector2D] Collider2D not found on this GameObject.");
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        MouseUpdate();
#else
        TouchUpdate();
#endif
    }

    void MouseUpdate()
    {
        if (Input.GetMouseButtonDown(0)) Begin(Input.mousePosition);
        if (Input.GetMouseButton(0)) Move(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) End(Input.mousePosition);
    }

    void TouchUpdate()
    {
        if (Input.touchCount == 0) return;
        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began) Begin(t.position);
        if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) Move(t.position);
        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) End(t.position);
    }

    void Begin(Vector2 screenPos)
    {
        isPointerDown = true;
        lastPos = screenPos;

        travelSinceLastSwipe = 0f;
        wasOverThis = IsPointerOverThis(screenPos);
        lastSwipeTime = -999f;

        startedOnThis = IsPointerOverThis(screenPos);
        if (startedOnThis)
            OnPointerDownOnThis?.Invoke();
    }

    void Move(Vector2 screenPos)
    {
        if (!isPointerDown || cam == null || myCol == null) return;

        travelSinceLastSwipe += Vector2.Distance(lastPos, screenPos);

        bool isOverNow = IsPointerOverThis(screenPos);
        bool crossedBoundary = (isOverNow != wasOverThis);

        if (crossedBoundary &&
            travelSinceLastSwipe >= minSwipeDistancePx &&
            Time.time - lastSwipeTime >= swipeCooldown)
        {
            travelSinceLastSwipe = 0f;
            lastSwipeTime = Time.time;

            Vector2 dir = (screenPos - lastPos).normalized;
            OnSwipe?.Invoke(dir);
        }

        wasOverThis = isOverNow;
        lastPos = screenPos;
    }

    void End(Vector2 screenPos)
    {
        if (!isPointerDown) return;
        isPointerDown = false;

        OnPointerUp?.Invoke();
        startedOnThis = false;
    }

    bool IsPointerOverThis(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        return hit == myCol;
    }
}