using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour {

    public BaseAttack AttackToPerform;

    public void CastMagicAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input4(AttackToPerform);
    }

    public void DoMeleeAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input7(AttackToPerform);
    }
}
