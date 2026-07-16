using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTracking : MonoBehaviour
{
    public Transform player;           // Reference to the player
    public float smoothSpeed = 0.125f; // Smoothing speed
    public Vector3 offset;             
    bool Valid;
    public float snapspeed = 15.0f;
    public float snapspeedLowBound = 4.0f;
    private Rigidbody2D rb;
    public bool snap = false;
    public float shiftAmount = 13.0f;

    private float targetZoom = 49;
    public float zoomSpeed = 0.05f;
    public float smoothZoomSpeed = 10f;
    public float minZoom = 44f;
    public float maxZoom = 60f;
    Camera cam;
    private void Start()
    {
        cam = GetComponent<Camera>();
        rb = player.gameObject.GetComponent<Rigidbody2D>();
        snapspeed = 20;
    }
    void LateUpdate()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        float normalizedX = (mousePos.x / Screen.width) * 2f - 1f;
        float normalizedY = (mousePos.y / Screen.height) * 2f - 1f;

        normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);
        normalizedY = Mathf.Clamp(normalizedY, -1f, 1f);

        Vector3 targetOffset = new Vector3(normalizedX, normalizedY, 0f) * shiftAmount;
        Vector3 desiredPosition = player.position + targetOffset + offset;
        if (Mathf.Abs(rb.linearVelocityY) > snapspeed)
        {

            snap = true;
        }
        if (snap)
        {
            transform.position = desiredPosition;
            if (Mathf.Abs(rb.linearVelocityY) < snapspeedLowBound)
            {
                snap = false;
            }
        }
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        float scrollDelta = Mouse.current.scroll.ReadValue().y; // makes it not mind numbingly slow

        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            targetZoom -= scrollDelta * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * smoothZoomSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * smoothZoomSpeed);
        }
    }
}
