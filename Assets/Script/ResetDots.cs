using UnityEngine;

public class ResetDots : MonoBehaviour
{
    public Animator animator;

    public void ResetFail()
    {
        animator.SetBool("Failed", false);
    }
    public void ResetSuccess()
    {
        animator.SetBool("Triggered", false);
    }
}
