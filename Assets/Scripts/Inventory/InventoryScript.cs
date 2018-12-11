using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    private static InventoryScript instance;

    public static InventoryScript MyInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<InventoryScript>();
            }
            return instance;
        }
    }

    private SlotScript fromSlot;

    private List<Bag> bags = new List<Bag>();

    [SerializeField]
    private BagButton[] bagButtons;


    //debugging
    [SerializeField]
    private Item[] items;

    public bool BagCapacity
    {
        get { return bags.Count < 4; }
    }

    public int MyEmptySlotCount
    {
        get
        {
            int count = 0;

            foreach (Bag bag in bags)
            {
                count += bag.MyBagScript.MyEmptySlotCount;
            }
            return count;
        }
    }

    public SlotScript FromSlot
    {
        get
        {
            return fromSlot;
        }

        set
        {
            fromSlot = value;
            if(value != null)
            {
                fromSlot.MyIcon.color = Color.grey; //przyciemnienie podniesionego itemka
            }
        }
    }

    private void Awake()
    {
        Bag bag = (Bag)Instantiate(items[0]);   //make
        bag.Initialize(16);                     //build
        bag.Use();                              //gibe
    }

    private void Update() //dodawanie itemsów - TESTY
    {
        if (Input.GetKeyDown(KeyCode.J))    //dodawanie torby do slotu toreb
        {
            Bag bag = (Bag)Instantiate(items[0]);
            bag.Initialize(16);
            bag.Use();
        }
        if (Input.GetKeyDown(KeyCode.K))    //dodawanie torby do wnętrza eq
        {
            Bag bag = (Bag)Instantiate(items[0]);
            bag.Initialize(16);
            AddItem(bag);
        }
        if (Input.GetKeyDown(KeyCode.L))    //dodawanie potiona
        {
            HealthPotion hppotion = (HealthPotion)Instantiate(items[1]);
            AddItem(hppotion);
        }
        if (Input.GetKeyDown(KeyCode.Y))    //dodawanie helmeta
        {
            AddItem((Armor)Instantiate(items[2]));
            AddItem((Armor)Instantiate(items[3]));
            AddItem((Armor)Instantiate(items[4]));
            AddItem((Armor)Instantiate(items[5]));
            AddItem((Armor)Instantiate(items[6]));
            AddItem((Armor)Instantiate(items[7]));
            AddItem((Armor)Instantiate(items[8]));
            AddItem((Armor)Instantiate(items[9]));
            
            //Armor helm = (Armor)Instantiate(items[2]);
            //AddItem(helm);
        }
    }

    public void AddBag(Bag bag)     //dodawanie torby do paska toreb
    {
        foreach(BagButton bagButton in bagButtons)
        {
            if (bagButton.MyBag == null)
            {
                bagButton.MyBag = bag;
                bags.Add(bag);
                bag.MyBagButton = bagButton;
                bag.MyBagScript.transform.SetSiblingIndex(bagButton.BagIndex);
                break;
            }
        }
    }

    public void RemoveBag(Bag bag)
    {
        bags.Remove(bag);
        Destroy(bag.MyBagScript.gameObject);
    }

    public void AddItem(Item item)  //dodawanie przedmiotu do torby
    {
        if(item.MyStackSize > 0)
        {
            if (PlaceInStack(item))
            {
                return;
            }

        }
        PlaceInEmpty(item);
    }

    private void PlaceInEmpty(Item item)    //dodawanie przedmiotu do wolnego pola
    {
        foreach(Bag bag in bags)
        {
            if (bag.MyBagScript.AddItem(item))
            {
                return;
            }
        }
    }

    private bool PlaceInStack(Item item)    //dodawanie przedmiotu do stosu
    {
        foreach(Bag bag in bags)
        {
            foreach (SlotScript slots in bag.MyBagScript.MySlots)
            {
                if (slots.StackItem(item))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void OpenClose()
    {
        bool closedBag = bags.Find(x => !x.MyBagScript.IsOpen);
        //if closed == tru, then open all closed
        //if closed == falsz, then close all

        foreach(Bag bag in bags)
        {
            if (bag.MyBagScript.IsOpen != closedBag)
            {
                bag.MyBagScript.OpenClose();
            }
        }
    }
    
}
