using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    Move move;
    CapsuleCollider2D collider;
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    ParticleSystem splatter;
    private ParticleSystem.EmissionModule speedEmissionModule;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = player.GetComponent<Move>();
        collider = GetComponent<CapsuleCollider2D>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            move.grounded = true;
            float currentSpeed = rb.linearVelocity.magnitude;

            // 2. Calculate what percentage of our "max speed" we are currently traveling at
            float speedPercentage = Mathf.Clamp01(currentSpeed / 18);
            int particleCount = Mathf.RoundToInt(30 * speedPercentage);

            // Tell the particle system to instantly spit out that exact number
            if (particleCount > 0)
            {
                splatter.Emit(particleCount);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            move.grounded = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (move.scrunch)
        {
            collider.size = move.ScrunchSize;
            collider.offset = move.ScrunchOffset;
        }
        else
        {
            collider.size = move.flatSize;
            collider.offset = move.flatOffset;
        }
    }
}
