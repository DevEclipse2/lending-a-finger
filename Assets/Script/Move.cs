using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Move : MonoBehaviour
{
    public bool vertical;
    public bool left;
    public bool grounded;
    public bool jump = false;
    public bool jumpLF = false;
    public float heldtime = 0.0f;

    [SerializeField]
    Transform Tip;
    [SerializeField]
    Transform Tail;


    [SerializeField]
    private float holdThresh = 0.15f;
    [SerializeField]
    private float pauseThresh = 6;
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

    public GameObject TipArrow;
    public GameObject TailArrow;


    public GameObject bar;
    Vector2 size;
    public UnityEngine.UI.Image barImage;
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


    [SerializeField]
    ParticleSystem trail;
    private ParticleSystem.EmissionModule speedEmissionModule;
    [SerializeField]
    SpriteRenderer renderer;
    [SerializeField]
    Sprite tap;
    [SerializeField]
    Sprite relax;
    [SerializeField] Sprite scrunched;
    bool InMenu;

    public GameObject menu;
    public GameObject saveCheckpoint;
    public GameObject checkPoint;
    public GameObject loadOption;
    public Animator[] menuBulletPoints;
    public TextMeshProUGUI[] menuText;
    int maxOptions = 4;
    bool first;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        size = bar.transform.localScale;
        targetColor = normalColor;
        currentColor = normalColor;
        speedEmissionModule = trail.emission;
        if (barImage != null) barImage.color = currentColor;
        if(TitleScreen.difficulty != 1)
        {
            saveCheckpoint.SetActive(false);
            maxOptions = 3;
        }
        if (TitleScreen.difficulty == 3)
        {
            loadOption.SetActive(false);
            maxOptions = 2;
        }
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
    void ShowMenu()
    {
        Options = 0;
        if (TitleScreen.difficulty == 1)
        {
            Options = 3;
        }
        menuText[Options].fontStyle = TMPro.FontStyles.Underline;
        InMenu = true;
        menu.SetActive(true);
        renderer.sprite = relax;
        collider.offset = flatOffset;
        collider.size = flatSize;
        if (!grounded && increaseAngular)
        {
            rigidbody.angularVelocity /= multiplier;
            increaseAngular = false;
        }
        first = false;
    }
    void HideMenu()
    {
        InMenu = false;
        menu.SetActive(false);
    }
    void UpdateBar(bool didjump)
    {
        switch (currentState)
        {
            case ChargeState.Idle:
                targetColor = normalColor;

                break;

            case ChargeState.Growing:

                barImage.color = Color.Lerp(normalColor, maxColor, currentCharge/maxHold);
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
                barImage.color = Color.Lerp(maxColor, overheatColor, currentCharge - maxHold / 1);

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

    byte Options= 0 ;

    // Update is called once per frame
    void Update()
    {
        if (heldtime > pauseThresh)
        {
            ShowMenu();
        }
        if (Tip.position.y >= Tail.position.y)
        { 
            TipArrow.gameObject.SetActive(true);
            TailArrow.gameObject.SetActive(false);
        }
        else
        {
            TipArrow.gameObject.SetActive(false);
            TailArrow.gameObject.SetActive(true);
        }

            float currentSpeed = rigidbody.linearVelocity.magnitude;

        // 2. Calculate what percentage of our "max speed" we are currently traveling at
        float speedPercentage = Mathf.Clamp01(currentSpeed / 18);

        // 3. Set the new emission rate based on that percentage
        speedEmissionModule.rateOverTime = 18 * speedPercentage;
        bool didjump = false;
        if (heldtime > holdThresh)
        {
            scrunch = true;
            if (grounded)
            {
                rigidbody.angularVelocity = 0;
            }
        }
        if (!InMenu)
        {
            if (scrunch)
            {
                renderer.sprite = scrunched;
                collider.size = ScrunchSize;
                collider.offset = ScrunchOffset;
                if (!grounded && !increaseAngular)
                {
                    rigidbody.angularVelocity *= multiplier;
                    increaseAngular = true;

                }
            }
            else
            {
                renderer.sprite = relax;
                collider.offset = flatOffset;
                collider.size = flatSize;
                if (!grounded && increaseAngular)
                {
                    rigidbody.angularVelocity /= multiplier;
                    increaseAngular = false;
                }
            }
        }
        if (jump)
        {
            if (!jumpLF)
            {
                currentState = ChargeState.Growing;
                jumpLF = true;
                isOverheated = false;
                if(!InMenu)
                {
                    renderer.sprite = tap;

                }
            }

            heldtime += Time.deltaTime;
            if (!isOverheated)
            {
                currentCharge = heldtime;
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
                    if (!InMenu)
                    {
                        if (grounded)
                        {
                            rigidbody.linearVelocity = Vector2.zero;
                            rigidbody.angularVelocity += 30.0f;
                            RaycastHit2D ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 3.0f);
                            transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z + 20));

                        }
                    }
                    else
                    {
                        foreach(TextMeshProUGUI text in menuText)
                        {
                            text.fontStyle = TMPro.FontStyles.Normal;
                        }
                        Options++;
                        if (Options == 1 && TitleScreen.difficulty == 3)
                        {
                            Options = 2;
                        }
                        if (Options == maxOptions)
                        {
                            Options = 0;
                        }
                        menuText[Options].fontStyle = TMPro.FontStyles.Underline;

                    }
                }
                else
                {
                    if (!InMenu)
                    {
                        if (grounded)
                        {
                            Debug.Log("Jump");
                            if (Tip.position.y >= Tail.position.y)
                            {
                                rigidbody.AddForce(transform.up * jumpForce * Mathf.Clamp(currentCharge, 0.0f, maxHold), ForceMode2D.Impulse);
                            }
                            else
                            {
                                rigidbody.AddForce(-transform.up * jumpForce * Mathf.Clamp(currentCharge, 0.0f, maxHold), ForceMode2D.Impulse);
                            }

                            rigidbody.angularVelocity = rotationForce * Mathf.Clamp(currentCharge, 0.0f, maxHold);
                            didjump = true;

                        }
                    }
                    else
                    {
                        menuBulletPoints[Options].SetBool("Triggered", true);
                        
                        switch(Options)
                        {
                            case 0:
                                if(!first)
                                {
                                    first = true;
                                    break;
                                }
                                HideMenu();
                                break;
                            case 1:
                                if (TitleScreen.difficulty != 3)
                                {
                                    checkPoint.GetComponent<Checkpoints>().LoadCheckpoint();
                                    HideMenu();
                                }
                                break;
                            case 2:
                                //checkPoint.GetComponent<Checkpoints>().LoadCheckpoint();

                                HideMenu();
                                break;
                            case 3:
                                if(TitleScreen.difficulty == 1)
                                {
                                    if (!first)
                                    {
                                        first = true;
                                        break;
                                    }
                                    checkPoint.GetComponent<Checkpoints>().SetCheckpoint(transform.position);
                                    HideMenu();
                                }
                                break;
                        }
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
