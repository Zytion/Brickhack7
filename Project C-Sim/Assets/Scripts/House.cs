using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    public int NumberOfResidents { get; set; }
    public GameObject personPrefab { get; set; }
    public List<GameObject> people { get; set; }

    // Start is called before the first frame update
    public void Start()
    {
        this.NumberOfResidents = Random.Range(1, 5);
        this.personPrefab = Resources.Load<GameObject>("Person");
        people = new List<GameObject>();
        SpawnRandomResidents();
    }

    private void SpawnRandomResidents()
    {
        // Generate NumberOfResident number of people for this house.
        for(int i = 0; i < this.NumberOfResidents; i++)
        {
            GameObject newPerson = Instantiate(personPrefab, this.transform.position, Quaternion.identity);
            Person personScript = newPerson.AddComponent<Person>();
            personScript.AssignRandomAttributes();
            people.Add(newPerson);
        }
    }
}
