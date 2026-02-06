using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DraggableItem : MonoBehaviour, IDraggable
{
    private Collider2D col;

    public Transform Transform => transform;
    public Collider2D Collider => col;

    // posisi awal sebelum drag dimulai
    private Vector3 dragStartPosition;

    // dropzone yang saat ini ditempati item ini (kalau ada)
    public DropZone CurrentZone { get; private set; }

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public bool CanBeDragged() => true;

    public void OnDragStart(Vector2 pointerWorld)
    {
        dragStartPosition = transform.position;

        // saat mulai drag, lepas dulu dari slot sebelumnya (biar slotnya kosong lagi)
        if (CurrentZone != null)
        {
            CurrentZone.ClearOccupant(this);
            CurrentZone = null;
        }
    }

    public void OnDrag(Vector2 pointerWorld)
    {
        // optional
    }

    public void OnDragEnd(Vector2 pointerWorld)
    {
        // optional
    }

    public void ReturnToDragStart()
    {
        transform.position = dragStartPosition;
    }

    public void SnapTo(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    public void SetCurrentZone(DropZone zone)
    {
        CurrentZone = zone;
    }
}