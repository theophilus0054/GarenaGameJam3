using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsButtonReset : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 startPos;
    private Quaternion startRot;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        startRot = transform.rotation;

        rb.bodyType = RigidbodyType2D.Static;
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
        rb.bodyType = RigidbodyType2D.Static;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = startPos;
        transform.rotation = startRot;
    }
}
