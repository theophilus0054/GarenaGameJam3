using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    [Header("Settings")]
    public float pickRadius = 0.15f;          // 0.01 terlalu kecil, gampang miss
    public LayerMask draggableLayer = ~0;
    public LayerMask dropTargetLayer = ~0;

    private Camera cam;

    private IDraggable currentDrag;
    private Vector2 dragOffset;

    private IDropTarget currentHoverTarget;

    void Awake()
    {
        cam = Camera.main;
        if (cam == null)
            Debug.LogError("[DragDropManager] Main Camera not found. Tag your camera as MainCamera.");
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
        var t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began) Begin(t.position);
        if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) Move(t.position);
        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) End(t.position);
    }

    void Begin(Vector2 screenPos)
    {
        if (cam == null)
        {
            Debug.LogError("[DragDropManager] Begin() aborted: Camera is null.");
            return;
        }

        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        currentDrag = FindDraggableAt(worldPos);

        if (currentDrag == null)
        {
            Debug.LogWarning($"[DragDropManager] No draggable found at {worldPos}. " +
                             $"Try increasing pickRadius or check draggableLayer.");
            return;
        }

        if (!currentDrag.CanBeDragged())
        {
            Debug.LogWarning($"[DragDropManager] Draggable '{currentDrag.Transform.name}' cannot be dragged right now.");
            currentDrag = null;
            return;
        }

        dragOffset = (Vector2)currentDrag.Transform.position - worldPos;

        currentDrag.OnDragStart(worldPos);
        UpdateHoverTarget(worldPos);

        Debug.Log($"[DragDropManager] Drag started: {currentDrag.Transform.name}");
    }

    void Move(Vector2 screenPos)
    {
        if (cam == null)
        {
            Debug.LogError("[DragDropManager] Move() aborted: Camera is null.");
            return;
        }

        if (currentDrag == null) return;

        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        currentDrag.Transform.position = worldPos + dragOffset;
        currentDrag.OnDrag(worldPos);

        UpdateHoverTarget(worldPos);
    }

    void End(Vector2 screenPos)
    {
        if (cam == null)
        {
            Debug.LogError("[DragDropManager] End() aborted: Camera is null.");
            return;
        }

        if (currentDrag == null) return;

        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        if (currentHoverTarget != null && currentHoverTarget.CanAccept(currentDrag))
        {
            currentHoverTarget.OnDrop(currentDrag, worldPos);
            Debug.Log($"[DragDropManager] Dropped '{currentDrag.Transform.name}' on '{(currentHoverTarget as MonoBehaviour)?.name}'");
        }
        else
        {
            Debug.LogWarning($"[DragDropManager] Drop rejected for '{currentDrag.Transform.name}'. Returning to start position.");

            if (currentDrag is DraggableItem item)
                item.ReturnToDragStart();
            else
                Debug.LogError($"[DragDropManager] '{currentDrag.Transform.name}' is IDraggable but not DraggableItem. Cannot return to start position.");
        }

        ClearHover(currentDrag);

        currentDrag.OnDragEnd(worldPos);
        Debug.Log($"[DragDropManager] Drag ended: {currentDrag.Transform.name}");

        currentDrag = null;
    }

    IDraggable FindDraggableAt(Vector2 worldPos)
    {
        // 1) Debug check: ada collider sama sekali ga?
        Collider2D[] anyHits = Physics2D.OverlapCircleAll(worldPos, pickRadius);
        if (anyHits.Length == 0)
        {
            Debug.LogWarning($"[DragDropManager] No Collider2D hit at {worldPos}. pickRadius={pickRadius}");
            return null; // EARLY RETURN (biar ga dobel warning)
        }

        // 2) Check layer mask draggable
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, pickRadius, draggableLayer);
        if (hits.Length == 0)
        {
            Debug.LogWarning($"[DragDropManager] Collider2D exists at {worldPos} but none match draggableLayer={draggableLayer.value}");
            return null; // EARLY RETURN
        }

        // 3) Cari DraggableItem paling aman (kamu pakai class ini)
        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;

            DraggableItem direct = col.GetComponentInParent<DraggableItem>(true);
            if (direct != null)
                return direct;
        }

        // 4) Fallback: cari lewat interface (kalau ada implement lain selain DraggableItem)
        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;

            var comps = col.GetComponentsInParent<MonoBehaviour>(true);
            for (int c = 0; c < comps.Length; c++)
            {
                if (comps[c] is IDraggable d)
                    return d;
            }
        }

        Debug.LogWarning($"[DragDropManager] Collider(s) match draggableLayer at {worldPos} but no DraggableItem/IDraggable found in parent chain.");
        return null;
    }

    IDropTarget FindDropTargetAt(Vector2 worldPos)
    {
        Collider2D[] anyHits = Physics2D.OverlapCircleAll(worldPos, pickRadius);
        // kalau ga ada collider sama sekali, normal
        if (anyHits.Length == 0) return null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, pickRadius, dropTargetLayer);
        if (hits.Length == 0) return null;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;

            // paling aman: kalau kamu pakai class DropZone, boleh ganti jadi direct search
            // var dz = col.GetComponentInParent<DropZone>(true);
            // if (dz != null) return dz;

            var comps = col.GetComponentsInParent<MonoBehaviour>(true);
            for (int c = 0; c < comps.Length; c++)
            {
                if (comps[c] is IDropTarget t)
                    return t;
            }
        }

        Debug.LogWarning($"[DragDropManager] Collider(s) match dropTargetLayer at {worldPos} but none implement IDropTarget.");
        return null;
    }

    void UpdateHoverTarget(Vector2 pointerWorld)
    {
        if (currentDrag == null) return;

        IDropTarget newTarget = FindDropTargetAt(pointerWorld);

        if (newTarget == currentHoverTarget) return;

        if (currentHoverTarget != null)
            currentHoverTarget.OnHoverExit(currentDrag);

        currentHoverTarget = newTarget;

        if (currentHoverTarget != null)
            currentHoverTarget.OnHoverEnter(currentDrag);
    }

    void ClearHover(IDraggable draggable)
    {
        if (currentHoverTarget != null)
        {
            currentHoverTarget.OnHoverExit(draggable);
            currentHoverTarget = null;
        }
    }
}