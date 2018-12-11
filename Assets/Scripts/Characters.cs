using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public abstract class Characters : MonoBehaviour
{ //klasa wzorcowa

    [SerializeField]
    protected Stat health; //parametr życia wszystkich obiektów

    public Stat MyHealth
    {
        get
        {
            return health;
        }
    }

    [SerializeField]
    private float speed;

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

        transform.Translate(direction.normalized * speed * Time.deltaTime); //Time.deltaTime = time passed since the last update


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
