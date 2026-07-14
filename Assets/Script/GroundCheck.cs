using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    Move move;
    CapsuleCollider2D collider;

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
