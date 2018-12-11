using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Item : ScriptableObject, IMoveable, IDescribable
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private int stackSize;

    private SlotScript slot;

    [SerializeField]
    private string title;

    [SerializeField]
    private Quality quality;

    private CharButton charButton;

    


    public Sprite MyIcon
    {
        get
        {
            return icon;
        }
    }

    public int MyStackSize
    {
        get
        {
            return stackSize;
        }
    }

    public SlotScript MySlot
    {
        get
        {
            return slot;
        }
        set
        {
            slot = value;
        }
    }

    public CharButton MyCharButton
    {
        get
        {
            return charButton;
        }

        set
        {
            MySlot = null;
            charButton = value;
        }
    }

    public virtual string GetDescription()  //ustawianie opisu itemka
    {
        return string.Format("<color={0}>* {1} *</color>", QualityColor.MyColors[quality], title);
    }

    public void Remove()
    {
        if(MySlot != null)
        {
            MySlot.RemoveItem(this);
            
        }
    }
}
