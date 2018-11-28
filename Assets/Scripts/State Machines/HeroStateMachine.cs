using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;
    public BaseHero hero;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    //for the ProgressBar
    private float cur_cooldown = 0f;
    private float max_cooldown = 5f;
    public Image CooldownBar;
    public GameObject Selector;
    //IENumerator
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 12f;
    //dead
    private bool alive = true;
    //heroPanel
    private HeroPanelStats stats;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

	void Start ()
    {
        
        //find spacer 
        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.FindChild("HeroPanel").FindChild("HeroPanelSpacer");
        //create panel, fill in info
        CreateHeroPanel();
        //cur_cooldown = Random.Range(0, 2.5f); instead od 2.5f you can use luck stat
        startPosition = transform.position;
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING;
    }
	
	void Update ()
    {
        Debug.Log("Hero currentState =" + currentState);
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeCooldownBar();
                break;
            case (TurnState.ADDTOLIST):
                BSM.HeroesToManage.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                //idle
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
                    //change tag
                    this.gameObject.tag = "DeadHero";
                    //not attackable enemy
                    BSM.HeroesInBattle.Remove(this.gameObject);
                    //not managable
                    BSM.HeroesToManage.Remove(this.gameObject);
                    //deactivate the selector
                    Selector.SetActive(false);
                    //reset gui
                    BSM.AttackPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    //remove item from performlist
                    if (BSM.HeroesInBattle.Count > 0)
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
                                    BSM.PerformList[i].AttackersTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];
                                }
                            }
                        }
                    }
                    //change color /play dead animation
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //reset heroinput
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                    
                }
                break;
        }
	}

    void UpgradeCooldownBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;
        float calc_cooldown = cur_cooldown / max_cooldown;
        CooldownBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0 , 0.96f ),CooldownBar.transform.localScale.y, CooldownBar.transform.localScale.z);
        if(cur_cooldown >= max_cooldown){
            currentState = TurnState.ADDTOLIST;
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
        Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x, EnemyToAttack.transform.position.y - 1.5f, EnemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition)) { yield return null; }

        //wait
        yield return new WaitForSeconds(0.5f);
        //do damage
        DoDamage();
        //animate to start position
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition)) { yield return null; }
        //remove this performer from list in BSM - no 2 moves in 1 turn
        BSM.PerformList.RemoveAt(0);
        //reset BSM->wait 
        if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
        {
            BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
            //reset this enemy state
            cur_cooldown = 0f;
            currentState = TurnState.PROCESSING;
        }
        else
        {
            currentState = TurnState.WAITING;
        }
        
        //end coroutine
        actionStarted = false;
        
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float getDamageAmount)
    {
        hero.curHP -= getDamageAmount;
        if(hero.curHP <= 0)
        {
            hero.curHP = 0;
            currentState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }
    //do damage
    void DoDamage()
    {
        float calc_damage = hero.curATK + BSM.PerformList[0].choosenAttack.attackDamage;
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }

    //create a hero panel
    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroName.text = hero.theName;
        stats.HeroHP.text = "HP: \n" + hero.curHP + "/"+ hero.baseHP;
        stats.HeroMP.text = "MP: \n" + hero.curMP + "/" + hero.baseMP;

        CooldownBar = stats.CooldownBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);

    }
    //update stats on damage or heal
    void UpdateHeroPanel()
    {
        stats.HeroHP.text = "HP: \n" + hero.curHP + "/" + hero.baseHP;
        stats.HeroMP.text = "MP: \n" + hero.curMP + "/" + hero.baseMP;
    }
}
