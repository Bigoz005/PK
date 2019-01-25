using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour {

    public BaseAttack AttackToPerform;
    //public BaseItem ItemToPerform;

    public void CastMagicAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input4(AttackToPerform);
    }

    public void DoMeleeAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input7(AttackToPerform);
    }
    /*
    public void UseItem()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input1(ItemToPerform);
    }
    */
}
