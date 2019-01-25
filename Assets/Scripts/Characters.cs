using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public abstract class Characters : MonoBehaviour
{ //klasa wzorcowa

    [SerializeField]
    protected Stat health; //parametr życia wszystkich obiektów

    //[SerializeField]
    protected Stat gold;

    public Stat MyHealth
    {
        get
        {
            return health;
        }
    }

    public int MyLevel
    {
        get
        {
            return level;
        }

        set
        {
            level = value;
        }
    }


    [SerializeField]
    private float speed;

    [SerializeField]
    private int level;

    private Animator animator;

    protected Vector2 direction;

    // Use this for initialization
    protected virtual void Start()
    {

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame //główny render dzialan wszystkich postaci
    protected virtual void Update()
    {
        /*if (!isLocalPlayer)
        {
            return;
        }*/

        Movement();
        
    }

    public void Movement()
    {

        //transform.Translate(direction.normalized * speed * Time.deltaTime); //Time.deltaTime = time passed since the last update
        transform.Translate(direction * speed); // Musi być bez deltatime jezeli serwer ma to ogarniac

        if (direction.x != 0 || direction.y != 0) //rozpoczęcie ruchu
        {
            AnimateMovement();
        }
        else
        {
            animator.SetLayerWeight(1, 0);  //zakonczenie ruchu, puszczenie klawiszy
        }
    }

    public void AnimateMovement()  
    {
        animator.SetLayerWeight(1, 1);

        animator.SetFloat("x", direction.x);
        animator.SetFloat("y", direction.y);

    }
}
