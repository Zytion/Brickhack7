using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector2 SimulationExtents { get; set; }
    public Vector2 SimulationPosition { get; set; }
    public int NumberOfHouses { get; set; }
    public int NumberOfPointsOfInterest { get; set; }
    public bool hasTownCenter;
    private List<Vector2> buildingPositions;
    private GameObject housePrefab;
    private GameObject townCenterPrefab;
    private List<GameObject> poiPrefabs;
    private float scaleFactor;
    private float buildingHalfWidth;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        hasTownCenter = true;
        NumberOfHouses = 5;
        NumberOfPointsOfInterest = 5;
        scaleFactor = 0.5f;
        buildingHalfWidth = scaleFactor / 2.0f;
        Debug.Log(buildingHalfWidth);
        InitializePrefabs();
        buildingPositions = new List<Vector2>();
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
        townCenterPrefab = Resources.Load<GameObject>("Buildings/TownCenter");
        townCenterPrefab.transform.localScale = new Vector2(scaleFactor, scaleFactor);
        housePrefab = Resources.Load<GameObject>("Buildings/House");
        housePrefab.transform.localScale = new Vector2(scaleFactor, scaleFactor);
        poiPrefabs = new List<GameObject>();
        poiPrefabs.Add(Resources.Load<GameObject>("Buildings/POI1"));
        poiPrefabs[0].transform.localScale = new Vector2(scaleFactor, scaleFactor);
        poiPrefabs.Add(Resources.Load<GameObject>("Buildings/POI2"));
        poiPrefabs[1].transform.localScale = new Vector2(scaleFactor, scaleFactor);
    }

    /// <summary>
    /// Start the simulation
    /// </summary>
    public void BeginSimulation()
    {
        // Generate a town center.
        if (hasTownCenter)
        {
            Instantiate(townCenterPrefab, Vector2.zero, Quaternion.identity);
            buildingPositions.Add(Vector2.zero);
        }
        // Generate a random number of houses.
        SpawnRandomBuildings(NumberOfHouses, housePrefab);
        // Generate a random number of points of interest.
        SpawnRandomBuildings(NumberOfPointsOfInterest, GetRandomPointOfInterest());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="num"></param>
    /// <param name="prefab"></param>
    public void SpawnRandomBuildings(int num, GameObject prefab)
    {
        for(int i = 0; i < num; i++)
        {
            Vector2 buildingPosition;
            do
            {
                float randomX = (SimulationPosition.x - SimulationExtents.x + buildingHalfWidth) + (Random.Range(0, (int)((SimulationExtents.x * 2 / (buildingHalfWidth * 2)))) * buildingHalfWidth * 2);
                float randomY = (SimulationPosition.y - SimulationExtents.y + buildingHalfWidth) + (Random.Range(0, (int)((SimulationExtents.y * 2 / (buildingHalfWidth * 2)))) * buildingHalfWidth * 2);
                Debug.Log(randomX + "," + randomY);
                buildingPosition = new Vector2(randomX, randomY);
            } while (buildingPositions.Contains(buildingPosition));

            buildingPositions.Add(buildingPosition);
            GameObject house = Instantiate(prefab, buildingPosition, Quaternion.identity);
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
    
    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        
    }
}
