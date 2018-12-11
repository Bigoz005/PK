using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;
    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    //for the ProgressBar
    private float cur_cooldown = 0f;
    private float max_cooldown = 10f;
    public Image CooldownBar;
    //this GameObject
    private Vector3 startPosition;
    public GameObject Selector;
    //time for action 
    private bool actionStarted = false;
    public GameObject HeroToAttack;
    private float animSpeed = 15f;

    //alive
    private bool alive = true;


    void Start () {
        currentState = TurnState.PROCESSING;
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine> ();
        startPosition = transform.position;
	}
	

	void Update () {

        Debug.Log("Enemy currentState =" + currentState);
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeCooldownBar();
            break;
            case (TurnState.CHOOSEACTION):
                if (BSM.HeroesInBattle.Count > 0)
                {
                    ChooseAction();
                }
                currentState = TurnState.WAITING;
            break;
            case (TurnState.WAITING):
                //idle state
            break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
            break;
            case (TurnState.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    //change tag of enemy
                    this.gameObject.tag = "DeadEnemy";
                    //not attackable by heroes
                    BSM.EnemiesInBattle.Remove(this.gameObject);
                    //disable the slector
                    Selector.SetActive(false);
                    //remove all inputs heroattacks
                    if (BSM.EnemiesInBattle.Count > 0)
                    {
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (i != 0)
                            {
                                if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                                {
                                    BSM.PerformList.Remove(BSM.PerformList[i]);
                                }
                                if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                                {
                                    BSM.PerformList[i].AttackersTarget = BSM.EnemiesInBattle[Random.Range(0, BSM.EnemiesInBattle.Count)];
                                }
                            }
                        }

                    }
                    //change the color to gray / play dead animation
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 255);
                    //set alive false
                    alive = false;
                    //reset enemyButtons
                    BSM.EnemyButtons();
                    //check alive
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                }
            break;
        }
    }

    void UpgradeCooldownBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;

        float calc_cooldown = cur_cooldown / max_cooldown;
        CooldownBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 0.94f), CooldownBar.transform.localScale.y, CooldownBar.transform.localScale.z);
        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {
        HandleTurns myAttack = new HandleTurns();
        myAttack.Attacker = enemy.theName;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        {
            myAttack.AttackersTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];
            int num = Random.Range(0, enemy.attacks.Count);
            myAttack.choosenAttack = enemy.attacks[num];
            Debug.Log(this.gameObject.name + " has choosen " + myAttack.choosenAttack.attackName + " and do " + myAttack.choosenAttack.attackDamage + " damage");

            BSM.CollectActions(myAttack);
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate enemy near hero
        Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x, HeroToAttack.transform.position.y + 1.5f, HeroToAttack.transform.position.z);
        while (MoveTowardsEnemy(heroPosition)){ yield return null;}

        //wait
        yield return new WaitForSeconds(0.5f);
        //do damage
        DoDamage();
        //animate to start position
        Vector3 firstPosition = startPosition;
        while(MoveTowardsStart(firstPosition)) { yield return null; }
        //remove this performer from list in BSM - no 2 actions in 1 turn
        BSM.PerformList.RemoveAt(0);
        //reset BSM->wait 
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        actionStarted = false;
        //reset this enemy state
        cur_cooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DoDamage()
    {
        float calc_damage = enemy.curATK + BSM.PerformList[0].choosenAttack.attackDamage;
        HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calc_damage);
    }

    public void TakeDamage(float getDamageAmount)
    {
        enemy.curHP -= getDamageAmount;
        if(enemy.curHP <= 0)
        {
            enemy.curHP = 0;
            currentState = TurnState.DEAD;
        }
    }

}
