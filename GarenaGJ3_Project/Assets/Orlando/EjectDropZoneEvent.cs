using UnityEngine;
using System.Collections;

public class EjectDropZoneEvent : MonoBehaviour, ILevelEvent
{
    [Header("Target")]
    [SerializeField] private DropZone targetZone;

    [Header("Eject Motion")]
    [SerializeField] private float ejectHeight = 2f;
    [SerializeField] private float ejectDuration = 0.6f;

    public bool IsActive { get; private set; }

    private DraggableItem ejectedItem;

    void OnEnable()
    {
        DropZone.OnAnyDrop += OnAnyDrop;
    }

    void OnDisable()
    {
        DropZone.OnAnyDrop -= OnAnyDrop;
    }

    private Coroutine ejectCoroutine;

    public void Activate()
    {
        IsActive = true;

        ejectedItem = targetZone.GetOccupant();

        if (ejectedItem != null)
        {
            targetZone.ClearOccupant(ejectedItem);
            ejectedItem.SetCurrentZone(null);

            ejectCoroutine = StartCoroutine(EjectUp(ejectedItem));
        }
        else
        {
            Complete();
        }
    }


    void OnAnyDrop(DraggableItem item, DropZone zone)
    {
        if (!IsActive) return;

        if (zone != targetZone) return;

        if (item == ejectedItem)
        {
            if (ejectCoroutine != null)
            {
                StopCoroutine(ejectCoroutine);
                ejectCoroutine = null;
            }

            // 🔒 Final authority di event
            item.SnapTo(targetZone.transform.position);
            item.SetCurrentZone(targetZone);

            Complete();
        }
        else
        {
            Debug.Log($"[EjectDropZoneEvent] Wrong item [{item}] to [{ejectedItem}] or zone [{zone}] to [{targetZone}], returning item to start");
            item.ReturnToDragStart();
        }
    }




    public void Complete()
    {
        Debug.Log("[EjectDropZoneEvent] Completed");
        IsActive = false;
        ejectedItem = null;

        LevelManager.Instance.NotifyEventCompleted(this);
    }

    IEnumerator EjectUp(DraggableItem item)
    {
        Vector3 start = item.transform.position;
        Vector3 end = start + Vector3.up * ejectHeight;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / ejectDuration;
            if (!IsActive) yield break; // kalau event sudah complete, stop
            item.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

}
