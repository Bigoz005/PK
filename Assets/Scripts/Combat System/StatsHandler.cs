using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsHandler : MonoBehaviour {

    public float currentHP = 80;

    public float currentMP = 100;

    public int curATK = 10;
    public int curDEF = 10;

    public int stamina = 10;
    public int intellect = 15;
    public int dexterity = 10;
    public int agility = 10;

    public float exp = 120;
    public int level = 3;

    public int gold = 70;

    public int MyGold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }
}
