using UnityEngine;

public class DragRequiredEvent : MonoBehaviour, ILevelEvent
{
    [Header("Requirement")]
    [SerializeField] private DraggableItem requiredItem;
    [SerializeField] private DropZone targetZone;

    public bool IsActive { get; private set; }

    void OnEnable()
    {
        DropZone.OnAnyDrop += OnAnyDrop;
    }

    void OnDisable()
    {
        DropZone.OnAnyDrop -= OnAnyDrop;
    }

    public void Activate()
    {
        IsActive = true;
        Debug.Log("DragRequiredEvent Activated");
    }

    public void Complete()
    {
        IsActive = false;
        Debug.Log("DragRequiredEvent Completed");
        SoundManager.PlaySound(SoundType.Seasoning);

        LevelManager.Instance.NotifyEventCompleted(this);
    }

    void OnAnyDrop(DraggableItem item, DropZone zone)
    {
        if (!IsActive) return;

        if (zone != targetZone) return;

        if (item == requiredItem)
        {
            Complete();
        }
        else
        {
            item.ReturnToHome();
        }
    }
}
