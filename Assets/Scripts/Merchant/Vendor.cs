using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : MonoBehaviour, IInteractable
{

    [SerializeField]
    private VendorItem[] items;

    [SerializeField]
    private VendorWindow vendorWindow;

    public GameObject Mob = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D coll = Mob.GetComponent<Collider2D>();

                //if (!IsOpen)
                //{
                //IsOpen = true;
                if (coll.OverlapPoint(wp))
                {
                    IsOpen = true;
                    vendorWindow.CreatePages(items);
                    vendorWindow.Open(this);
                }
                else if(!(coll.OverlapPoint(wp)))
                {
                    IsOpen = false;
                    vendorWindow.Close();
                }
                //}
            }
        }
    }

    public bool IsOpen
    {
        get;
        set;
    }

    public void Interact()
    {
        vendorWindow.CreatePages(items);
        vendorWindow.Open(this);
    }

    public void StopInteract()
    {
        //if (IsOpen)
        //{
            //IsOpen = false;
            vendorWindow.Close();
        //}
    }

}
