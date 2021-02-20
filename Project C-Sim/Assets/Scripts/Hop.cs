using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hop : MonoBehaviour
{
    public float height;
    public float speed;
    public bool UIPerson;
    private float seed;
    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(0, 10000);   
    }

    // Update is called once per frame
    void Update()
    {
        if(!UIPerson)
        {
            if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 0.01f)
            {
                gameObject.transform.GetChild(0).transform.position = new Vector3(this.transform.position.x, this.transform.position.y + Mathf.Abs(Mathf.Sin((Time.time * speed + seed)) * height), this.transform.position.z);
            }
            else
            {
                gameObject.transform.GetChild(0).transform.position = Vector2.Lerp(transform.GetChild(0).transform.position, gameObject.transform.position, 0.1f);
            }
        }
        else
        {
                gameObject.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(this.transform.position.x, this.transform.position.y + Mathf.Abs(Mathf.Sin((Time.time * speed + seed)) * height), this.transform.position.z);
        }
    }
}
