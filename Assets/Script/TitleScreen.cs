using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Move;
using Color = UnityEngine.Color;

public class TitleScreen : MonoBehaviour
{
    static byte difficulty = 0;
    byte maxdifficulty = 4;
    bool cancel;
    bool jumpLF;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
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

    bool doTutorial;
        
    //make players skip by typing out skip in morse 
    // ... _._ .. .__.


    //this is your key
    //there are many like it
    //but this is yours
    //pick a key by tapping it twice, holding it twice

    //many things can be done with one button
    //you can tap it .. 
    //hold it _
    //yes keep holding
    //goodboy
    //holding it too much will cancel an input
    // and holding it even longer brings up a multi function menu
    //what's the matter, can't hold it?
    //good boy
    // bring up the multi function menu
    //the multi function menu contains gameplay functions, as well as the transcoder
    //to pause and exit , you must type it out in morse code
    //a lookup sheet is provided for easier difficulties

    //spamming is a very important thing
    //it is the second best thing to skill
    //press the key as fast as you can

    //this is your finger
    //there are at most 9 others
    //apparently it has left you
    //like my wife and kids

    // select your main key for this session
    // . _ .. _ 


    void Start()
    {
        size = bar.transform.localScale;
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
    //running key (key changes under circumstance)

    // Update is called once per frame

    //bloody mess : keep jumping or bleed out
    //lava is rising
    //action movie : slow down during spin
    // pent up : max hold time is dramatically reduced
    void UpdateBar(bool didjump)
    {
        switch (currentState)
        {
            case ChargeState.Idle:
                targetColor = normalColor;
                break;

            case ChargeState.Growing:
                barImage.color = Color.Lerp(normalColor, maxColor, currentCharge / maxHold);

                // Did we reach the top
                if (currentCharge >= maxHold)
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

    void AddDifficulty() { }
    void ContinueGame() { }
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
                bar.transform.localScale = new Vector2(size.x * Mathf.Clamp(currentCharge, 0, maxHold) / maxHold, size.y );
            }
        }
        else
        {
            heldTime = 0;
            if (jumpLF)
            {
                jumpLF = false;
                //do heldtime threshes here
                //maybe o.2 seconds is a tap
                //etc etc 

                if (heldTime < holdThresh)
                {
                    //tap
                    difficulty++;
                    if (difficulty > maxdifficulty)
                    {
                        difficulty -= maxdifficulty;
                    }
                }
                else
                {
                    //hold
                    // if it is overheated do nothing

                    //if it is not yet charged also do nothing
                    if (isOverheated || currentState == ChargeState.Growing)
                    {
                        Debug.Log("Not accepted");
                    }
                    if (currentState == ChargeState.Holding)
                    {
                        Debug.Log("Let's go!");
                        switch (difficulty)
                        {
                            case 0:
                                Debug.Log("Select Difficulty first!");
                                break;

                            case 1:
                                Debug.Log("play easy");
                                break;
                            case 2:
                                Debug.Log("play medium");
                                break;
                            case 3:
                                Debug.Log("play hard");
                                break;
                            case 4:
                                Debug.Log("play true");
                                break;
                            default: break;
                        }
                    }
                }

            }
        }
        UpdateBar(false);
    }
}


//selection scheme
//press any button to cycle through
//hold to proceed