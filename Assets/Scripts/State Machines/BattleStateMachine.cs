using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleStateMachine : MonoBehaviour {

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public PerformAction battleStates;

    public List<HandleTurns> PerformList = new List<HandleTurns> ();
    public List<GameObject> HeroesInBattle = new List<GameObject> ();
    public List<GameObject> EnemiesInBattle = new List<GameObject> ();

    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1, //basic attack
        INPUT2, //selectig enemy
        DONE
    }

    public HeroGUI heroInput;

    public List<GameObject> HeroesToManage = new List<GameObject> ();
    private HandleTurns HeroChoice;

    public GameObject enemyButton;
    public Transform Spacer;

    public GameObject AttackPanel;
    public GameObject ItemSelectPanel;
    public GameObject AttackSelectPanel;
    public GameObject EnemySelectPanel;
    public GameObject MagicPanel;

    // attack
    public Transform actionSpacer;
    public Transform itemSelectSpacer;
    public Transform magicSpacer;
    public Transform attackSelectSpacer;
    public GameObject actionButton;
    public GameObject magicButton;
    public GameObject itemSelectButton;
    public GameObject attackSelectButton;
    private List<GameObject> atkBtns = new List<GameObject>();

    // enemy buttons
    private List<GameObject> enemyBtns = new List<GameObject>();

	// Use this for initialization
	void Start ()
    {
        battleStates = PerformAction.WAIT;
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        heroInput = HeroGUI.ACTIVATE;

        ItemSelectPanel.SetActive(false);
        AttackSelectPanel.SetActive(false);
        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        MagicPanel.SetActive(false);


        EnemyButtons();
    }
	
	// Update is called once per frame
	void Update ()
    {
		switch(battleStates)
        {
            case (PerformAction.WAIT):
                if (PerformList.Count > 0)
                {
                    battleStates = PerformAction.TAKEACTION;
                }
                break;

            case (PerformAction.TAKEACTION):
                GameObject performer = GameObject.Find(PerformList[0].Attacker);
                if(PerformList[0].Type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    for(int i = 0; i<HeroesInBattle.Count; i++)
                    {
                        if (PerformList[0].AttackersTarget == HeroesInBattle[i])
                        {
                            ESM.HeroToAttack = PerformList[0].AttackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            PerformList[0].AttackersTarget = HeroesInBattle[Random.Range(0, HeroesInBattle.Count)];
                            ESM.HeroToAttack = PerformList[0].AttackersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                        }
                    }
                    
                }
                if (PerformList[0].Type == "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.EnemyToAttack = PerformList[0].AttackersTarget;
                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                }
                battleStates = PerformAction.PERFORMACTION;
                break;

            case (PerformAction.PERFORMACTION):
                //idle
            break;

            case (PerformAction.CHECKALIVE):
                if(HeroesInBattle.Count < 1)
                {
                    battleStates = PerformAction.LOSE;
                    //lose the battle
                }
                else if (EnemiesInBattle.Count < 1)
                {
                    battleStates = PerformAction.WIN;
                    //win the battle
                }
                else
                {
                    //call function
                    clearAttackPanel();
                    heroInput = HeroGUI.ACTIVATE;
                }
                break;
            case (PerformAction.WIN):
                Debug.Log("yoo win blyat");
                  for( int i = 0; i< HeroesInBattle.Count; i++)
                  {
                        HeroesInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
                  }

                break;
            case (PerformAction.LOSE):
                Debug.Log("yoo lose bitch");
            break;
        }


        switch (heroInput)
        {
            case (HeroGUI.ACTIVATE):
                if(HeroesToManage.Count > 0)
                {
                    HeroesToManage[0].transform.FindChild("Selector").gameObject.SetActive(true);
                    HeroChoice = new HandleTurns();

                    AttackPanel.SetActive(true);
                    //populate attack buttons
                    CreateAttackButtons();

                    heroInput = HeroGUI.WAITING;
                }
                break;

            case (HeroGUI.WAITING):
                //idle
                break;

            case (HeroGUI.DONE):
                HeroInputDone();
                break;
        }
	}

    public void CollectActions(HandleTurns input)
    {
        PerformList.Add(input);
    }

    public void EnemyButtons()
    {
        //cleanup
        foreach(GameObject enemyBtn in enemyBtns)
        {
            Destroy(enemyBtn);
        }
        enemyBtns.Clear();
        //create buttons
        foreach(GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton> ();

            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine> ();

            Text buttonText = newButton.transform.FindChild("Text").gameObject.GetComponent<Text> ();
            buttonText.text = cur_enemy.enemy.theName;

            button.EnemyPrefab = enemy;

            newButton.transform.SetParent(Spacer, false);
            enemyBtns.Add(newButton);
        }
    }
     
    void HeroInputDone()
    {
        PerformList.Add(HeroChoice);
        EnemySelectPanel.SetActive(false);
        //clean the attackapnel
        clearAttackPanel();

        HeroesToManage[0].transform.FindChild("Selector").gameObject.SetActive(false);
        HeroesToManage.RemoveAt(0);
        heroInput = HeroGUI.ACTIVATE;
    }

    void clearAttackPanel()
    {
        ItemSelectPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        AttackPanel.SetActive(false);
        MagicPanel.SetActive(false);

        foreach (GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();
    }
    //create actionbuttons
    void CreateAttackButtons()
    {
        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        Text AttackButtonText = AttackButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input6()); //callback to function '=>'
        AttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(AttackButton);

        //if more than 0 attack, list them else set button to inactive state
        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks.Count > 0)
        {
            foreach (BaseAttack melleAtk in HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks)
            {
                GameObject AttackSelectButton = Instantiate(attackSelectButton) as GameObject;
                Text AttackSelectButtonText = AttackSelectButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
                AttackSelectButtonText.text = melleAtk.attackName;
                AttackButton ATB = AttackSelectButton.GetComponent<AttackButton>();
                ATB.AttackToPerform = melleAtk;
                AttackSelectButton.transform.SetParent(attackSelectSpacer, false);
                atkBtns.Add(AttackSelectButton);
            }
        }
        else
        {
            AttackButton.GetComponent<Button>().interactable = false;
        }

        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        Text MagicAttackButtonText = MagicAttackButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
        MagicAttackButtonText.text = "Magic";
        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        MagicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(MagicAttackButton);

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.MagicAttacks.Count > 0)
        {
            foreach (BaseAttack magicAtk in HeroesToManage[0].GetComponent<HeroStateMachine>().hero.MagicAttacks)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;
                Text MagicButtonText = MagicButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
                MagicButtonText.text = magicAtk.attackName;
                AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                ATB.AttackToPerform = magicAtk;
                MagicButton.transform.SetParent(magicSpacer, false);
                atkBtns.Add(MagicButton);
            }
        }
        else
        {
            MagicAttackButton.GetComponent<Button>().interactable = false;
        }
        
        GameObject ItemSelectButton = Instantiate(actionButton) as GameObject;
        Text ItemSelectButtonText = ItemSelectButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
        ItemSelectButtonText.text = "Item";
        ItemSelectButton.GetComponent<Button>().onClick.AddListener(() => Input8());
        ItemSelectButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(ItemSelectButton);

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.ItemsInBag.Count > 0)
        {
            IList list = HeroesToManage[0].GetComponent<HeroStateMachine>().hero.ItemsInBag;
            for (int i = 0; i < list.Count; i++)
            {
                BaseItem item = (BaseItem)list[i];
                GameObject ItemSelectionButton = Instantiate(itemSelectButton) as GameObject;
                Text ItemButtonText = ItemSelectionButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
                ItemButtonText.text = item.itemName;
                AttackButton ATB = ItemSelectionButton.GetComponent<AttackButton>();
                ATB.ItemToPerform = item;
                ItemSelectionButton.transform.SetParent(itemSelectSpacer, false);
                atkBtns.Add(ItemSelectionButton);
            }
        }
        else
        {
            ItemSelectButton.GetComponent<Button>().interactable = false;
        }

    }
    /*
    public void Input1()//attack button
    {
        HeroChoice.Attacker = HeroesToManage[0].name;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";
        HeroChoice.choosenAttack = HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks[0];

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }
    */
    public void Input1(BaseItem ItemToUse) //choosen item
    {
        HeroChoice.Attacker = HeroesToManage[0].name;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";

        HeroChoice.choosenItem = ItemToUse;
        ItemSelectPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    public void Input2(GameObject choosenEnemy)
    {
        HeroChoice.AttackersTarget = choosenEnemy;
        heroInput = HeroGUI.DONE;
    }

    public void Input3()//switching to magic attacks selector
    {
        AttackPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }

    public void Input4(BaseAttack choosenMagic) //choosen magic attack
    {
        HeroChoice.Attacker = HeroesToManage[0].name;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";

        HeroChoice.choosenAttack = choosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    public void Input6()//switching to melee attacks selector
    {
        AttackPanel.SetActive(false);
        AttackSelectPanel.SetActive(true);
    }

    public void Input7(BaseAttack choosenAttack) //choosen melee attack
    {
        HeroChoice.Attacker = HeroesToManage[0].name;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";

        HeroChoice.choosenAttack = choosenAttack;
        AttackSelectPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    public void Input8()//switching to magic attacks selector
    {
        AttackPanel.SetActive(false);
        ItemSelectPanel.SetActive(true);
    }
}
