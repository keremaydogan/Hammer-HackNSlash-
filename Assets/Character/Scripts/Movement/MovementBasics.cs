using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum IntelligenceType
{
    manualIntelligence = 0,
    virtualIntelligence = 1,

}

public class MovementBasics : MonoBehaviour
{

    MovementPhysics mp;

    [SerializeField] IntelligenceType intelligenceType;
    Intelligence intl = new ManualIntelligence();

    Vector3 pos => transform.position;

    [HideInInspector] public bool runInp => intl.inputs.runInput;
    [HideInInspector] public float jumpInp => intl.inputs.jumpInput;

    [HideInInspector] public Vector3 moveDirect => intl.moveDir;

    // RESTART

    Vector3 startPos;

    // DETECTION
    [HideInInspector] public string enemy = "";
    HashSet<Character> distantChars = new HashSet<Character>();


    private void Awake()
    {
        mp = gameObject.GetComponent<MovementPhysics>();

        EnemyTagSelector();
        IntelSelecter();

        intl.Awake();

        // RESTART
        startPos = transform.position;

    }

    private void Update()
    {
        intl.Update();

        Restart();
    }

    private void FixedUpdate()
    {
        intl.FixedUpdate();

        CheckTargetDistance();
    }

    void IntelSelecter()
    {
        switch (intelligenceType){
            case IntelligenceType.manualIntelligence:
                intl = new ManualIntelligence();
                break;
            case IntelligenceType.virtualIntelligence:
                intl = new VirtualIntelligence();
                break;
        }
    }

    void EnemyTagSelector()
    {
        switch (gameObject.tag)
        {
            case "Enemy":
                enemy = "Ally";
                break;
            case "Ally":
                enemy = "Enemy";
                break;
        }
    }

    public void AddTarget(Character target)
    {
        intl.AddTarget(target);
    }

    public void RemoveTarget(Character target)
    {
        intl.RemoveTarget(target);
    }

    public void CheckTargetDistance()
    {

        if(intl.targets.Count > 0) {
            distantChars.Clear();
            foreach (Character target in intl.targets) {
                if (Mathf.Abs((target.pos - pos).magnitude) > mp.enemySeeDistance) {
                    distantChars.Add(target);
                }
            }

            if(distantChars.Count > 0) // to prevent: "InvalidOperationException: Collection was modified; enumeration operation may not execute."
            {
                foreach(Character target in distantChars)
                {
                    intl.targets.Remove(target);
                }
            }
        }
    }

    // RESTART
    void Restart()
    {
        if ( intl.inputs.restart || transform.position.y < -10){
            transform.position = startPos;
        }
    }


}
