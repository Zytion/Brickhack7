using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector2 SimulationExtents { get; set; }
    public Vector2 SimulationPosition { get; set; }
    public int NumberOfHouses { get; set; }
    public int NumberOfPointsOfInterest { get; set; }
    private List<Vector2> poiPositions;
    private List<Vector2> housePositions;
    private GameObject housePrefab;
    private List<GameObject> poiPrefabs;
    private float scaleFactor;
    private float buildingHalfWidth;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        NumberOfHouses = 1;
        NumberOfPointsOfInterest = 5;
        scaleFactor = 1f;
        buildingHalfWidth = scaleFactor / 2.0f;
        Debug.Log(buildingHalfWidth);
        InitializePrefabs();
        poiPositions = new List<Vector2>();
        housePositions = new List<Vector2>();
        GameObject simulation = GameObject.Find("Simulation");
        SimulationExtents = simulation.GetComponent<BoxCollider2D>().bounds.extents;
        SimulationPosition = simulation.transform.position;
        // Start the simulation
        // TODO: move to a button click event.
        BeginSimulation();
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
    public void BeginSimulation()
    {
        // Generate a random number of houses.
        SpawnRandomBuildings(NumberOfHouses, false, housePrefab);
        // Generate a random number of points of interest.
        SpawnRandomBuildings(NumberOfPointsOfInterest, true, null);
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
    public void Update()
    {
        
    }
}
