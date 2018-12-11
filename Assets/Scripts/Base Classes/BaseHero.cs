using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseClass{
    public int stamina;
    public int intellect;
    public int dexterity;
    public int agility;

    public List<BaseItem> ItemsInBag = new List<BaseItem>();
    public List<BaseAttack> MagicAttacks = new List<BaseAttack>();
}
