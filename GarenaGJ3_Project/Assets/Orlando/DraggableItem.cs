using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DraggableItem : MonoBehaviour, IDraggable
{
    private Collider2D col;

    public Transform Transform => transform;
    public Collider2D Collider => col;

    private Vector3 dragStartPosition;

    public DropZone CurrentZone { get; private set; }

    // =============================
    // FOLLOW VISUAL
    // =============================
    private Transform followTarget;
    private bool isFollowing;

    // ✅ simpan follow lama
    private Transform savedFollowTarget;
    private bool wasFollowingBeforeDrag;
    private DropZone homeZone;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        if(CurrentZone != null)
            homeZone = CurrentZone;
    }

    public void setHomeZone(DropZone zone)
    {
        homeZone = zone;
    }

    public void ReturnToHome()
    {
        if (homeZone != null)
        {
            ReturnToDragStart();
            SnapTo(homeZone.transform.position, false);
        }
        else
        {
            Debug.LogWarning("[DraggableItem] ReturnToHome called but homeZone is null");
        }
    }

    void Update()
    {
        if (isFollowing && followTarget != null)
        {
            transform.position = followTarget.position;
        }
    }

    public bool CanBeDragged() => true;

    public void OnDragStart(Vector2 pointerWorld)
    {
        // =============================
        // Simpan follow state
        // =============================
        wasFollowingBeforeDrag = isFollowing;
        savedFollowTarget = followTarget;

        if (homeZone != null && isFollowing)
        {
            DragDropManager.Instance.SpawnGhostFrom(this, homeZone.transform);
        }

        // Stop follow saat drag
        StopFollow();

        dragStartPosition = transform.position;
    }

    public void OnDrag(Vector2 pointerWorld) { }

    public void OnDragEnd(Vector2 pointerWorld) { }

    // =============================
    // FOLLOW API
    // =============================
    public void Follow(Transform target)
    {
        followTarget = target;
        isFollowing = true;
    }

    public void StopFollow()
    {
        isFollowing = false;
        followTarget = null;
    }

    // =============================
    // POSITION HELPERS
    // =============================
    public void ReturnToDragStart()
    {
        Debug.Log($"[DraggableItem] ReturnToDragStart to {dragStartPosition}");

        transform.position = dragStartPosition;

        // ✅ kalau sebelumnya follow → lanjut lagi
        if (wasFollowingBeforeDrag && savedFollowTarget != null)
        {
            Follow(savedFollowTarget);
        }
    }

    public void SnapTo(Vector3 worldPosition, bool stopFollow = true)
    {
        Debug.Log($"[DraggableItem] SnapTo {worldPosition}");

        if (stopFollow)
            StopFollow();
        transform.position = worldPosition;
    }

    public void SetCurrentZone(DropZone zone)
    {
        CurrentZone = zone;
    }
}
