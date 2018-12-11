using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour {

    private Image content;

    [SerializeField]
    private Text statValue;

    private float CurrentFill;
    public float MaxHealthValue { get; set; }

    private float CurrentValue;

    public float MyCurrentValue
    {
        get
        {
            return CurrentValue;
        }
        set
        {
            if(value > MaxHealthValue) //blokada "nadstanu" życia
            {
                CurrentValue = MaxHealthValue;
            }
            else if(value < 0) //blokada "ujemnego" życia
            {
                CurrentValue = 0;
            }
            else
            {
                CurrentValue = value; //ustawienie wartości
            }

            CurrentFill = CurrentValue / MaxHealthValue;

            if (statValue != null)
            {
                statValue.text = CurrentValue + "/" + MaxHealthValue;
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
        MaxHealthValue = MaxValue;
        MyCurrentValue = CurrentValue;
    }
}
