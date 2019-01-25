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
	
    //obsluga stanow
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
                    //zmien tag
                    this.gameObject.tag = "DeadEnemy";
                    //usuniecie, zeby nie mozna bylo go atakowac
                    BSM.EnemiesInBattle.Remove(this.gameObject);
                    //ukrycie selectora
                    Selector.SetActive(false);
                    //usuniecie inputow heroattack
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
                    //zmiana koloru na szary / animacja smierci
                    //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 255, 255, 255);
                    this.gameObject.SetActive(false);
                    //martwy
                    alive = false;
                    //reset enemyButtons
                    BSM.EnemyButtons();
                    //sprawdz czy zyje
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                }
            break;
        }
    }

    void UpgradeCooldownBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;

        float calc_cooldown = cur_cooldown / max_cooldown;
        CooldownBar.transform.localScale = new Vector2(Mathf.Clamp(calc_cooldown, 0, 0.94f), CooldownBar.transform.localScale.y);
        if (cur_cooldown >= max_cooldown)
        { 
            currentState = TurnState.CHOOSEACTION;
        }
    }
    
    
    //wybor akcji przez przeciwnika
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
            Debug.Log(this.gameObject.name + " has choosen " + myAttack.choosenAttack.attackName + " and do " + (myAttack.choosenAttack.attackDamage + enemy.curATK)+ " damage");

            BSM.CollectActions(myAttack);
        }
    }
    //wykonanie akcji przeciwnika
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animowanie przeciwnika do bohatera
        Vector2 heroPosition = new Vector2(HeroToAttack.transform.position.x, HeroToAttack.transform.position.y + 2.0f);
        while (MoveTowardsEnemy(heroPosition)){ yield return null;}

        yield return new WaitForSeconds(0.5f);
        //atak
        DoDamage();
        //powrot to startowej pozycji
        Vector2 firstPosition = startPosition;
        while(MoveTowardsStart(firstPosition)) { yield return null; }
        //usuniecie z PerformList zeby nie bylo 2 atakow w jednej turze np
        BSM.PerformList.RemoveAt(0);
        //reset BSM->wait 
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        actionStarted = false;
        //reset stanu przeciwnika
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
        enemy.currentHP -= getDamageAmount;
        if(enemy.currentHP <= 0)
        {
            enemy.currentHP = 0;
            currentState = TurnState.DEAD;
        }
    }

}
