using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Vector2 SimulationExtents { get; set; }
    public Vector2 SimulationPosition { get; set; }
    public int NumberOfHouses { get; set; }
    public int NumberOfPointsOfInterest { get; set; }
    public List<GameObject> People { get; set; }
    public int MaxHouseCapacity { get; set; }
    public float SeperationDistance { get; set; }
    public float InfectionRadius { get; set; }
    public int NumInfected { get; set; }
    public int NumDead { get; set; }
    public GameObject StartResetButton { get; set; }
    private int numHealthy;
    public int NumHealthy
    {
        get { return initalPeople - NumInfected - NumDead; }
    }


    private List<Vector2> poiPositions;
    private List<Vector2> housePositions;
    private GameObject housePrefab;
    private List<GameObject> poiPrefabs;
    private float scaleFactor;
    private float buildingHalfWidth;
    private float sepForceMult;
    private float infectionTimer;
    private float infectionCoolDown;
    private float infectionRatio;
    private int initalPeople;
    private SimulationValues simValues;
    private bool isRunning;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        InfectionRadius = 1.5f;
        scaleFactor = 1f;
        buildingHalfWidth = scaleFactor / 2.0f;
        sepForceMult = 20.0f;
        isRunning = false;
        simValues = GameObject.Find("SimValues").GetComponent<SimulationValues>();
        GameObject simulation = GameObject.Find("Simulation");
        StartResetButton = GameObject.Find("Start_Reset_Button");
        StartResetButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (StartResetButton.transform.GetChild(0).GetComponent<Text>().text == "Start")
            {
                StartResetButton.transform.GetChild(0).GetComponent<Text>().text = "Reset";
                StartCoroutine(BeginSimulation());
            }
            else
            {
                StartResetButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
            }
        });
        SimulationExtents = simulation.GetComponent<BoxCollider2D>().bounds.extents;
        SimulationPosition = simulation.transform.position;
        InitializePrefabs();
    }

    /// <summary>
    /// Initialize the prefab values.
    /// </summary>
    public void InitializePrefabs()
    {
        housePrefab = Resources.Load<GameObject>("Buildings/House");
        housePrefab.transform.localScale = new Vector2(scaleFactor, scaleFactor);
        poiPrefabs = new List<GameObject>();
        poiPrefabs.Add(Resources.Load<GameObject>("Buildings/POI1"));
        poiPrefabs[0].transform.localScale = new Vector2(scaleFactor, scaleFactor);
        poiPrefabs.Add(Resources.Load<GameObject>("Buildings/POI2"));
        poiPrefabs[1].transform.localScale = new Vector2(scaleFactor, scaleFactor);
        poiPrefabs.Add(Resources.Load<GameObject>("Buildings/POI3"));
        poiPrefabs[2].transform.localScale = new Vector2(scaleFactor, scaleFactor);
    }

    /// <summary>
    /// Start the simulation
    /// </summary>
    IEnumerator BeginSimulation()
    {
        // Initialize sim values.
        People = new List<GameObject>();
        poiPositions = new List<Vector2>();
        housePositions = new List<Vector2>();
        isRunning = true;

        // Read values from the simulation sliders.
        ReadSimValues();

        // Generate a random number of houses.
        SpawnRandomBuildings(NumberOfHouses, false, housePrefab);
        // Generate a random number of points of interest.
        SpawnRandomBuildings(NumberOfPointsOfInterest, true, null);

        // Wait until there are people in the list.
        while(People.Count == 0)
        {
            yield return new WaitForEndOfFrame();
        }
        
        // keep track of the indicies of people initially infected.
        List<int> indiciesUsed = new List<int>();
        int count = 0;
        int numberInfected = (int)(People.Count * infectionRatio);
        // Make sure there is at least one initial infection.
        if (numberInfected == 0)
        {
            numberInfected++;
        }
        // Infect numberInfected number of people.
        while (count < numberInfected)
        {
            int randomIndex = Random.Range(0, People.Count);
            while (indiciesUsed.Contains(randomIndex))
            {
                randomIndex = ++randomIndex % People.Count;
            }
            People[randomIndex].GetComponent<Person>().Infected = true;
            count++;
        }

        NumInfected = numberInfected;
        initalPeople = People.Count;
    }
    public void ReadSimValues()
    {
        NumberOfHouses = simValues.numHouses;
        SeperationDistance = simValues.socialDistance / 5.0f;
        NumberOfPointsOfInterest = (int)simValues.placesOfInterest;
        MaxHouseCapacity = simValues.maxHouseDensity;
        infectionRatio = simValues.initallyInfected;
        Debug.Log("NumberOfHouses " + NumberOfHouses);
        Debug.Log("SeperationDistance " + SeperationDistance);
        Debug.Log("NumberOfPointsOfInterest " + NumberOfPointsOfInterest);
        Debug.Log("MaxHouseCapacity " + MaxHouseCapacity);
        Debug.Log("initallyInfected " + infectionRatio);
        infectionTimer = 0;
        infectionCoolDown = 1 / 6f;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="num"></param>
    /// <param name="isPOI"></param>
    /// <param name="prefab"></param>
    public void SpawnRandomBuildings(int num, bool isPOI, GameObject prefab)
    {
        for(int i = 0; i < num; i++)
        {
            Vector2 buildingPosition;
            do
            {
                float randomX = (SimulationPosition.x - SimulationExtents.x + buildingHalfWidth) 
                                 + (Random.Range(0, (int)((SimulationExtents.x * 2 / (buildingHalfWidth * 2)))) 
                                 * buildingHalfWidth * 2);
                float randomY = (SimulationPosition.y - SimulationExtents.y + buildingHalfWidth) 
                                 + (Random.Range(0, (int)((SimulationExtents.y * 2 / (buildingHalfWidth * 2)))) 
                                 * buildingHalfWidth * 2);
                buildingPosition = new Vector2(randomX, randomY);
            } while (poiPositions.Contains(buildingPosition) || housePositions.Contains(buildingPosition));

            GameObject house = Instantiate(isPOI ? GetRandomPointOfInterest() : prefab, buildingPosition, Quaternion.identity);
            if (!isPOI)
            {
                house.GetComponent<House>().gameManager = this;
                house.GetComponent<House>().MaxHouseCapacity = MaxHouseCapacity;
                housePositions.Add(buildingPosition);
            }
            else
            {
                poiPositions.Add(buildingPosition);
            }
        }
        
    }

    /// <summary>
    /// Gets a random point of interest prefab
    /// </summary>
    /// <returns></returns>
    public GameObject GetRandomPointOfInterest()
    {
        int random = Random.Range(0, poiPrefabs.Count);
        return poiPrefabs[random];
    }

    public Vector2 GetRandomBuilding()
    {
        return poiPositions[Random.Range(0, poiPositions.Count)];
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public Vector2 GetSeperation(GameObject person)
    {
        Vector2 netForce = Vector2.zero;
        foreach(GameObject p in People)
        {
            // Ignore the person if its equal to person, or they aren't moving, or they are too far.
            if (p == person 
                || !p.GetComponent<Person>().Moving
                || Vector3.SqrMagnitude(person.transform.position - p.transform.position) > SeperationDistance * SeperationDistance) continue;

            Vector2 diff = (Vector2)(person.transform.position - p.transform.position);
            netForce += diff * (SeperationDistance * SeperationDistance - diff.sqrMagnitude) * sepForceMult;
        }
        return netForce;
    }

    public void KillPerson(GameObject person)
    {
        People.Remove(person);
        //Instatiate skull and cross bones
        NumInfected--;
        NumDead++;
        Destroy(person);

    }

    public void CalculateInfections()
    {
        // Iterate through each person.
        for(int i = 0; i < People.Count -1; i++)
        {
            // If this person is infected, look at everyone else.
            if (People[i].GetComponent<Person>().Infected)
            {
                for(int j = 0; j < People.Count; j++)
                {
                    if (i == j
                        || People[j].GetComponent<Person>().Infected
                        || Vector3.SqrMagnitude(People[i].transform.position - People[j].transform.position) > InfectionRadius * InfectionRadius)  continue;
                    // Infect this person
                    //Get infection rate based on distance
                    float maskReduction = 0.4242f;
                    float distance = People[i].GetComponent<Person>().CloseToDest || People[j].GetComponent<Person>().CloseToDest ? SeperationDistance : Vector3.SqrMagnitude(People[i].transform.position - People[j].transform.position);
                    float infectionChance = (1 / (distance / 8 + 1 / 8)) / 200;
                    infectionChance *= (People[i].GetComponent<Person>().HasMask ? maskReduction : 1.0f) * (People[j].GetComponent<Person>().HasMask ? maskReduction : 1.0f);
                    infectionChance *= People[j].GetComponent<Person>().Recovered ? 0.1f : 1.0f;
                    People[j].GetComponent<Person>().Infected = Random.Range(0.0f, 1.0f) < infectionChance;
                    NumInfected++;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        if (!isRunning) return;

        infectionTimer += Time.deltaTime;
        // Time to check for infections again.
        if (infectionTimer > infectionCoolDown)
        {
            infectionTimer = 0;
            CalculateInfections();
        }
    }
}
