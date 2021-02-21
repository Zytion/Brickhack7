using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTest : MonoBehaviour
{

	public Window_Graph healthyGraph;
	public Window_Graph infectedGraph;

	private List<int> iValues;
	private List<int> sValues;

	private float time;

	// Start is called before the first frame update
	void Start()
	{
		iValues = new List<int>(30);
		sValues = new List<int>(30);
		for (int i = 0; i < iValues.Capacity; ++i)
		{
			int newVal = Random.Range(0, 70);
			iValues.Add(newVal);
			sValues.Add(newVal + Random.Range(5, 30));
		}
	}

	// Update is called once per frame
	void Update()
	{
		time += Time.deltaTime;
		if (time > 1)
		{
			time = 0;
			iValues.RemoveAt(0);
			sValues.RemoveAt(0);
			int newVal = Random.Range(0, 70);
			iValues.Add(newVal);
			sValues.Add(newVal + Random.Range(5, 30));
			for (int i = 0; i < iValues.Count; ++i)
			{
				healthyGraph.UpdateValue(i, sValues[i]);
				infectedGraph.UpdateValue(i, iValues[i]);
			}
		}
	}
}
