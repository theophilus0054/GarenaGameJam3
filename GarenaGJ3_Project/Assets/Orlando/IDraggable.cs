using UnityEngine;

public interface IDraggable
{
    Transform Transform { get; }
    Collider2D Collider { get; }

    void OnDragStart(Vector2 pointerWorld);
    void OnDrag(Vector2 pointerWorld);
    void OnDragEnd(Vector2 pointerWorld);

    bool CanBeDragged();
}