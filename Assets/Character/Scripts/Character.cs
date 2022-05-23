using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : MonoBehaviour
{

    MovementPhysics mp;
    CombatModule cm;

    public int health;

    CharacterInfo charInfo;
    public PhysicalStatus phyStat => mp.phyStat;
    public PhysicalSubStatus phySubStat => mp.phySubStat;
    public CombatStatus combatStat => cm.comStat;
    public Vector3 pos => transform.position;

    private void Awake()
    {
        mp = gameObject.GetComponent<MovementPhysics>();
        cm = gameObject.GetComponent<CombatModule>();
    }

}
