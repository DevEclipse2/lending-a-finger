using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{

    public bool jump = false;
    public bool jumpLF = false;
    public float heldtime = 0.0f;

    [SerializeField]
    private float holdThresh = 0.15f;

    Rigidbody2D rigidbody
    [SerializeField]
    Vector2 flatSize;
    [SerializeField]
    Vector2 ScrunchSize;
    [SerializeField]
    Vector2 flatOffset;
    [SerializeField]
    Vector2 ScrunchOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void onJump(InputAction.CallbackContext context)
    {
        if (context.started) 
        {
            jump = true;
        }
        if (context.canceled)
        {
            jump = false;

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (heldtime > holdThresh)
        {
            Debug.Log("hold");

        }
        if (jump)
        {
            if (!jumpLF)
            {
                jumpLF = true;
            }
            heldtime += Time.deltaTime;
        }
        else
        {
            if (jumpLF)
            {
                jumpLF = false;
                //do heldtime threshes here
                //maybe o.2 seconds is a tap
                //etc etc 
                if (heldtime > holdThresh) {
                    Debug.Log("hold");
                
                
                }
                else
                {
                    Debug.Log("tap");

                }
                heldtime = 0.0f;
            }
        }
    }
}
