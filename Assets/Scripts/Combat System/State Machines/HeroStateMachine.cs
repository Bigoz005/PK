using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;
    //public Player heroMain;
    public BaseHero heroBattle;
    public StatsHandler statContainer;
    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    //CooldownBar
    private float cur_cooldown = 0f;
    private float max_cooldown = 5f;
    public Image CooldownBar;
    public GameObject Selector;
    //IENumerator
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    public bool itemUsage = false;
    private Vector2 startPosition;
    private float animSpeed = 12f;
    //dead
    private bool alive = true;
    //heroPanel
    private HeroPanelStats stats;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

	void Start ()
    {
        statContainer = GameObject.Find("StatsContainer").GetComponent<StatsHandler>();


        heroBattle.currentHP = statContainer.currentHP;
        heroBattle.currentMP = statContainer.currentMP;
        heroBattle.curATK = statContainer.curATK;
        heroBattle.curDEF = statContainer.curDEF;
        heroBattle.stamina = statContainer.stamina;
        heroBattle.agility = statContainer.agility;
        heroBattle.dexterity = statContainer.dexterity;
        heroBattle.intellect = statContainer.intellect;
        heroBattle.gold = statContainer.gold;
        heroBattle.xp = statContainer.exp;
        heroBattle.level = statContainer.level;
        //znajdz spacer 
        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").Find("HeroPanelSpacer");
        //stworz panel, uzupelnij panel
        CreateHeroPanel();
        //cur_cooldown = Random.Range(0, 2.5f); instead od 2.5f you can use luck stat
        startPosition = transform.position;
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING;
    }
	//oblsuga stanu hero
	void Update ()
    {
        Debug.Log("Hero currentState =" + currentState);
        UpdateStats();
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
                    //zmien tag
                    this.gameObject.tag = "DeadHero";
                    //usuniecie z heroesinbattle zeby nie bili martwego
                    BSM.HeroesInBattle.Remove(this.gameObject);
                    //nie do obslugiwania
                    BSM.HeroesToManage.Remove(this.gameObject);
                    //ukrycie selectora
                    Selector.SetActive(false);
                    //reset gui
                    BSM.AttackPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    //usuniecie z performlist
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
                    //zmien kolor /animacja smierci
                    //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //reset heroinput
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                    
                }
                break;
        }
    }
    //ladowania paska, gdy ma 100% gracz moze wykonac akcje
    void UpgradeCooldownBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;
        float calc_cooldown = cur_cooldown / max_cooldown;
        CooldownBar.transform.localScale = new Vector2(Mathf.Clamp(calc_cooldown, 0 , 0.96f ),CooldownBar.transform.localScale.y);
        if(cur_cooldown >= max_cooldown){
            currentState = TurnState.ADDTOLIST;
        }
    }
    //Wykonywanie akcji przez postac
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        /*if (itemUsage)
        {
            //wait
            yield return new WaitForSeconds(0.5f);
            //do damage
            UseItem();
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
            itemUsage = false;
            actionStarted = false;
        }
        else*/
        {

            //przesuniecie hero do przeciwnika
            Vector2 enemyPosition = new Vector2(EnemyToAttack.transform.position.x, EnemyToAttack.transform.position.y - 2.0f);
            while (MoveTowardsEnemy(enemyPosition)) { yield return null; }

            //wait
            yield return new WaitForSeconds(0.5f);
            
            DoDamage();
            //UseItem();
            //przesuniecie na start
            Vector2 firstPosition = startPosition;
            while (MoveTowardsStart(firstPosition)) { yield return null; }
            //usuniecie z PerformList aby nie bylo 2 czynnosci w 1 turze
            BSM.PerformList.RemoveAt(0);
            //reset BSM->wait 
            if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
            {
                BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
                //reset stanu hero
                cur_cooldown = 0f;
                currentState = TurnState.PROCESSING;
            }
            else
            {
                currentState = TurnState.WAITING;
            }

            //end coroutine
            itemUsage = false;
            actionStarted = false;
        }       
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
        //heroMain.MyHealth.MyCurrentValue -= getDamageAmount;
        heroBattle.currentHP -= getDamageAmount;
        if(heroBattle.currentHP <= 0)
        {
            heroBattle.currentHP = 0;
            currentState = TurnState.DEAD;
        }
        UpdateHeroPanel();
       
    }

    //use heal potion
    /* void UseItem()
     {
         hero.curHP = hero.curHP + BSM.PerformList[0].choosenItem.HPToAdd;
     }
     */
    //mozna dodac typ ataku jak starczy czasu i na tej podstawie jezeli potezny magiczny to - 30, zwykly -15, slaby -10, na razie na sztywno dla danych 3 atakow
    void IsMagic()
    {
        if (BSM.PerformList[0].choosenAttack.attackName == "Ice Spell1" || BSM.PerformList[0].choosenAttack.attackName == "Poison 1")
        {
            heroBattle.currentMP -= 10;
        }
        if (BSM.PerformList[0].choosenAttack.attackName == "Fire 1")
        {
            heroBattle.currentMP -= 20;
        }
    }
   
    void DoDamage()
    {
        IsMagic();
        float calc_damage = heroBattle.curATK + BSM.PerformList[0].choosenAttack.attackDamage;
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);

        UpdateHeroPanel();
    }
   
    /*void UpdateGold()
    {
        heroMain.MyGold.MyCurrentValue += 20;
    }*/
    //create a hero panel
    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroName.text = heroBattle.theName;
        stats.HeroHP.text = "HP: \n" + heroBattle.currentHP + "/"+ heroBattle.baseHP;
        stats.HeroMP.text = "MP: \n" + heroBattle.currentMP + "/" + heroBattle.baseMP;

        CooldownBar = stats.CooldownBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);

    }
    //update info o dmg lub leczeniu
    void UpdateHeroPanel()
    {
        stats.HeroHP.text = "HP: \n" + heroBattle.currentHP + "/" + heroBattle.baseHP;
        stats.HeroMP.text = "MP: \n" + heroBattle.currentMP + "/" + heroBattle.baseMP;
    }

    void UpdateStats()
    {
        statContainer.currentHP = heroBattle.currentHP;
        statContainer.currentMP = heroBattle.currentMP;
        statContainer.curATK = heroBattle.curATK;
        statContainer.curDEF = heroBattle.curDEF;
        statContainer.stamina = heroBattle.stamina;
        statContainer.agility = heroBattle.agility;
        statContainer.dexterity = heroBattle.dexterity;
        statContainer.intellect = heroBattle.intellect;
        statContainer.gold = heroBattle.gold;
        statContainer.exp = heroBattle.xp;
        statContainer.level = heroBattle.level;
    }
}
