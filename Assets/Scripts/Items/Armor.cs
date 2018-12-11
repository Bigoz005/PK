using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ArmorType { Helmet, Chest, Trinket, Gloves, Boots, MainHand, OffHand, TwoHand }

[CreateAssetMenu(fileName = "Armor", menuName = "Items/Armor", order = 2)]
public class Armor : Item
{
    [SerializeField]
    private ArmorType armorType;

    [SerializeField]
    private int hp;

    [SerializeField]
    private int energy;

    [SerializeField]
    private int strength;

    [SerializeField]
    private int intellect;

    [SerializeField]
    private int dexterity;

    [SerializeField]
    private string opis;

    internal ArmorType MyArmorType
    {
        get
        {
            return armorType;
        }
    }

    
    public override string GetDescription()
    {
        string stats = string.Empty;

        stats += string.Format("\nType: {0}", armorType);        

        if (hp > 0)
        {
            stats += string.Format("\n+{0} HP", hp); 
        }
        if (energy > 0)
        {
            stats += string.Format("\n+{0} Energy", energy);
        }
        if (strength > 0)
        {
            stats += string.Format("\n+{0} Strength", strength);
        }
        if (intellect > 0)
        {
            stats += string.Format("\n+{0} Intellect", intellect);
        }
        if (dexterity > 0)
        {
            stats += string.Format("\n+{0} Dexterity", dexterity);
        }
        if (opis != null)
        {
            stats += string.Format("\n<i>\n{0}</i>", opis);
        }


        return base.GetDescription() + stats;
    }

    public void Equip()
    {
        CharacterPanel.MyInstance.EquipArmor(this);
    }

}
