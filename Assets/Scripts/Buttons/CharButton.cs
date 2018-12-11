using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField]
    private ArmorType armorType;

    private Armor equippedArmor;

    [SerializeField]
    private Image icon;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(HandScript.MyInstance.MyMoveable is Armor)
            {
                Armor tmp = (Armor)HandScript.MyInstance.MyMoveable;

                if (tmp.MyArmorType == armorType)
                {
                    EquipArmor(tmp);
                }

                UIManager.MyInstance.RefreshTooltip(tmp);
            }
            else if(HandScript.MyInstance.MyMoveable == null && equippedArmor != null)
            {
                HandScript.MyInstance.TakeMoveable(equippedArmor);
                CharacterPanel.MyInstance.MySelectedButton = this;
                icon.color = Color.grey;
            }
        }
    }

    public void EquipArmor(Armor armor)
    {
        armor.Remove();

        if(equippedArmor != null)
        {
            if(equippedArmor != armor)
            {
                armor.MySlot.AddItem(equippedArmor);
            }
            
            UIManager.MyInstance.RefreshTooltip(equippedArmor);
        }
        else
        {
            UIManager.MyInstance.HideTooltip();
        }

        icon.enabled = true;
        icon.sprite = armor.MyIcon;
        icon.color = Color.white;
        this.equippedArmor = armor; //reference to the item equipped
        this.equippedArmor.MyCharButton = this;


        if (HandScript.MyInstance.MyMoveable == (armor as IMoveable))    //blokada
        {
            HandScript.MyInstance.Drop();
        }
        
    }

    public void DequipArmor()
    {
        icon.color = Color.white;
        icon.enabled = false;
        equippedArmor.MyCharButton = null;
        equippedArmor = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(equippedArmor != null)
        {
            UIManager.MyInstance.ShowTooltip(new Vector2(0,1), transform.position,equippedArmor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.MyInstance.HideTooltip();
    }

}
