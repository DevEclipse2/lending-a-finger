using UnityEngine;

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

    private void Start()
    {
        rb = player.gameObject.GetComponent<Rigidbody2D>();
        snapspeed = 20;
    }
    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
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
    }
}
