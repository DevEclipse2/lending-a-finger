using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MorseParser : MonoBehaviour
{
    public enum Result { Success, Fail, Wait, Idle };
    public Result result = Result.Idle;

    public int ResultIndex;
    public string TargetString { get; private set; } = "";
    public string[] Options {get; private set;}
    private bool pressed;
    private bool pressedLF;

    string inputString = "";
    string parsedString = "";

    float heldTime;
    float holdThresh;
    Dictionary<string, char> LookupTable = new Dictionary<string, char>
    {
        {".-"   ,'a'},
        {"-..." ,'b'},
        {"-.-." ,'c'},
        {"-.."  ,'d'},
        {"."    ,'e'},
        {"..-." ,'f'},
        {"--."  ,'g'},
        {"...." ,'h'},
        {".."   ,'i'},
        {".---" ,'j'},
        {"-.-"  ,'k'},
        {".-.." ,'l'},
        {"--"   ,'m'},
        {"-."   ,'n'},
        {"---"  ,'o'},
        {".--." ,'p'},
        {"--.-" ,'q'},
        {".-."  ,'r'},
        {"..."  ,'s'},
        {"-"    ,'t'},
        {"..-"  ,'u'},
        {"...-" ,'v'},
        {".--"  ,'w'},
        {"-..-" ,'x'},
        {"-.--" ,'y'},
        {"--.." ,'z'},

        {".----",'1'},
        {"..---",'2'},
        {"...--",'3'},
        {"....-",'4'},
        {".....",'5'},
        {"-....",'6'},
        {"--...",'7'},
        {"---..",'8'},
        {"----.",'9'},
        {"-----",'0'},

    };

    float wordCountdown = 0;
    [SerializeField]
    float wordCountdownMax = 0.3f;

    string currentSelectedChar = "";
    public bool Enabled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public int SetString(string[] options)
    {
        if(result == Result.Idle)
        {
            Options = options;
            result = Result.Wait;
            return 0;
        }
        return -1;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(!Enabled) return;
        if (pressed)
        {
            heldTime += Time.deltaTime;
            wordCountdown = 0;
            if (heldTime > holdThresh)
            {
                //show image of dash

            }
            else
            {
                //show image of dot
            }
        }
        else
        {
            heldTime = 0;
            wordCountdown += Time.deltaTime;
            if(wordCountdown > wordCountdownMax)
            {
                wordCountdown = 0;
                //splits character off and parses it
                if(inputString != null && inputString != "")
                {
                    if (LookupTable.ContainsKey(inputString))
                    {
                        parsedString += LookupTable[inputString].ToString();
                    }
                    else
                    {
                        Debug.LogError("Error could not parse");
                    }
                    inputString = "";
                }
            }
            if (pressedLF)
            {
                pressedLF = false;
                if (heldTime < holdThresh)
                {
                    //"."
                    inputString += ".";
                }
                else
                { //"-"
                    inputString += "-";
                }
                if(LookupTable.ContainsKey(inputString))
                {
                    currentSelectedChar = LookupTable[inputString].ToString();
                }
                else
                {
                    currentSelectedChar = "?!";
                }

            }
        }

    }
}
