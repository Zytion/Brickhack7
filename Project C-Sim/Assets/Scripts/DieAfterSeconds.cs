using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterSeconds : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DieAfterSecondsMethod());
    }

    IEnumerator DieAfterSecondsMethod ()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
