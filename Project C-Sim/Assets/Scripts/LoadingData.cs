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
	public int date;
	public int states;
	public int positive;
	public int negative;
	public int pending;
	public int hospitalizedCurrently;
	public int hospitalizedCumulative;
	public int inIcuCurrently;
	public int inIcuCumulative;
	public int onVentilatorCurrently;
	public int onVentilatorCumulative;
	public string dateChecked;
	public int death;
	public int hospitalized;
	public int totalTestResults;
	public string lastModified;
	public int total;
	public int posNeg;
	public int deathIncrease;
	public int hospitalizedIncrease;
	public int negativeIncrease;
	public int positiveIncrease;
	public int totalTestResultsIncrease;
	public string hash;

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
		UpdateText();
	}

	public void UpdateText()
	{
		Info stateInfo;
		switch (dropdown.value)
		{
			case 0:
				stateInfo = LoadData("https://api.covidtracking.com/v1/states/ny/current.json");
				break;
			case 1:
				stateInfo = LoadData("https://api.covidtracking.com/v1/states/va/current.json");
				break;
			case 2:
				stateInfo = LoadData("https://api.covidtracking.com/v1/states/oh/current.json");
				break;
			default:
				stateInfo = LoadData("https://api.covidtracking.com/v1/states/ny/current.json");
				break;
		}

		stateText.text = String.Format("Date Collected: {5}\nTotal Tested: {0}\nIncrease in Testing from Yesterday: {6}\nPositive: {1}\n Increase from Yesterday: {2}"
			+"\n# Hospitalized: {3}\nTotal Dead: {4}\nIncrease in Death: {7}",
			stateInfo.totalTestResults, stateInfo.positive, stateInfo.positiveIncrease, stateInfo.hospitalized, stateInfo.death,
			stateInfo.lastModified, stateInfo.totalTestResultsIncrease, stateInfo.deathIncrease);


	}

	private Info LoadData(string url)
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		StreamReader reader = new StreamReader(response.GetResponseStream());
		string jsonResponse = reader.ReadToEnd();
		Debug.Log(jsonResponse);
		return Info.CreateFromJSON(jsonResponse);
	}
}
