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

    public float popUpForce = 5f;        // tinggi lompat
    public float randomSideForce = 2f;   // dorong kiri kanan
    public float randomTorque = 15f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startRot = transform.rotation;

        rb.bodyType = RigidbodyType2D.Static;

        defaultColor = spriteRenderer.color;
    }

    public void EnablePhysics()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Random direction
        float side = Random.Range(-randomSideForce, randomSideForce);
        Vector2 force = new Vector2(side, popUpForce);

        rb.AddForce(force, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-randomTorque, randomTorque), ForceMode2D.Impulse);
    }


    public void DisablePhysicsAndReset()
    {
        SetColor(defaultColor);
        rb.bodyType = RigidbodyType2D.Static;
        
        if (rb.bodyType != RigidbodyType2D.Static)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

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
