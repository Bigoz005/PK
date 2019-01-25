using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour {

    private Image content;

    [SerializeField]
    private Text statValue;

    private float CurrentFill;
    public float MyMaxValue { get; set; }

    private float CurrentValue;

    public bool IsFull
    {
        get { return content.fillAmount == 1; }
    }

    private float overflow;

    public float MyOverflow
    {
        get
        {
            float tmp = overflow;
            overflow = 0;
            return tmp;
        }
    }

    private Text GoldAmount;
    private int CurrentGold;

    public int MyCurrentGold
    {
        get
        {
            return CurrentGold;
        }
        set
        {
            if(value < 0)
            {
                CurrentGold = 0;
            }
            else
            {
                CurrentGold = value;
            }
            if(GoldAmount != null)
            {
                GoldAmount.text = CurrentGold.ToString();
            }
        }
    }

    public float MyCurrentValue
    {
        get
        {
            return CurrentValue;
        }
        set
        {
            if(value > MyMaxValue) //blokada "nadstanu" życia
            {
                overflow = value - MyMaxValue;
                CurrentValue = MyMaxValue;
            }
            else if(value < 0) //blokada "ujemnego" życia
            {
                CurrentValue = 0;
            }
            else
            {
                CurrentValue = value; //ustawienie wartości
            }

            CurrentFill = CurrentValue / MyMaxValue;

            if (statValue != null)
            {
                statValue.text = CurrentValue + "/" + MyMaxValue;
            }
        }
    }
    

	// Use this for initialization
	void Start () {
        content = GetComponent<Image>();

	}
	
	// Update is called once per frame
	void Update () {

        content.fillAmount = CurrentFill;
		
	}

    public void Initialize(float CurrentValue, float MaxValue)
    {
        MyMaxValue = MaxValue;
        MyCurrentValue = CurrentValue;      
    }

    public void InitializeGold(int CurrentGold)
    {
        MyCurrentGold = CurrentGold;
    }

    public void Reset()
    {
        content.fillAmount = 0;
    }
}
