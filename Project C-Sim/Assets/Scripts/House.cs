using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    public int NumberOfResidents { get; set; }
    public GameObject personPrefab { get; set; }
    public List<GameObject> people { get; set; }

    public GameManager gameManager { get; set; }

    // Start is called before the first frame update
    public void Start()
    {
        NumberOfResidents = 1;
        personPrefab = Resources.Load<GameObject>("Person");
        personPrefab.transform.localScale = new Vector2(.5f, .5f);
        people = new List<GameObject>();
        SpawnRandomResidents();
    }

    public void SpawnRandomResidents()
    {
        Debug.Log(gameManager);
        // Generate NumberOfResident number of people for this house.
        for(int i = 0; i < this.NumberOfResidents; i++)
        {
            GameObject newPerson = Instantiate(personPrefab, this.transform.position, Quaternion.identity);
            Person personScript = newPerson.GetComponent<Person>();
            personScript.AssignRandomAttributes();
            personScript.home = this.gameObject;
            personScript.gameManager = this.gameManager;
            Debug.Log(personScript.gameManager);
            people.Add(newPerson);
        }
    }
}
