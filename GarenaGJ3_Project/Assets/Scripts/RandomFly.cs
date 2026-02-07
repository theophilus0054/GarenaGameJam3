using UnityEngine;
using DG.Tweening;

public class RandomFly : MonoBehaviour
{
    private Tween flyTween;

    public void Play(
        Vector3 startPos,
        float minAngle,
        float maxAngle,
        float distance,
        float duration,
        Ease ease
    )
    {
        transform.position = startPos;

        float angle = Random.Range(minAngle, maxAngle);
        Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
        dir.Normalize();

        Vector3 target = startPos + (Vector3)(dir * distance);

        flyTween = transform.DOMove(target, duration)
            .SetEase(ease)
            .SetUpdate(true);
    }

    public void Stop()
    {
        flyTween?.Kill();
    }
}
