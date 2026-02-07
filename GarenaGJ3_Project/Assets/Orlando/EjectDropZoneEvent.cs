using UnityEngine;
using DG.Tweening;

public class EjectDropZoneEvent : MonoBehaviour, ILevelEvent
{
    [Header("Target")]
    [SerializeField] private DropZone targetZone;

    [Header("Visual Prefab (must have RandomFly)")]
    [SerializeField] private GameObject fairyPrefab;

    [Header("Direction")]
    [SerializeField] private float minAngle = -40f;
    [SerializeField] private float maxAngle = 40f;

    [Header("Motion")]
    [SerializeField] private float ejectDistance = 8f;
    [SerializeField] private float ejectDuration = 1.2f;
    [SerializeField] private Ease ejectEase = Ease.OutQuad;

    public bool IsActive { get; private set; }

    private DraggableItem ejectedItem;
    private RandomFly activeVisual;

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

        ejectedItem = targetZone.GetOccupant();

        if (ejectedItem == null)
        {
            Complete();
            return;
        }

        targetZone.ClearOccupant(ejectedItem);
        ejectedItem.SetCurrentZone(null);

        SpawnAndEject();
    }

    void SpawnAndEject()
    {
        GameObject go = Instantiate(
            fairyPrefab,
            targetZone.transform.position,
            Quaternion.identity
        );

        activeVisual = go.GetComponent<RandomFly>();
        if (activeVisual == null)
        {
            Debug.LogError("[EjectDropZoneEvent] Prefab has no RandomFly!");
            Destroy(go);
            return;
        }

        activeVisual.Play(
            targetZone.transform.position,
            minAngle,
            maxAngle,
            ejectDistance,
            ejectDuration,
            ejectEase
        );

        // 🔗 logic item ikut visual
        ejectedItem.Follow(activeVisual.transform);
    }

    void OnAnyDrop(DraggableItem item, DropZone zone)
    {
        if (!IsActive) return;
        if (zone != targetZone) return;

        if (item == ejectedItem)
        {
            activeVisual?.Stop();
            if (activeVisual != null)
                Destroy(activeVisual.gameObject);

            item.StopFollow();

            targetZone.SetOccupant(item);
            item.SnapTo(targetZone.transform.position);
            item.SetCurrentZone(targetZone);

            Complete();
        }
        else
        {
            item.ReturnToDragStart();
        }
    }

    public void Complete()
    {
        IsActive = false;
        ejectedItem = null;
        activeVisual = null;

        LevelManager.Instance.NotifyEventCompleted(this);
    }
}
