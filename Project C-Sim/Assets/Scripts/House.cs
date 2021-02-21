using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class House : Building
{
    public int NumberOfResidents { get; set; }
    public GameObject personPrefab { get; set; }
    public List<GameObject> people { get; set; }
    public bool ResidentsInQuarantine { get; set; }

    public GameManager gameManager { get; set; }
    public int MaxHouseCapacity { get; set;}

    public GameObject Actors { get; set; }

    private float quarantineTimer;
    private float quarantineTime;

    // Start is called before the first frame update
    public void Start()
    {
        NumberOfResidents = Random.Range(1, MaxHouseCapacity + 1);
        personPrefab = Resources.Load<GameObject>("Person");
        personPrefab.transform.localScale = new Vector2(.5f, .5f);
        people = new List<GameObject>();
        SpawnRandomResidents();
        quarantineTime = 45f;
    }

    public void SpawnRandomResidents()
    {
        // Generate NumberOfResident number of people for this house.
        for(int i = 0; i < this.NumberOfResidents; i++)
        {
            GameObject newPerson = Instantiate(personPrefab, this.transform.position, Quaternion.identity);
            Person personScript = newPerson.GetComponent<Person>();
            personScript.AssignRandomAttributes();
            personScript.home = this.gameObject;
            personScript.gameManager = this.gameManager;
            people.Add(newPerson);
            gameManager.People.Add(newPerson);
            newPerson.transform.parent = Actors.transform;
        }
    }

    public void QuarantineResidents()
    {
        ResidentsInQuarantine = true;
        foreach(GameObject person in people)
        {
            person.GetComponent<Person>().Quarantining = true;
        }
        quarantineTimer = 0;
    }

    private void Update()
    {
        if(ResidentsInQuarantine)
        {
            if(quarantineTimer > quarantineTime)
            {
                //End quarantine
                ResidentsInQuarantine = false;
                foreach (GameObject person in people)
                {
                    person.GetComponent<Person>().Quarantining = false;
                }
            }
            quarantineTimer += Time.deltaTime;
        }
    }
}
