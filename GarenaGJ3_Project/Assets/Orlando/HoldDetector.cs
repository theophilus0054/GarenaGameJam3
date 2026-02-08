using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoldDetector : MonoBehaviour
{
    [Header("Hold Settings")]
    public float holdTime = 0.35f;
    public float holdMaxMovePx = 20f;

    public event Action OnHold;
    public event Action OnHoldRelease;

    private Camera cam;
    private Collider2D myCol;

    private Vector2 startPos;
    private float startTime;

    private bool isPointerDown;
    private bool holdingFired;

    void Start()
    {
        cam = Camera.main;
        myCol = GetComponent<Collider2D>();

        if (cam == null) Debug.LogError("[HoldDetector2D] Main Camera not found. Tag camera as MainCamera.");
        if (myCol == null) Debug.LogError("[HoldDetector2D] Collider2D not found on this GameObject.");
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
        holdingFired = false;

        startPos = screenPos;
        startTime = Time.time;
    }

    void Move(Vector2 screenPos)
    {
        if (!isPointerDown || holdingFired) return;
        if (cam == null || myCol == null) return;

        float dt = Time.time - startTime;
        float movePx = Vector2.Distance(startPos, screenPos);

        if (dt >= holdTime && movePx <= holdMaxMovePx && IsPointerOverThis(screenPos))
        {
            holdingFired = true;
            OnHold?.Invoke();
        }
    }

    void End(Vector2 screenPos)
    {
        if (!isPointerDown) return;
        isPointerDown = false;

        if (holdingFired)
            OnHoldRelease?.Invoke();
    }

    bool IsPointerOverThis(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        return hit == myCol;
    }
}