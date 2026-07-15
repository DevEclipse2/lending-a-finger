using TMPro; // Used to update the UI text on the button
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindUI : MonoBehaviour
{
    public InputActionReference actionToRebind;

    public TextMeshProUGUI bindingText;
    public TextMeshProUGUI bindinghint;
    string hintoriginal = "choose your key for this session!";
    string hintError = "Input Disrupted! \n try again";

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    bool rebinding;

    bool jumpTrig;
    bool filterTrig;

    [SerializeField]
    int presscount = 0;

    bool pressed;
    bool pressedLF;
    [SerializeField]
    float   holdThresh;
    float   holdTimer;
    bool    holding;

    [SerializeField]
    Animator animator;

    public bool[] IsHoldInputTest; 
    private void Start()
    {
        // Update the UI with the default or current binding on startup
        UpdateUI();
        bindingText.text = "?";
        rebinding = true;
        StartRebinding();
    }
    public void onJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pressed = true;
        }
        if (context.canceled)
        {
            pressed = false;

        }
        jumpTrig = true;
    }
    public void Filter(InputAction.CallbackContext context)
    {
        Debug.Log("filter");
        filterTrig = true;
    }
    /// <summary>
    /// Call this from your UI Button's OnClick() event
    /// </summary>
    public void StartRebinding()
    {
        actionToRebind.action.Disable();

        bindingText.text = "?";

        rebindingOperation = actionToRebind.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<Keyboard>/escape")
            .OnComplete(operation => FinishRebinding())
            .OnCancel(operation => FinishRebinding())
            .Start();
    }

    private void FinishRebinding()
    {
        rebindingOperation.Dispose();
        actionToRebind.action.Enable();

        UpdateUI();
    }

    private void UpdateUI()
    {
        int bindingIndex = actionToRebind.action.GetBindingIndexForControl(actionToRebind.action.controls[0]);

        bindingText.text = InputControlPath.ToHumanReadableString(
            actionToRebind.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
    void testFailed()
    {
        filterTrig = false;
        jumpTrig = false;
        presscount = 0;
        StartRebinding();
        Debug.Log("interrupted!");
    }

    void MinigameFailed()
    {
        presscount = 0;

        StartRebinding();
        Debug.Log("lost minigame!");
    }

    void TestInputs(bool hold)
    {
        
        if (hold != IsHoldInputTest[presscount]) { 
            
            MinigameFailed();
        
        }
        presscount++;
        if(presscount == IsHoldInputTest.Length)
        {
            Debug.Log("MiniGame Won!");
            animator.SetBool("Proceed", true);
        }
    }

    private void Update()
    {
        if (pressed)
        {
            if (!pressedLF)
            {
                pressedLF = true;
            }

            holdTimer += Time.deltaTime;
            if(holdTimer > holdThresh)
            {
                holding = true;
            }
        }
        else
        {
            if (pressedLF)
            {
                holdTimer = 0;

                pressedLF = false;
                TestInputs(holding);
                holding = false;
            }
           
        }
        if(filterTrig && !jumpTrig && presscount < 5)
        {
            testFailed();
            
        }
    }
}