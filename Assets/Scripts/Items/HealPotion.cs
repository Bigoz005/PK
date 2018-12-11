using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPotion : BaseItem {

    public HealPotion()
    {
        itemName = "Heal Potion";
        itemDescription = "Heal Potion Gives You 50HP";
        HPToAdd = 50f;
        MPToAdd = 50f;
        StrenghtToAdd = 0f;
        AgilityToAdd = 0f;
    }

}
