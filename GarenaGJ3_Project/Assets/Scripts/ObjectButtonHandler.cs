using UnityEngine;
using DG.Tweening;

public class ObjectButtonHandler : MonoBehaviour
{
    public SoundType hoverSound = SoundType.Hover;
    public SoundType clickSound = SoundType.Click;

    public float hoverScaleParent = 1.1f;
    public float hoverScaleChild = 1.75f;

    public float hoverTweenTime = 0.15f;
    public float clickTweenTime = 0.1f;

    private Vector3 originalParentScale;
    private Transform[] children;
    private Vector3[] childOriginalScales;

    void Start()
    {
        originalParentScale = transform.localScale;

        children = GetComponentsInChildren<Transform>(true);
        childOriginalScales = new Vector3[children.Length];

        for (int i = 0; i < children.Length; i++)
            childOriginalScales[i] = children[i].localScale;
    }

    // HOVER
    void OnMouseEnter()
    {
        if(GetComponent<Rigidbody2D>() != null){
            if(GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
            {
                return;
            }
        }

        SoundManager.PlaySound(hoverSound);

        transform.DOScale(originalParentScale * hoverScaleParent, hoverTweenTime)
                 .SetEase(Ease.OutBack);

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == transform) continue;

            children[i].DOScale(childOriginalScales[i] * hoverScaleChild, hoverTweenTime)
                       .SetEase(Ease.OutBack);
        }
    }

    void OnMouseExit()
    {
        ResetScale();
    }

    // CLICK BOUNCE
    void OnMouseDown()
    {
        if(GetComponent<Rigidbody2D>() != null){
            if(GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
            {
                return;
            }
        }
        SoundManager.PlaySound(clickSound);
        ClickBounce();
    }

    void ResetScale()
    {
        transform.DOScale(originalParentScale, hoverTweenTime);

        for (int i = 0; i < children.Length; i++)
            children[i].DOScale(childOriginalScales[i], hoverTweenTime);
    }

    void ClickBounce()
    {
        Sequence seq = DOTween.Sequence();

        // 1️⃣ Child back to normal first
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == transform) continue;

            seq.Join(children[i].DOScale(childOriginalScales[i], clickTweenTime)
                                .SetEase(Ease.OutExpo));
        }

        // 2️⃣ Parent bounce after child done
        Vector3 scaleBig = originalParentScale * hoverScaleParent;
        Vector3 scaleSmall = originalParentScale * 0.9f;

        seq.Append(transform.DOScale(scaleSmall, clickTweenTime).SetEase(Ease.InQuad));
        seq.Append(transform.DOScale(scaleBig, clickTweenTime).SetEase(Ease.OutElastic));
    }
}
