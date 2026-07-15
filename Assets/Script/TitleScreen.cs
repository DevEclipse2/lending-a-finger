using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;
using static Move;
using Color = UnityEngine.Color;

public class TitleScreen : MonoBehaviour
{
    static byte difficulty = 0;

    bool cancel;
    bool jumpLF;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool input = false;
    float heldTime;
    [SerializeField]
    float holdThresh;
    [SerializeField]
    float cancelThresh;
    [SerializeField]
    float maxHold;
    public GameObject bar;
    Vector2 size;
    public Image barImage;
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

    void Start()
    {
        
    }
    public void onJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            input = true;
        }
        if (context.canceled)
        {
            input = false;

        }
    }

    //3 levels of difficulty
    //easy - gets a single recovery point
    //medium - multiple area checkpoints, where you get teleported to the closest/ last area
    //hard - get reset to points around 10 minutes ago
    //true - morst brutal difficulty

    //stars
    //floor is lava
    //heads over heels - jump only with one direction
    //tornado - more spin!
    //acrobat - more spin multiplier!
    //twichy nerve - auto jump at the end of the charge , no cancelling 
    //flipmania keep up a flip streak (or you die)

    // Update is called once per frame
    void UpdateBar(bool didjump)
    {
        switch (currentState)
        {
            case ChargeState.Idle:
                targetColor = normalColor;

                break;

            case ChargeState.Growing:

                barImage.color = Color.Lerp(normalColor, maxColor, currentCharge / maxHold);
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
                barImage.color = Color.Lerp(maxColor, overheatColor, currentCharge - maxHold / cancelThresh - maxHold);

                if (heldTime >= cancelThresh)
                {
                    currentState = ChargeState.Receding;
                }
                break;

            case ChargeState.Receding:
                // Drain based on duration
               
                currentCharge -= Time.deltaTime * drainSpeed;
                barImage.color = overheatColor;
                if (currentCharge <= 0f)
                {
                    currentCharge = 0f;
                    currentState = ChargeState.Idle;
                }

                break;
        }
    }


    void Update()
    {
        if (heldTime > holdThresh)
        {

        }
        if (input)
        {
            if (!jumpLF)
            {
                currentState = ChargeState.Growing;
                jumpLF = true;
                isOverheated = false;
            }

            heldTime += Time.deltaTime;
            if (!isOverheated)
            {
                currentCharge = heldTime;
            }
            else
            {

            }
            if (currentCharge > cancelThresh)
            {
                isOverheated = true;
            }
            if (barImage != null)
            {
                bar.transform.localScale = new Vector2(size.x, size.y * Mathf.Clamp(currentCharge, 0, maxHold) / maxHold);
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

                if (heldTime < holdThresh)
                {
                    //tap
                }
                else
                {
                    //hold
                    // if it is overheated do nothing

                    //if it is not yet charged also do nothing
                }

            }
        }
    }
}


//selection scheme
//press any button to cycle through
//hold to proceed