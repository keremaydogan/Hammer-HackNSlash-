using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ControlKeys
{
    public KeyCode forward;
    public KeyCode backward;
    public KeyCode right;
    public KeyCode left;
    public KeyCode run;
    public KeyCode jump;
    public KeyCode restart;
}

public class ManualIntelligence : Intelligence
{

    public ControlKeys controlKeys;

    Vector3 startPos;

    //Transform cameraPos; // FOR MAKING MOVEDIR DYNAMIC


    public override void Awake()
    {
        SetKeys();

    }

    public override void Update()
    {
        InputReciever();
        StatesUpdate();

    }

    public override void FixedUpdate()
    { 
    }

    void SetKeys()
    {
        controlKeys.forward = KeyCode.W;
        controlKeys.backward = KeyCode.S;
        controlKeys.right = KeyCode.D;
        controlKeys.left = KeyCode.A;
        controlKeys.run = KeyCode.LeftShift;
        controlKeys.restart = KeyCode.R;
        controlKeys.jump = KeyCode.L;
    }

    void InputReciever() // Keyboard
    {
        if (Input.GetKey(controlKeys.forward)) { inputs.zInput = 1; } else if (Input.GetKey(controlKeys.backward)) { inputs.zInput = -1; } else { inputs.zInput = 0; }

        if (Input.GetKey(controlKeys.right)) { inputs.xInput = 1; } else if (Input.GetKey(controlKeys.left)) { inputs.xInput = -1; } else { inputs.xInput = 0; }

        if (Input.GetKeyDown(controlKeys.jump)) { inputs.jumpInput = 1; } else if (Input.GetKey(controlKeys.jump)) { inputs.jumpInput = 0; } else { inputs.jumpInput = -1; }

        if (Input.GetKey(controlKeys.run)) { inputs.runInput = true; } else { inputs.runInput = false; }

        if (Input.GetKeyDown(controlKeys.restart)) { inputs.restart = true; } else { inputs.restart = false; }
    }

    // IT'S STATIC. NEEDS TO BE DYNAMIC.
    void StatesUpdate() 
    {
        moveDir = Quaternion.Euler(0, 45, 0)*new Vector3(inputs.xInput, 0, inputs.zInput);
        
        
    }

}
