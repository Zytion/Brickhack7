using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleScript : MonoBehaviour
{
    GameObject target;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            if(gm.People.Count > 0)
            target = gm.People[Random.Range(0, gm.People.Count)];
        }
        else
        {
            this.GetComponent<Rigidbody2D>().AddForce(target.transform.position - this.transform.position * 1);
        }
    }
}
