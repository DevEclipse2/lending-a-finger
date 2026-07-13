using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    public bool vertical;
    public bool left;
    public bool grounded;
    public bool jump = false;
    public bool jumpLF = false;
    public float heldtime = 0.0f;

    [SerializeField]
    private float holdThresh = 0.15f;

    Rigidbody2D rigidbody;
    CapsuleCollider2D collider;
    [SerializeField]
    Vector2 flatSize;
    [SerializeField]
    Vector2 ScrunchSize;
    [SerializeField]
    Vector2 flatOffset;
    [SerializeField]
    Vector2 ScrunchOffset;

    [SerializeField]
    float maxHold = 3.0f;
    [SerializeField]
    float shunt = 13.0f;

    [SerializeField]
    Vector2 jumpForce;
    [SerializeField]
    Vector2 jumpForceLeft;
    [SerializeField]
    float rotationForce;
    [SerializeField]
    float rotationForceLeft;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        grounded = true;
        Debug.Log("grounded");
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
        Debug.Log("not grounded");

    }


    // Update is called once per frame
    void Update()
    {
        if (heldtime > holdThresh)
        {
            Debug.Log("hold");
            collider.size = ScrunchSize;
            collider.offset = ScrunchOffset;
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
                if (heldtime < holdThresh)
                {
                    Debug.Log("tap");
                    if (grounded)
                    {
                        vertical = transform.rotation == Quaternion.Euler(new Vector3(0, 0, 0));
                        if (!vertical)
                        {
                            vertical = true;
                            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            rigidbody.linearVelocity = Vector2.zero;
                            rigidbody.angularVelocity = 0;
                        }
                        else
                        {
                            //vertical is already true 
                            left = !left;
                        }
                        
                    }
                }
                else
                {
                    if (grounded)
                    {
                        if (!vertical)
                        {
                            if (left)
                            {
                                rigidbody.AddForce(new Vector2(-shunt, 0), ForceMode2D.Impulse);
                            }
                            else
                            {
                                rigidbody.AddForce(new Vector2(shunt, 0), ForceMode2D.Impulse);
                            }

                        }
                        else
                        {
                            vertical = false;

                            if (left)
                            {
                                rigidbody.AddForce(jumpForceLeft * Mathf.Clamp( heldtime, 0.0f, maxHold), ForceMode2D.Impulse);
                                rigidbody.angularVelocity = rotationForceLeft * Mathf.Clamp(heldtime, 0.0f, maxHold);
                            }
                            else
                            {
                                rigidbody.AddForce(jumpForce * Mathf.Clamp(heldtime, 0.0f, maxHold), ForceMode2D.Impulse);
                                rigidbody.angularVelocity = rotationForce * Mathf.Clamp(heldtime, 0.0f, maxHold);
                            }
                            
                        }

                    }
                    Debug.Log("release");
                    
                }
                collider.offset = flatOffset;
                collider.size = flatSize;
                heldtime = 0.0f;
            }
        }
    }
}
