using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="HealthPotion",menuName ="Items/Potion",order =1)]
public class HealthPotion : Item, IUseable
{
    [SerializeField]
    private int health; //dla ręcznego ustawiania

    public void Use()
    {
        if(Player.MyInstance.MyHealth.MyCurrentValue < Player.MyInstance.MyHealth.MaxHealthValue)
        { 
        Remove();

        Player.MyInstance.MyHealth.MyCurrentValue += 20;
        }
    }

    public override string GetDescription()
    {
        return base.GetDescription() + string.Format("\nRestores {0} health", health);
    }
}
