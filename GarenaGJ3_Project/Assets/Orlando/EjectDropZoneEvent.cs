using UnityEngine;
using DG.Tweening;
using System.Collections;

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
    private Coroutine failCoroutine;

    void OnEnable()
    {
        DropZone.OnAnyDrop += OnAnyDrop;
    }

    void OnDisable()
    {
        DropZone.OnAnyDrop -= OnAnyDrop;
    }

    // ================= ACTIVATE =================
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

        if (failCoroutine != null)
            StopCoroutine(failCoroutine);

        failCoroutine = StartCoroutine(FailAfterDuration());
    }

    void SpawnAndEject()
    {
        GameObject go = Instantiate(fairyPrefab, targetZone.transform.position, Quaternion.identity);
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
        SoundManager.PlaySound(SoundType.FairyPickup);

        // item follow fairy
        ejectedItem.Follow(activeVisual.transform);
    }

    // ================= FAIL TIMER =================
    IEnumerator FailAfterDuration()
    {
        yield return new WaitForSeconds(ejectDuration);

        if (!IsActive) yield break;

        Debug.Log("[EjectDropZoneEvent] Fairy escaped → FAIL");

        // stop follow
        ejectedItem.StopFollow();
        ejectedItem.ReturnToDragStart();

        if (activeVisual != null)
            Destroy(activeVisual.gameObject);

        Complete();

        // destroy event object like Cable
        WinLoseManager.Instance.Lose(LoseCause.LoseStolen);
    }

    // ================= DROP HANDLER =================
    void OnAnyDrop(DraggableItem item, DropZone zone)
    {
        if (!IsActive) return;
        if (zone != targetZone) return;

        if (item == ejectedItem)
        {
            // SUCCESS
            StopCoroutine(failCoroutine);

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

    // ================= COMPLETE =================
    public void Complete()
    {
        IsActive = false;
        ejectedItem = null;
        activeVisual = null;
        SoundManager.PlaySound(SoundType.DropItem);

        LevelManager.Instance.NotifyEventCompleted(this);
    }
}
