using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class House : Building
{
    public int NumberOfResidents { get; set; }
    public GameObject personPrefab { get; set; }
    public List<GameObject> people { get; set; }
    [SerializeField]
    public bool ResidentsInQuarantine { get; set; }

    public GameManager gameManager { get; set; }
    public int MaxHouseCapacity { get; set;}

    public GameObject Actors { get; set; }

    // Start is called before the first frame update
    public void Start()
    {
        NumberOfResidents = Random.Range(1, MaxHouseCapacity + 1);
        personPrefab = Resources.Load<GameObject>("Person");
        personPrefab.transform.localScale = new Vector2(.5f, .5f);
        people = new List<GameObject>();
        SpawnRandomResidents();
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
    }

    private void Update()
    {
        if(ResidentsInQuarantine)
        {
            GetComponent<SpriteRenderer>().color = new Color(0, 0.6f, 0);
            if(CheckQuarantineFinish())
            {
                //End quarantine
                ResidentsInQuarantine = false;
                foreach (GameObject person in people)
                {
                    person.GetComponent<Person>().Quarantining = false;
                }
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 0.7005286f, 0.3349057f, 1);
        }
    }

    private bool CheckQuarantineFinish()
    {
        foreach (GameObject person in people)
        {
            if (person.GetComponent<Person>().Infected)
                return false;
        }
        return true;
    }

    public void RemovePerson(GameObject person)
    {
        people.Remove(person);
    }
}
