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

public class MovementBasics : MonoBehaviour
{
    Vector3 startPos;
    ControlKeys controlKeys;
    float zInput, xInput;
    bool runInput;
    float jumpInput;

    [HideInInspector] public float zInp => zInput;
    [HideInInspector] public float xInp => xInput;
    [HideInInspector] public bool runInp => runInput;
    [HideInInspector] public float jumpInp => jumpInput;

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
        if (Input.GetKey(controlKeys.forward)) { zInput = 1; } else if (Input.GetKey(controlKeys.backward)) { zInput = -1; } else { zInput = 0; }

        if (Input.GetKey(controlKeys.right)) { xInput = 1; } else if (Input.GetKey(controlKeys.left)) { xInput = -1; } else { xInput = 0; }

        if (Input.GetKeyDown(controlKeys.jump)) { jumpInput = 1; } else if (Input.GetKey(controlKeys.jump)) { jumpInput = 0; } else { jumpInput = -1; }

        if (Input.GetKey(controlKeys.run)) { runInput = true; } else { runInput = false; }
    }

}
