using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass
{
    public enum Type
    {
        ROCK,
        FIRE,
        WATER
    }

    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE
    }

    public Type enemyType;
    public Rarity rarity;

}
