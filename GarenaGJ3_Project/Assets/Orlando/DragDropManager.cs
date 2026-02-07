using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager Instance { get; private set; }

    [Header("Settings")]
    public float pickRadius = 0.15f;
    public LayerMask draggableLayer = ~0;
    public LayerMask dropTargetLayer = ~0;

    [Header("Ghost Settings")]
    [Range(0f, 1f)]
    public float ghostAlpha = 0.35f;
    public int ghostSortingOffset = -1;

    private Camera cam;

    private IDraggable currentDrag;
    private Vector2 dragOffset;
    private IDropTarget currentHoverTarget;

    // 👻 ghost
    private GameObject ghostObject;

    void Awake()
    {
        // =========================
        // SINGLETON SETUP
        // =========================
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject); // aktifkan kalau mau global antar scene

        cam = Camera.main;
        if (cam == null)
            Debug.LogError("[DragDropManager] Main Camera not found.");
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        MouseUpdate();
#else
        TouchUpdate();
#endif
    }

    #region Input
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
    #endregion

    void Begin(Vector2 screenPos)
    {
        if (cam == null) return;

        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        currentDrag = FindDraggableAt(worldPos);

        if (currentDrag == null) return;
        if (!currentDrag.CanBeDragged())
        {
            currentDrag = null;
            return;
        }

        dragOffset = (Vector2)currentDrag.Transform.position - worldPos;

        currentDrag.OnDragStart(worldPos);
        UpdateHoverTarget(worldPos);

    }

    void Move(Vector2 screenPos)
    {
        if (cam == null || currentDrag == null) return;

        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        currentDrag.Transform.position = worldPos + dragOffset;
        currentDrag.OnDrag(worldPos);

        UpdateHoverTarget(worldPos);
    }

    void End(Vector2 screenPos)
    {
        if (cam == null || currentDrag == null) return;

        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        if (currentHoverTarget != null && currentHoverTarget.CanAccept(currentDrag))
        {
            currentHoverTarget.OnDrop(currentDrag, worldPos);
        }
        else
        {
            if (currentDrag is DraggableItem item)
                item.ReturnToDragStart();
        }

        ClearHover(currentDrag);
        currentDrag.OnDragEnd(worldPos);

        DestroyGhost(); // 👻 ghost MATI DI SINI

        currentDrag = null;
    }


    // =============================
    // 👻 GHOST API
    // =============================
    public void SpawnGhostFrom(IDraggable source, Transform spawnTarget)
    {
        DestroyGhost();

        var sr = source.Transform.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        ghostObject = new GameObject($"Ghost_{source.Transform.name}");
        ghostObject.transform.position = spawnTarget.position;
        ghostObject.transform.rotation = source.Transform.rotation;
        ghostObject.transform.localScale = source.Transform.localScale;

        var ghostSR = ghostObject.AddComponent<SpriteRenderer>();
        ghostSR.sprite = sr.sprite;
        ghostSR.color = new Color(1f, 1f, 1f, ghostAlpha);
        ghostSR.sortingLayerID = sr.sortingLayerID;
        ghostSR.sortingOrder = sr.sortingOrder + ghostSortingOffset;
    }

    public void DestroyGhost()
    {
        if (ghostObject != null)
            Destroy(ghostObject);

        ghostObject = null;
    }

    #region Hover
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
    #endregion

    #region Raycast
    IDraggable FindDraggableAt(Vector2 worldPos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, pickRadius, draggableLayer);

        foreach (var col in hits)
        {
            if (col == null) continue;

            var d = col.GetComponentInParent<DraggableItem>(true);
            if (d != null)
                return d;
        }

        return null;
    }

    IDropTarget FindDropTargetAt(Vector2 worldPos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, pickRadius, dropTargetLayer);

        foreach (var col in hits)
        {
            if (col == null) continue;

            var comps = col.GetComponentsInParent<MonoBehaviour>(true);
            foreach (var c in comps)
                if (c is IDropTarget t)
                    return t;
        }

        return null;
    }
    #endregion
}
