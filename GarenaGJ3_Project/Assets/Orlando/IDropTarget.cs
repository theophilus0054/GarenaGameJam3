using UnityEngine;

public interface IDropTarget
{
    Collider2D Collider { get; }

    bool CanAccept(IDraggable draggable);
    void OnDrop(IDraggable draggable, Vector2 pointerWorld);
    void OnHoverEnter(IDraggable draggable);
    void OnHoverExit(IDraggable draggable);
}