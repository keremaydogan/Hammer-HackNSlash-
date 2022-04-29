using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatStatus
{
    Still = 1,
    Alert = 2,
    Attack = 3,
}

public class CombatModule : MonoBehaviour
{

    CombatStatus combatStat;
    public CombatStatus comStat => combatStat;

}
