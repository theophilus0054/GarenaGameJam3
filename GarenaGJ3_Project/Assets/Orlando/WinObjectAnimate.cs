using UnityEngine;
using DG.Tweening;
using System.Collections;

public class WinObjectAnimate : MonoBehaviour
{
    public Transform obj1;
    public Transform obj2;
    public Transform obj3;
    public Transform obj4;
    public Transform obj5;
    public Transform obj6;

    public float bounceTime = 0.4f;
    public float obj3StretchTime = 0.4f;

    void Start()
    {
        obj1.gameObject.SetActive(false);
        obj2.gameObject.SetActive(false);
        obj3.gameObject.SetActive(false);
        obj4.gameObject.SetActive(false);
        obj5.gameObject.SetActive(false);
        obj6.gameObject.SetActive(false);
    }

    public void Play()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // Reset all scale
        ResetScale(obj1);
        ResetScale(obj2);
        ResetScale(obj3);
        ResetScale(obj4);
        ResetScale(obj5);
        ResetScale(obj6);

        // ================= OBJECT 1 + 2 =================
        obj1.gameObject.SetActive(true);
        obj2.gameObject.SetActive(true);

        Bounce(obj1);
        obj2.localScale = Vector3.zero;
        obj2.DOScale(Vector3.one, bounceTime)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        yield return new WaitForSecondsRealtime(bounceTime);

        // ================= OBJECT 3 =================
        obj3.gameObject.SetActive(true);

        // Simpan scale awal
        Vector3 baseScale = obj3.localScale;

        // Start flat
        obj3.localScale = new Vector3(baseScale.x, 0f, baseScale.z);

        obj3.DOScaleY(baseScale.y, obj3StretchTime)
            .SetEase(Ease.OutElastic)
            .SetUpdate(true);

        // ================= OBJECT 4 =================
        yield return new WaitForSecondsRealtime(0.2f);
        obj4.gameObject.SetActive(true);
        Bounce(obj4);

        // ================= OBJECT 5 (MID OBJECT 4) =================
        yield return new WaitForSecondsRealtime(bounceTime * 0.5f);
        obj5.gameObject.SetActive(true);
        Bounce(obj5);

        // ================= OBJECT 6 =================
        yield return new WaitForSecondsRealtime(bounceTime);
        obj6.gameObject.SetActive(true);
        Bounce(obj6);
    }

    void Bounce(Transform t)
    {
        t.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(t.DOScale(1.2f, bounceTime * 0.6f).SetEase(Ease.OutBack));
        seq.Append(t.DOScale(1f, bounceTime * 0.4f).SetEase(Ease.OutBounce));
    }

    void ResetScale(Transform t)
    {
        if (t == null) return;
        t.localScale = Vector3.zero;
    }
}
