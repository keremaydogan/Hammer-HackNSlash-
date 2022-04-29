using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Inputs
{
    public float zInput, xInput;
    public bool runInput;
    public float jumpInput;

    // Manual
    public bool restart;
}


public abstract class Intelligence
{
    //ForAll
    public Inputs inputs;

    public Vector3 moveDir = Vector3.zero;

    public HashSet<Character> targets = new HashSet<Character>();

    public abstract void Awake();
    public abstract void Update();
    public abstract void FixedUpdate();

    public void AddTarget(Character target)
    {
        targets.Add(target);
    }

    public void RemoveTarget(Character target)
    {
        targets.Remove(target);
    }

}
