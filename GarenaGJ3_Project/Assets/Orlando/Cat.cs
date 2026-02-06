using UnityEngine;

public class Cat : MonoBehaviour
{
    private SwipeDetector detector;
    [SerializeField] private int requiredSwipes = 3;

    private int swipeCount = 0;

    void Awake()
    {
        if (detector == null)
            detector = GetComponent<SwipeDetector>();
    }

    void OnEnable()
    {
        if (detector != null)
            detector.OnSwipe += HandleSwipe;
    }

    void OnDisable()
    {
        if (detector != null)
            detector.OnSwipe -= HandleSwipe;
    }

    void HandleSwipe(Vector2 dir)
    {
        swipeCount++;

        Debug.Log($"{name} diswipe {swipeCount}x");

        if (swipeCount >= requiredSwipes)
        {
            Destroy(gameObject);
        }
    }
}
