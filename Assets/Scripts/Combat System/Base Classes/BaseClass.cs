using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass {

    public string theName;

    public float baseHP=100;
    public float currentHP=100;

    public float baseMP=50;
    public float currentMP=50;

    public int baseATK=10;
    public int curATK=10;
    public int baseDEF=10;
    public int curDEF=10;

    public List<BaseAttack> attacks = new List<BaseAttack>();
}
