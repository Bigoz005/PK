using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneToBattle : MonoBehaviour {

    public GameObject Mob = null;

    void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D coll = Mob.GetComponent<Collider2D>();

                if(coll.OverlapPoint(wp))
                {
                    SceneManager.LoadScene("BattleScene");
                }
            }
        }
    }
}
