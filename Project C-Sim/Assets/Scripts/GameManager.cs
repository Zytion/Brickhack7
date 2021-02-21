using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BuildingType
{
    House,
    POI,
    Hospital
}
public class GameManager : MonoBehaviour
{
    public Vector2 SimulationExtents { get; set; }
    public Vector2 SimulationPosition { get; set; }
    public int NumberOfHouses { get; set; }
    public int NumberOfPointsOfInterest { get; set; }
    public int NumberOfHospitals { get; set; }
    public List<GameObject> People { get; set; }
    public int MaxHouseCapacity { get; set; }
    public float SeperationDistance { get; set; }
    public float InfectionRadius { get; set; }
    public int NumInfected { get; set; }
    public int NumDead { get; set; }
    public int NumRecovered { get; set; }
    public GameObject StartResetButton { get; set; }
    private int numHealthy;
    public int NumHealthy
    {
        get { return initalPeople - NumInfected - NumDead - NumRecovered; }
    }
    public List<Vector2> HospitalPositions { get; set; }


    private List<Vector2> poiPositions;
    private List<Vector2> housePositions;
    private GameObject housePrefab;
    private GameObject hospitalPrefab;
    private List<GameObject> poiPrefabs;
    private float scaleFactor;
    private float buildingHalfWidth;
    private float sepForceMult;
    private float infectionTimer;
    private float infectionCoolDown;
    private float infectionRatio;
    private float maskRatio;
    private float socialDistancingRatio;
    private int initalPeople;
    private SimulationValues simValues;
    private bool isRunning;
    public bool IsRunning => isRunning;
    private GameObject actors;
    private Window_Graph healthyGraph;
	private Window_Graph infectedGraph;
	private List<TextMeshProUGUI> GraphTexts;
	private int highest;
	private float graphTimer;

	[SerializeField] private List<int> iValues;
    [SerializeField] private List<int> sValues;

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
        actors = GameObject.Find("Actors");
		healthyGraph = GameObject.Find("healthyGraph").GetComponent<Window_Graph>();
		infectedGraph = GameObject.Find("infectedGraph").GetComponent<Window_Graph>();
		StartResetButton = GameObject.Find("Start_Reset_Button");

		graphTimer = 0;
		highest = 0;
		GraphTexts = new List<TextMeshProUGUI>();
		GraphTexts.AddRange(GameObject.Find("GraphData").GetComponentsInChildren<TextMeshProUGUI>());
		GraphTexts.AddRange(GameObject.Find("GraphStats").GetComponentsInChildren<TextMeshProUGUI>());

		StartResetButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartButtonPress();
        });
        SimulationExtents = simulation.GetComponent<BoxCollider2D>().bounds.extents;
        SimulationPosition = simulation.transform.position;
        InitializePrefabs();
    }

    public void StartButtonPress()
    {
        if (StartResetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "Start")
        {
            StartResetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Reset";
            StartCoroutine(BeginSimulation());
        }
        else
        {
            StartResetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Start";
            for (int i = 0; i < actors.transform.childCount; i++)
            {
                Destroy(actors.transform.GetChild(i).gameObject);
            }
            isRunning = false;
        }
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
        hospitalPrefab = Resources.Load<GameObject>("Buildings/Hospital");
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
        HospitalPositions = new List<Vector2>();
        isRunning = true;
        iValues = new List<int>();
        sValues = new List<int>();

        // Read values from the simulation sliders.
        ReadSimValues();

        // Generate a random number of houses.
        SpawnRandomBuildings(NumberOfHouses, BuildingType.House, housePrefab);
        // Generate a random number of points of interest.
        SpawnRandomBuildings(NumberOfPointsOfInterest, BuildingType.POI, null);
        // Only generate hospitals if the user chose to.
        if(NumberOfHospitals > 0)
        {
            SpawnRandomBuildings(NumberOfHospitals, BuildingType.Hospital, hospitalPrefab);
        }

        // Wait until there are people in the list.
        while(People.Count == 0)
        {
            yield return new WaitForEndOfFrame();
        }

        // keep track of the indicies of people initially infected.
        List<int> indiciesUsed = new List<int>();
        int count = 0;
        int numberInfected = (int)(People.Count * infectionRatio);
        NumInfected = numberInfected;
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
            indiciesUsed.Add(randomIndex);
            count++;
        }

        // keep track of the indicies of people initially infected.
        indiciesUsed.Clear();
        count = 0;
        int numberWearingMasks = (int)(People.Count * maskRatio);
        Debug.Log(maskRatio + ": " + numberWearingMasks);
        // Infect numberInfected number of people.
        while (count < numberWearingMasks)
        {
            int randomIndex = Random.Range(0, People.Count);
            while (indiciesUsed.Contains(randomIndex))
            {
                randomIndex = ++randomIndex % People.Count;
            }
            People[randomIndex].GetComponent<Person>().HasMask = true;
            indiciesUsed.Add(randomIndex);
            count++;
        }

        indiciesUsed.Clear();
        count = 0;
        int numberSocialDistant = (int)(People.Count * socialDistancingRatio);

        // Infect numberInfected number of people.
        while (count < numberSocialDistant)
        {
            int randomIndex = Random.Range(0, People.Count);
            while (indiciesUsed.Contains(randomIndex))
            {
                randomIndex = ++randomIndex % People.Count;
            }
            People[randomIndex].GetComponent<Person>().SocialDistancing = true;
            indiciesUsed.Add(randomIndex);
            count++;
        }


        initalPeople = People.Count;
    }
    public void ReadSimValues()
    {
        NumberOfHouses = simValues.numHouses;
        SeperationDistance = simValues.socialDistance / 5.0f;
        NumberOfPointsOfInterest = (int)simValues.placesOfInterest;
        MaxHouseCapacity = simValues.maxHouseDensity;
        NumberOfHospitals = simValues.numOfHospitals;
        infectionRatio = simValues.initallyInfected;
        maskRatio = simValues.maskWearingPercentage;
        socialDistancingRatio = simValues.populationSocialDistance;
        infectionTimer = 0;
        infectionCoolDown = 1 / 6f;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="num"></param>
    /// <param name="isPOI"></param>
    /// <param name="prefab"></param>
    public void SpawnRandomBuildings(int num, BuildingType buildingType, GameObject prefab)
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

            GameObject building = Instantiate(buildingType == BuildingType.POI ? GetRandomPointOfInterest() : prefab, buildingPosition, Quaternion.identity);

            switch (buildingType)
            {
                case BuildingType.House:
                    {
                        building.GetComponent<House>().gameManager = this;
                        building.GetComponent<House>().MaxHouseCapacity = MaxHouseCapacity;
                        building.GetComponent<House>().Actors = actors;
                        housePositions.Add(buildingPosition);
                        break;
                    }
                case BuildingType.POI:
                    {
                        poiPositions.Add(buildingPosition);
                        break;
                    }
                case BuildingType.Hospital:
                    {
                        poiPositions.Add(buildingPosition);
                        HospitalPositions.Add(buildingPosition);
                        break;
                    }
            }
            building.transform.parent = actors.transform;
        }
        
    }

    public void AudioToggle (Toggle toggle)
    {
        if(toggle.isOn)
        {
            toggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Unmute";
            this.GetComponent<AudioSource>().volume = 0;
        }
        else
        {
            toggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Mute";
            this.GetComponent<AudioSource>().volume = 1;
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
        GameObject par = (GameObject)Instantiate(Resources.Load("DeathParticleEffect"), person.transform.position, Quaternion.identity);
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
                        || Vector3.SqrMagnitude(People[i].transform.position - People[j].transform.position) > InfectionRadius * InfectionRadius
                        || (People[i].GetComponent<Person>().InHouse && People[i].GetComponent<Person>().home != People[j].GetComponent<Person>().home))  continue;
                    // Infect this person
                    //Get infection rate based on distance
                    float maskReduction = 0.4242f;
                    float distance = 0.0f;
                    if (People[i].GetComponent<Person>().CloseToDest || People[j].GetComponent<Person>().CloseToDest)
                    {
                        distance = SeperationDistance;
                        distance *= (People[i].GetComponent<Person>().SocialDistancing ? 0.5f : 1.0f) * (People[j].GetComponent<Person>().SocialDistancing ? 0.5f : 1.0f);
                    }
                    else
                        distance = Vector3.SqrMagnitude(People[i].transform.position - People[j].transform.position);

                    float infectionChance = (1.0f / ((distance / 2.0f) + (1.0f / 6.0f))) / 200.0f;
                    infectionChance *= (People[i].GetComponent<Person>().HasMask ? maskReduction : 1.0f) * (People[j].GetComponent<Person>().HasMask ? maskReduction : 1.0f);
                    infectionChance *= People[j].GetComponent<Person>().Recovered ? 0.1f : 1.0f;
                    infectionChance *= People[i].GetComponent<Person>().Quarantining && People[i].GetComponent<Person>().InHouse ? 0.1f : 1.0f;
                    People[j].GetComponent<Person>().Infected = Random.Range(0.0f, 1.0f) < infectionChance;
                    // Person was infected, so increment the infection counter.
                    if (People[j].GetComponent<Person>().Infected)
                    {
                        NumInfected++;
                    }
                }
            }
        }
    }

    public void UpdateGraph()
    {
        Debug.Log("Number Infected: " + NumInfected);
        Debug.Log("Number Susceptible: " + (NumHealthy + NumInfected));
        int iVal = (int)((NumInfected / (float)initalPeople) * 100);
        iValues.Add(iVal);
        int sVal = (int)(((NumHealthy + NumInfected) / (float)initalPeople) * 100);
        sValues.Add(sVal);

        Debug.Log(iVal + "," + sVal);
        if (iValues.Count > 30)
        {
            iValues.RemoveAt(0);
        }
        if (sValues.Count > 30)
        {
            sValues.RemoveAt(0);
        }

        for (int i = 0; i < iValues.Count; ++i)
        {
            healthyGraph.UpdateValue(i, sValues[i]);
            infectedGraph.UpdateValue(i, iValues[i]);
        }

		int sus = (int)(((NumHealthy) / (float)initalPeople) * 100);

		if (iVal > highest)
			highest = iVal;

		GraphTexts[0].text = string.Format("{0}% removed", (100 - (iVal + sus)));	//Removed
		GraphTexts[1].text = string.Format("{0}% susceptible", sus);  //Susceptible
		GraphTexts[2].text = string.Format("{0}% infected", iVal);  //Infected
		GraphTexts[4].text = string.Format("Highest infection amount: {0}%", highest);  //Highest
		GraphTexts[5].text = string.Format("Dead: {0}", NumDead);  //Estimated Dead

	}

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartButtonPress();
        }

        if(Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(Resources.Load<GameObject>("WhaleB"), Vector3.zero, Quaternion.identity);
        }

        if (!isRunning) return;

        infectionTimer += Time.deltaTime;
        // Time to check for infections again.
        if (infectionTimer > infectionCoolDown)
        {
            infectionTimer = 0;
            CalculateInfections();
		}
		graphTimer += Time.deltaTime;
		if (graphTimer > 2)
		{
			graphTimer = 0;
			UpdateGraph();
		}

	}
}
