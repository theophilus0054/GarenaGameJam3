using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour, IDropTarget
{
    [Header("Rules")]
    public bool canMove = true;

    private Collider2D col;

    // 1 slot saja
    private DraggableItem occupant;

    public Collider2D Collider => col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public bool CanAccept(IDraggable draggable)
    {
        // harus DraggableItem2D biar bisa balik/snap dan tracking zone
        if (draggable is not DraggableItem item) return false;

        // kalau sudah ada isi, tidak bisa terima item lain
        if (occupant != null) return false;

        return true;
    } 

    public void OnHoverEnter(IDraggable draggable)
    {
        // optional highlight
        // Debug.Log($"{name} hover enter");
    }

    public void OnHoverExit(IDraggable draggable)
    {
        // optional unhighlight
        // Debug.Log($"{name} hover exit");
    }

    public void OnDrop(IDraggable draggable, Vector2 pointerWorld)
    {
        if (draggable is not DraggableItem item)
            return;

        // kalau tidak bisa terima, item balik
        if (!CanAccept(item))
        {
            item.ReturnToDragStart();
            return;
        }

        // aturan canMove
        if (canMove)
        {
            item.SnapTo(transform.position);
            occupant = item;
            item.SetCurrentZone(this);
        }
        else
        {
            // drop ditolak secara rule, jadi balik
            item.ReturnToDragStart();
        }
    }

    // dipanggil item ketika mulai drag, agar slot jadi kosong
    public void ClearOccupant(DraggableItem item)
    {
        if (occupant == item)
        {
            occupant = null;
        }
    }
}