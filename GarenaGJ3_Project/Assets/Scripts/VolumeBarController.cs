using UnityEngine;
using System.Collections;

public class VolumeBarController : MonoBehaviour
{
    public Vector3 hiddenPosition;
    public Vector3 showPosition;

    [Header("Bounce Settings")]
    public float bounceHeight = 0.5f;
    public float bounceDuration = 0.6f;
    public int bounceCount = 3;

    [Header("Spawn Settings")]
    public float spawnHeight = 1.2f;

    private bool isVisible = false;
    private Coroutine bounceRoutine;

    void Awake()
    {
        transform.position = hiddenPosition;
    }

    public bool isVolumeBarVisible { get { return isVisible; } }

    public void Toggle()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }

    void Show()
    {
        isVisible = true;

        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        bounceRoutine = StartCoroutine(BounceIn());
    }

    void Hide()
    {
        isVisible = false;

        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        transform.position = hiddenPosition;
    }

    IEnumerator BounceIn()
    {
        float elapsed = 0f;

        Vector3 startPos = showPosition + Vector3.up * spawnHeight;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / bounceDuration;

            // bounce curve (sinus + damping)
            float bounce =
                Mathf.Abs(Mathf.Sin(t * bounceCount * Mathf.PI)) *
                bounceHeight *
                (1f - t);

            float y = Mathf.Lerp(startPos.y, showPosition.y, t) + bounce;

            transform.position = new Vector3(
                showPosition.x,
                y,
                showPosition.z
            );

            yield return null;
        }

        transform.position = showPosition;
    }
}
