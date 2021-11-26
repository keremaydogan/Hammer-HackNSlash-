using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Intelligence : MonoBehaviour
{

    ControlKeys controlKeys;

    protected Vector3 moveDir = Vector3.zero;
    public Vector3 moveDirect => moveDir;


    public abstract void InputReciever();

}
