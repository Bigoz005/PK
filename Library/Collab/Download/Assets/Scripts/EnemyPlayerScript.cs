using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerScript : Characters {

    // Use this for initialization
    private Position position;

    private Vector3 min, max;

    protected override void Start () {
        position = new Position();

        base.Start();
    }

    // Update is called once per frame
    protected override void Update () {

        /* zatrzymanie gracza na mapie ->*/
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), Mathf.Clamp(transform.position.y, min.y, max.y), transform.position.z);   //zatrzymanie gracza na mapie
       
        Move(); // -------------- zeby nie uzalezniac ruchu od serwera zakomentuj ta funkcje!!!!
        base.Update();

    }

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
}
