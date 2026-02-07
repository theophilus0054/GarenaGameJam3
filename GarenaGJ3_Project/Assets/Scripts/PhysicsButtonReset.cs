using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsButtonReset : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 startPos;
    private Quaternion startRot;
    private SpriteRenderer spriteRenderer;

    private Color defaultColor = Color.white;
    private Color hoverColor = MainMenuManager.Hex("#FFD966");   // yellow-ish hover
    private Color clickColor = MainMenuManager.Hex("#FF6B6B");   // red-ish click

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startRot = transform.rotation;

        rb.bodyType = RigidbodyType2D.Static;

        defaultColor = spriteRenderer.color;
    }

    void OnMouseEnter()
    {
        if (!MainMenuManager.Instance.physicsModeActive) return;
        transform.DOScale(1.1f, 0.1f);
        SetColor(hoverColor);
    }

    void OnMouseExit()
    {
        if (!MainMenuManager.Instance.physicsModeActive) return;
        transform.DOScale(1f, 0.1f);
        SetColor(defaultColor);
    }

    void OnMouseDown()
    {
        if (!MainMenuManager.Instance.physicsModeActive) return;
        SetColor(clickColor);
    }

    public void EnablePhysics()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.AddTorque(Random.Range(-15f, 15f));
    }

    public void DisablePhysicsAndReset()
    {
        SetColor(defaultColor);
        rb.bodyType = RigidbodyType2D.Static;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = startPos;
        transform.rotation = startRot;
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}
