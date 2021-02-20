using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hop : MonoBehaviour
{
    public float height;
    public float speed;
    private float seed;
    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(0, 10000);   
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.GetChild(0).transform.position = new Vector3(this.transform.position.x, this.transform.position.y + Mathf.Abs(Mathf.Sin(Time.time * speed + seed) * height), this.transform.position.z);
    }
}
