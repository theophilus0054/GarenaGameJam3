using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour, IDropTarget
{
    [Header("Rules")]
    public bool fillable = true;

    [Header("Initial Occupant (Optional)")]
    [SerializeField] private DraggableItem initialOccupant;

    private Collider2D col;
    private DraggableItem occupant;

    public Collider2D Collider => col;

    public static System.Action<DraggableItem, DropZone> OnAnyDrop;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (initialOccupant == null) return;
        if (occupant != null) return;
        if(!fillable) return;

        occupant = initialOccupant;
        occupant.SnapTo(transform.position);
        occupant.SetCurrentZone(this);
        occupant.setHomeZone(this);
    }

    public DraggableItem GetOccupant() => occupant;

    public bool CanAccept(IDraggable draggable)
    {
        if (draggable is not DraggableItem) return false;
        if (occupant != null) return false;
        return true;
    } 

    public void OnHoverEnter(IDraggable draggable) { }
    public void OnHoverExit(IDraggable draggable) { }

    public void OnDrop(IDraggable draggable, Vector2 pointerWorld)
    {
        if (draggable is not DraggableItem item)
            return;

        if (!CanAccept(item))
        {
            item.ReturnToDragStart();
            return;
        }

        OnAnyDrop?.Invoke(item, this);

        if (!fillable)  
            item.ReturnToHome();
    }

    public void SetOccupant(DraggableItem item)
    {
        occupant = item;
    }

    public void ClearOccupant(DraggableItem item)
    {
        if (occupant == item)
            occupant = null;
    }
}
