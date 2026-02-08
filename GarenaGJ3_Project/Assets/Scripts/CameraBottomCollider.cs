using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBottomCollider : MonoBehaviour
{
    public float thickness = 1f; // ketebalan lantai

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        GameObject floor = new GameObject("CameraBottomFloor");
        floor.transform.parent = transform;

        BoxCollider2D col = floor.AddComponent<BoxCollider2D>();

        col.size = new Vector2(width * 2f, thickness);

        float yPos = transform.position.y - cam.orthographicSize - thickness / 2f;
        floor.transform.position = new Vector3(transform.position.x, yPos, 0f);
    }
}
