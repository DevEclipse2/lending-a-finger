using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    public Vector2 flatSize;
    [SerializeField]
    public Vector2 ScrunchSize;
    [SerializeField]
    public Vector2 flatOffset;
    [SerializeField]
    public Vector2 ScrunchOffset;

    [SerializeField]
    float maxHold = 3.0f;
    [SerializeField]
    float shunt = 13.0f;

    [SerializeField]
    float jumpForce;
    [SerializeField]
    float rotationForce;

    public float multiplier = 2.1f;
    public bool scrunch = false;
    bool increaseAngular = false;


    public GameObject bar;
    Vector2 size;
    public Image barImage;
    public float overheatThreshold = 4.0f;



    public float drainSpeed = 3.0f;

    public Color normalColor = Color.green;
    public Color maxColor = new Color(1f, 0.5f, 0f); // Orange
    public Color overheatColor = Color.red;

    public Color targetColor;
    public Color currentColor;
    private float currentCharge = 0f;
    private bool isOverheated = false;
    private ChargeState currentState = ChargeState.Idle;

    public enum ChargeState { Idle, Growing, Holding, Receding }

   
    public float colorBlendSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        size = bar.transform.localScale;
        targetColor = normalColor;
        currentColor = normalColor;
        if (barImage != null) barImage.color = currentColor;
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

    void UpdateBar(bool didjump)
    {
        switch (currentState)
        {
            case ChargeState.Idle:
                targetColor = normalColor;

                break;

            case ChargeState.Growing:

                barImage.color = Color.Lerp(normalColor, maxColor, currentCharge);
                if (didjump)
                {
                    currentState = ChargeState.Receding;
                }
                // Did we reach the top
                else if (currentCharge >= maxHold)
                {
                    currentCharge = maxHold;
                    currentState = ChargeState.Holding;
                }
                break;

            case ChargeState.Holding:
                barImage.color = maxColor; // Lock target to Orange

                if (heldtime >= overheatThreshold)
                {
                    currentState = ChargeState.Receding;
                }
                else if (didjump)
                {
                    currentState = ChargeState.Receding;
                }
                break;

            case ChargeState.Receding:
                // Drain based on duration
                if(grounded)
                {
                    currentCharge -= Time.deltaTime * drainSpeed;
                    barImage.color = overheatColor;
                    if (currentCharge <= 0f)
                    {
                        currentCharge = 0f;
                        currentState = ChargeState.Idle;
                    }
                }
                
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool didjump = false;
        if (heldtime > holdThresh)
        {
            
            scrunch = true;
            if (grounded)
            {
                rigidbody.angularVelocity = 0;
            }
        }
        if (scrunch)
        {
            collider.size = ScrunchSize;
            collider.offset = ScrunchOffset;
            if (!grounded && !increaseAngular) {
                rigidbody.angularVelocity *= multiplier;
                increaseAngular = true;

            }
        }
        else
        {
            collider.offset = flatOffset;
            collider.size = flatSize;
            if (!grounded && increaseAngular)
            {
                rigidbody.angularVelocity /= multiplier;
                increaseAngular = false;

            }
        }
        if (jump)
        {
            if (!jumpLF)
            {
                currentState = ChargeState.Growing;
                jumpLF = true;
                isOverheated = false;
            }

            heldtime += Time.deltaTime;
            if (!isOverheated)
            {
                currentCharge = heldtime;
            }
            else
            {
                
            }
            if(currentCharge > overheatThreshold)
            {
                isOverheated = true;
            }
            if (barImage != null)
            {
                bar.transform.localScale = new Vector2(size.x, size.y * Mathf.Clamp( currentCharge, 0, maxHold) / maxHold);
            }
        }
        else
        {
            if (jumpLF)
            {
                jumpLF = false;
                //do heldtime threshes here
                //maybe o.2 seconds is a tap
                //etc etc 
                scrunch = false;

                if (heldtime < holdThresh)
                {
                    Debug.Log("tap");
                    if (grounded)
                    {
                        rigidbody.linearVelocity = Vector2.zero;
                        rigidbody.angularVelocity += 30.0f;
                        RaycastHit2D ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 3.0f);
                        //if(ray.collider.gameObject.CompareTag("Ground"))
                        //{
                        //    transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z + 20));
                        //}
                        //else
                        //{
                        //    transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z - 20));
                        //}
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z + 20));

                    }
                }
                else
                {
                    c
                        Debug.Log("Jump");
                        rigidbody.AddForce(transform.up * jumpForce * Mathf.Clamp(currentCharge + 0.4f, 0.0f, maxHold), ForceMode2D.Impulse);
                        rigidbody.angularVelocity = rotationForce * Mathf.Clamp(currentCharge, 0.0f, maxHold);
                        didjump = true;

                    }
                    Debug.Log("release");

                }
                
                heldtime = 0.0f;
                currentCharge = 0.0f;
                bar.transform.localScale = new Vector2(size.x, size.y * currentCharge / maxHold);

            }
        }
        UpdateBar(didjump);

    }
}
