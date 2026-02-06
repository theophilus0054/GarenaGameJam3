using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ClickDetector : MonoBehaviour
{
    [Header("Tap Settings")]
    public float tapMaxMovePx = 15f;
    public float multiTapMaxDelay = 0.28f;

    public event Action<int> OnTapCount;

    private Camera cam;
    private Collider2D myCol;

    private Vector2 startPos;
    private bool isPointerDown;

    private int tapCount;
    private float lastTapTime;

    void Start()
    {
        cam = Camera.main;
        myCol = GetComponent<Collider2D>();

        if (cam == null) Debug.LogError("[ClickDetector2D] Main Camera not found. Tag camera as MainCamera.");
        if (myCol == null) Debug.LogError("[ClickDetector2D] Collider2D not found on this GameObject.");
    }

    void Update()
    {
        if (tapCount > 0 && Time.time - lastTapTime > multiTapMaxDelay)
            tapCount = 0;

#if UNITY_EDITOR || UNITY_STANDALONE
        MouseUpdate();
#else
        TouchUpdate();
#endif
    }

    void MouseUpdate()
    {
        if (Input.GetMouseButtonDown(0)) Begin(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) End(Input.mousePosition);
    }

    void TouchUpdate()
    {
        if (Input.touchCount == 0) return;
        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began) Begin(t.position);
        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) End(t.position);
    }

    void Begin(Vector2 screenPos)
    {
        isPointerDown = true;
        startPos = screenPos;
    }

    void End(Vector2 screenPos)
    {
        if (!isPointerDown) return;
        isPointerDown = false;

        if (cam == null || myCol == null) return;

        float movePx = Vector2.Distance(startPos, screenPos);

        // Tap harus release di atas object ini
        if (movePx <= tapMaxMovePx && IsPointerOverThis(screenPos))
        {
            RegisterTap();
        }
    }

    void RegisterTap()
    {
        if (Time.time - lastTapTime <= multiTapMaxDelay) tapCount++;
        else tapCount = 1;

        lastTapTime = Time.time;
        OnTapCount?.Invoke(tapCount);
    }

    bool IsPointerOverThis(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        return hit == myCol;
    }
}