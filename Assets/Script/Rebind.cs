using TMPro; // Used to update the UI text on the button
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
    public InputActionReference actionToRebind;

    public TextMeshProUGUI bindingText;
    public TextMeshProUGUI bindinghint;
    string hintoriginal = "choose your key for this session!";
    string hintError = "Input Disrupted! \n try again";

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    bool rebinding;


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

    public Animator[] promptAnimators;
    public Image[] promptImages;
    public Color EmptyColor;
    public Color FullColor;

    bool filter = false;


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
        Debug.Log("jump");

    }
    public void Filter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            filter = true;
        }
        if (context.canceled)
        {
            filter = false;
        }
        Debug.Log("filter");
    }
    /// <summary>
    /// Call this from your UI Button's OnClick() event
    /// </summary>
    public void StartRebinding()
    {
        foreach (Image image in promptImages)
        {
            image.color = EmptyColor;
        }
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
        presscount = 0;
        rebindingOperation.Cancel();  // Stop listening for inputs
        rebindingOperation.Dispose(); // Free the unmanaged memory (NativeArray)
        rebindingOperation = null;
        StartRebinding();
        foreach (Animator animate in promptAnimators)
        {
            animate.SetBool("Failed", true);

        }
        foreach (Image image in promptImages)
        {
            image.color = EmptyColor;
        }
        presscount = 0;
        Debug.Log("interrupted!");
    }

    void MinigameFailed()
    {
        foreach (Image image in promptImages)
        {
            image.color = EmptyColor;
        }
        presscount = 0;
        Debug.Log("lost minigame!");
    }

    void TestInputs(bool hold)
    {

        if (presscount == IsHoldInputTest.Length) {
            Debug.Log("MiniGame Won!");
            animator.SetBool("Proceed", true); return; }
        
        if (hold != IsHoldInputTest[presscount]) {
            if (presscount == 0) {
                promptAnimators[presscount].SetBool("Failed", true);
                MinigameFailed();
                return;
            }
            promptAnimators[presscount -1].SetBool("Failed", true);
            MinigameFailed();
            return;
        }
        presscount++;
        promptImages[presscount -1].color = FullColor;
        promptAnimators[presscount - 1].SetBool("Triggered", true);

        if (presscount == IsHoldInputTest.Length)
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
        if(filter && !pressed && presscount < 5)
        {
            testFailed();
            
        }
    }
}