using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour, IDropTarget
{
    [Header("Rules")]
    public bool canMove = true;

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

        occupant = initialOccupant;
        occupant.SnapTo(transform.position);
        occupant.SetCurrentZone(this);
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

        if (canMove)
        {
            occupant = item;
            item.SnapTo(transform.position);
            item.SetCurrentZone(this);

            OnAnyDrop?.Invoke(item, this);
        }
        else
        {
            item.ReturnToDragStart();
        }
    }

    public void ClearOccupant(DraggableItem item)
    {
        if (occupant == item)
            occupant = null;
    }
}
