using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uppercut : BaseAttack
{
    public Uppercut()
    {
        attackName = "Uppercut";
        attackDescription = "You uppercut is better than Bruce Lee.";
        attackDamage = 5f;
        attackCost = 0;
    }

}
