using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass {
    public string theName;

    public float baseHP;
    public float curHP;

    public float baseMP;
    public float curMP;

    public int baseATK;
    public int curATK;
    public int baseDEF;
    public int curDEF;

    public List<BaseAttack> attacks = new List<BaseAttack>();
}
