using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Characters
{

    private static Player instance;

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
    private float initHealth = 100; //początkowe zdrowie postaci

    private IInteractable interactable;

    private Vector3 min, max;

    // Use this for initialization
    protected override void Start()
    {
        health.Initialize(initHealth, initHealth);

        base.Start();
    }

    // Update is called once per frame //wywolanie dzialania w odwolaniu do characters
    protected override void Update()
    {
        //health.MyCurrentValue = 100;    //TEST health

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), Mathf.Clamp(transform.position.y, min.y, max.y), transform.position.z);   //zatrzymanie gracza na mapie
        Controls();
        base.Update(); //wywolanie z funkcji dziedziczącej
    }

    private void Controls()
    {
        direction = Vector2.zero;

        //TESTY
        if (Input.GetKeyDown(KeyCode.I))
        {
            health.MyCurrentValue -= 10;
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
        if(collision.tag == "Interactable")
        {
            interactable = collision.GetComponent<IInteractable>();
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

}
