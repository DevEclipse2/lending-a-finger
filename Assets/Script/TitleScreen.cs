using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool input = false;
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
    void Update()
    {
        
    }
}
