using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;

[Serializable]
public class Info
{
	public int positive;
	public string negative;
	public int hospitalizedCurrently;
	public int hospitalizedCumulative;
	public int death;
	public int hospitalized;
	public int totalTestResults;
	public int deathIncrease;
	public int hospitalizedIncrease;
	public int negativeIncrease;
	public int positiveIncrease;
	public int totalTestResultsIncrease;

	public static Info CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<Info>(jsonString);
	}
}

public class LoadingData : MonoBehaviour
{
	public TextMeshProUGUI stateText;
	public TextMeshProUGUI countryText;
	public TMP_Dropdown dropdown;


	// Start is called before the first frame update
	void Start()
	{
		//LoadData("https://api.covidtracking.com/v1/us/current.json");
	}

	public void UpdateText()
	{

		switch (dropdown.value)
		{
			case 0:
				break;
			case 1:
				break;
			case 2:
				break;
			default:
				break;
		}
	}

	private void LoadData(string url)
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		StreamReader reader = new StreamReader(response.GetResponseStream());
		string jsonResponse = reader.ReadToEnd();
		Debug.Log(jsonResponse);
		Info info = Info.CreateFromJSON(jsonResponse);
		Debug.Log(info.positive);
	}
}
