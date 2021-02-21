using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPersonSpawner : MonoBehaviour
{
    private float newTime;
    private float timer;
    private List<GameObject> people;
    public float distanceDeath;

    private void Start()
    {
        people = new List<GameObject>();
        newTime = 0;
        timer = 0.2f;
    }
    // Update is called once per frame
    void Update()
    {
        newTime += Time.deltaTime;
        if(newTime > timer)
        {
            newTime = 0;
            timer = Random.Range(0.8f, 0.2f);
            GameObject person = Instantiate(Resources.Load("UIPerson"), gameObject.GetComponent<RectTransform>().position, Quaternion.identity) as GameObject;
            if (Random.Range(0, 15) == 9)
            {
                person.transform.GetChild(0).GetComponent<Image>().color = Color.red;
            }
            person.transform.SetParent(this.transform);
            people.Add(person);
        }

        if(people.Count > 0)
        {
            for(int i = 0; i < people.Count; i++)
            {
                people[i].GetComponent<RectTransform>().position = new Vector3(people[i].GetComponent<RectTransform>().position.x - 0.03f, people[i].GetComponent<RectTransform>().position.y, people[i].GetComponent<RectTransform>().position.z);
                if(Vector2.Distance(this.GetComponent<RectTransform>().position, people[i].GetComponent<RectTransform>().position) > distanceDeath)
                {
                    GameObject per = people[i];
                    people.RemoveAt(i);
                    Destroy(per);
                }
            }
        }
    }
}
