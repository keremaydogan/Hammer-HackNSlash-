using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct ControlKeys
{
    public KeyCode forward;
    public KeyCode backward;
    public KeyCode right;
    public KeyCode left;
    public KeyCode run;
    public KeyCode jump;
    public KeyCode restart;
}

struct Inputs
{
    public float zInput, xInput;
    public bool runInput;
    public float jumpInput;
}


public class MovementBasics : MonoBehaviour
{
    Intelligence intelligence;
    ControlKeys controlKeys;
    Inputs inputs;

    Vector3 startPos;

    [HideInInspector] public float zInp => inputs.zInput;
    [HideInInspector] public float xInp => inputs.xInput;
    [HideInInspector] public bool runInp => inputs.runInput;
    [HideInInspector] public float jumpInp => inputs.jumpInput;

    Vector3 moveDir = Vector3.zero;
    [HideInInspector] public Vector3 moveDirect => moveDir;

    private void Awake()
    {
        startPos = transform.position;

        controlKeys.forward = KeyCode.W;
        controlKeys.backward = KeyCode.S;
        controlKeys.right = KeyCode.D;
        controlKeys.left = KeyCode.A;
        controlKeys.run = KeyCode.LeftShift;
        controlKeys.restart = KeyCode.R;
        controlKeys.jump = KeyCode.Keypad6;

        intelligence = new ManualIntelligence();
    }

    private void Update()
    {
        InputReciever();
        StatesUpdate();

        Restart();
    }

    void Restart()
    {
        if (Input.GetKeyDown(controlKeys.restart) || transform.position.y < -10)
        {
            transform.position = startPos;
        }
    }

    void StatesUpdate()
    {
        moveDir = new Vector3(xInp, 0, zInp).normalized;
    }

    void InputReciever() // Keyboard
    {
        if (Input.GetKey(controlKeys.forward)) { inputs.zInput = 1; } else if (Input.GetKey(controlKeys.backward)) { inputs.zInput = -1; } else { inputs.zInput = 0; }

        if (Input.GetKey(controlKeys.right)) { inputs.xInput = 1; } else if (Input.GetKey(controlKeys.left)) { inputs.xInput = -1; } else { inputs.xInput = 0; }

        if (Input.GetKeyDown(controlKeys.jump)) { inputs.jumpInput = 1; } else if (Input.GetKey(controlKeys.jump)) { inputs.jumpInput = 0; } else { inputs.jumpInput = -1; }

        if (Input.GetKey(controlKeys.run)) { inputs.runInput = true; } else { inputs.runInput = false; }
    }

}
