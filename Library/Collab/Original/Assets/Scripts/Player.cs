using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Player : Characters
{
    
    private Position position;
    //Wyzej dodane przez janiaka

    private static Player instance;
    public StatsHandler statContainer;

    public static Player MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
            }
            return instance;
        }
    }

    [SerializeField]
    public float maxInitHealth = 100; //początkowe zdrowie postac

    [SerializeField]
    public float currentMP;

    [SerializeField]
    public Stat XpStat;

    [SerializeField]
    private Text levelText;

    private IInteractable interactable;

    private Vector3 min, max;

    [SerializeField]
    private Text GoldText;

    public int MyGold
    {
        get;

        set;
    }

    public Text MyGoldText
    {
        get
        {
            return GoldText;
        }

        set
        {
            GoldText = value;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        position = new Position();// Nad tym komentem elementy dodane przez Janiaka

        DontDestroyOnLoad(statContainer = GameObject.Find("StatsContainer").GetComponent<StatsHandler>());

        currentMP = statContainer.currentMP;
        MyGold = statContainer.gold;
        GoldText.text = MyGold.ToString();
        health.Initialize(statContainer.currentHP, maxInitHealth);
        XpStat.Initialize(statContainer.exp, Mathf.Round(100 * MyLevel * Mathf.Pow(MyLevel, 0.5f)));
        MyLevel = statContainer.level;
        levelText.text = MyLevel.ToString();
        
  
        base.Start();
        //Setposition(position.X, position.Y);
    }

    // Update is called once per frame //wywolanie dzialania w odwolaniu do characters
    protected override void Update()
    {
        //health.MyCurrentValue = 100;    //TEST health

        /* zatrzymanie gracza na mapie ->*/ transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), Mathf.Clamp(transform.position.y, min.y, max.y), transform.position.z);   //zatrzymanie gracza na mapie
        Controls();
        //Move(); // -------------- zeby nie uzalezniac ruchu od serwera zakomentuj ta funkcje!!!!
        base.Update(); //wywolanie z funkcji dziedziczącej
                       //transform.position = new Vector3(position.X, position.Y, 0); //dla testu


        UpdateStats();
    }

    private void Controls()
    {
        direction = Vector2.zero;

        //TESTY
        if (Input.GetKeyDown(KeyCode.I))
        {
            //health.MyCurrentValue -= 10;
            GainXP(786);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            health.MyCurrentValue += 10;
        }

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }

        //Pomizej zmiany Janiak

        Vector3 Playerposition = gameObject.transform.position;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Playerposition.y >= position.Y)
            {
                //direction += Vector2.up;
                ClientScript.Move("Up");
            }

        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Playerposition.y <= position.Y)
            {
                //direction += Vector2.down;
                ClientScript.Move("Down");
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Playerposition.x <= position.X)
            {
                //direction += Vector2.left;
                ClientScript.Move("Left");
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Playerposition.x >= position.X)
            {
                //direction += Vector2.right;
                ClientScript.Move("Right");
            }
        }
    }

    public void GainXP(int xp)
    {
        XpStat.MyCurrentValue += xp;

        if(XpStat.MyCurrentValue >= XpStat.MyMaxValue)
        {
            StartCoroutine(LevelUp());
        }
    }

    private IEnumerator LevelUp()
    {
        while (!XpStat.IsFull)
        {
            yield return null;
        }
        MyLevel++;
        levelText.text = MyLevel.ToString();
        XpStat.MyMaxValue = 100 * MyLevel * Mathf.Pow(MyLevel, 0.5f);
        XpStat.MyMaxValue = Mathf.Round(XpStat.MyMaxValue);
        XpStat.MyCurrentValue = XpStat.MyOverflow;
        //XpStat.Reset();
    }

    public void SetLimits(Vector3 min, Vector3 max) //postac nie wychodzi poza mape
    {
        this.min = min;
        this.max = max;
    }

    public void Interact()
    {
        if(interactable != null)
        {
            interactable.Interact();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform.root.tag == "Interactable")
        {
            interactable = collision.gameObject.transform.root.GetComponent<IInteractable>();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Interactable")
        {
            if (interactable != null)
            {
                interactable.StopInteract();
                interactable = null;
            }
        }

    }

    //Nizej elementy dopisane przez Janiaka

    class Position
    {
        public float X { get; set; }
        public float Y { get; set; }

    }

    public void Getposition(float x, float y)
    {
        position.X = x;
        position.Y = y;
    }

    public void Move()
    {
        //transform.Translate(direction*speed*Time.deltaTime);
        Vector3 Playerposition = gameObject.transform.position;
        if (Playerposition.x < position.X)
        {
            direction += Vector2.right;
            //to polecenie oznacza = direction (1,0,0)*speed(np 1.5) to przesunie o 1.5 postać w jednym framie przy dodaniu * deltaTime
            //przesunie postac o 1.5 w sekunde
            //transform.Translate(direction * speed);


            //if (direction.x != 0 || direction.y != 0)
            //{
            //    animator.SetLayerWeight(1, 1);
            //    Animate();
            //}
            //else
            //{

            //}


        }
        else if (Playerposition.x > position.X)
        {
            direction += Vector2.left;
            //transform.Translate(direction * speed);

            //if (direction.x != 0 || direction.y != 0)
            //{
            //    Animate();
            //}

        }

        else if (Playerposition.y < position.Y)
        {
            direction += Vector2.up;
            //transform.Translate(direction * speed);

            //if (direction.x != 0 || direction.y != 0)
            //{
            //    Animate();
            //}

        }

        else if (Playerposition.y > position.Y)
        {
            direction += Vector2.down;
            //transform.Translate(direction * speed);

            //if (direction.x != 0 || direction.y != 0)
            //{
            //    Animate();
            //}

        }
    }

    void UpdateStats()
    {
        statContainer.currentHP = health.MyCurrentValue;
        statContainer.gold = MyGold;
        statContainer.currentMP= currentMP;
        statContainer.exp = XpStat.MyCurrentValue;
        statContainer.level = MyLevel;
        /*  statContainer.currentMP = heroBattle.currentMP;
          statContainer.curATK = heroBattle.curATK;
          statContainer.curDEF = heroBattle.curDEF;
          statContainer.stamina = heroBattle.stamina;
          statContainer.agility = heroBattle.agility;
          statContainer.dexterity = heroBattle.dexterity;
          statContainer.intellect = heroBattle.intellect;*/
    }

    public void Setposition(float x, float y)
    {
        transform.position = new Vector3(x, y, 0);
    }

}
