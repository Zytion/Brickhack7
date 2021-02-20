using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sex
{
    Male,
    Female
}
public class Person : MonoBehaviour
{
    public int Age { get; set; }
    public Sex Sex { get; set; }
    
    // Start is called before the first frame update
    public void Start()
    {

    }

    public void AssignRandomAttributes()
    {
        this.Sex = Random.Range(0, 2) == 0 ? Sex.Male : Sex.Female;
        this.Age = Random.Range(1, 90);
    }
    
    // Update is called once per frame
    public void Update()
    {
        
    }
}
