using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleScript : MonoBehaviour
{
    public GameObject target;
    public GameManager gm;
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
            gameObject.transform.GetChild(0).transform.position = Vector2.Lerp(transform.GetChild(0).transform.position, gameObject.transform.position, 0.1f);
            if (gm.IsRunning)
            {
                if(gm.People.Count > 0)
                target = gm.People[Random.Range(0, gm.People.Count)].gameObject;
            }
        }
        else
        {
            gameObject.transform.GetChild(0).transform.position = new Vector3(this.transform.position.x, this.transform.position.y + Mathf.Abs(Mathf.Sin((Time.time * 6f + 1525f)) * 0.4f), this.transform.position.z);
            gameObject.transform.position = Vector3.Lerp(target.transform.position, this.transform.position, 0.988f);
            if(Vector2.Distance(this.transform.position, target.transform.position) < 0.2f)
            {
                gm.KillPerson(target);
            }
        }
    }
}
