using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class HandleTurns{

    public string Attacker; //attacker's name
    public string Type; 
    public GameObject AttackersGameObject; //attacks
    public GameObject AttackersTarget; //to be attacked

    //which attack is performed
    public BaseAttack choosenAttack;
    public BaseItem choosenItem;
}
