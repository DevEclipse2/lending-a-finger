using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    GameObject checkpointHandler;
    [SerializeField]
    int id;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetBool("Triggered",true);
        var thing = checkpointHandler.GetComponent<Checkpoints>();
        if (id >= thing.checkpointID)
        {
            thing.SetCheckpoint(transform.position);
        }
    }
}
