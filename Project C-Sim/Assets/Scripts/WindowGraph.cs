/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WindowGraph : MonoBehaviour
{

	[SerializeField] private Sprite circleSprite;
	//[SerializeField] private Sprite upSprite;
	//[SerializeField] private Sprite downSprite;
	private RectTransform graphContainer;
	//public bool showCircles = true;
	public List<int> infectedValues;
	public List<int> suseptableValues;
	private List<GameObject> dots;

	private int highest, hightestDay, population, dead;

	[SerializeField] private List<TextMeshProUGUI> texts;

	private void Awake()
	{
		graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
		dots = new List<GameObject>();
		infectedValues = new List<int>() { 5};
		suseptableValues = new List<int>() { 95 };
		population = 100;
		dead = 0;
		highest = 0;
		hightestDay = 0;
	}

	private void Update()
	{
		ShowGraph(infectedValues, suseptableValues);
	}

	//private GameObject CreateCircle(Vector2 anchoredPosition) {
	//       GameObject gameObject = new GameObject("circle", typeof(Image));
	//       gameObject.transform.SetParent(graphContainer, false);
	//       gameObject.GetComponent<Image>().sprite = circleSprite;
	//       RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
	//       rectTransform.anchoredPosition = anchoredPosition;
	//       rectTransform.sizeDelta = new Vector2(11, 11);
	//       rectTransform.anchorMin = new Vector2(0, 0);
	//       rectTransform.anchorMax = new Vector2(0, 0);
	//       return gameObject;
	//   }

	private GameObject CreateDot(Vector2 anchoredPosition)
	{
		GameObject gameObject = new GameObject("dot", typeof(RectTransform));
		gameObject.transform.SetParent(graphContainer, false);
		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = anchoredPosition;
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);
		return gameObject;
	}

	public void ShowGraph(List<int> iValues, List<int> sValues)
	{
		foreach (GameObject dot in dots)
		{
			Destroy(dot);
		}

		float graphHeight = graphContainer.sizeDelta.y;
		float graphWidth = graphContainer.sizeDelta.x;
		float yMaximum = 100f;

		GameObject lastDotGameObject = null;
		for (int i = 0; i < sValues.Count; i++)
		{
			float xPosition = graphWidth / (sValues.Count - 1) * i;
			float yPosition = ((iValues[i] + sValues[i]) / yMaximum) * graphHeight;
			GameObject circleGameObject = CreateDot(new Vector2(xPosition, yPosition));
			if (lastDotGameObject != null)
			{
				CreateDotConnection(lastDotGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, Color.blue);
			}
			lastDotGameObject = circleGameObject;
			dots.Add(circleGameObject);
		}
		lastDotGameObject = null;
		for (int i = 0; i < iValues.Count; i++)
		{
			float xPosition = graphWidth / (iValues.Count - 1) * i;
			float yPosition = (iValues[i] / yMaximum) * graphHeight;
			GameObject circleGameObject = CreateDot(new Vector2(xPosition, yPosition));
			if (lastDotGameObject != null)
			{
				CreateDotConnection(lastDotGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, Color.red);
			}
			lastDotGameObject = circleGameObject;
			dots.Add(circleGameObject);
		}
		int sus = sValues[sValues.Count - 1];
		int infected = iValues[iValues.Count - 1];
		UpdateData(100 - sus - infected, sus, infected, iValues.Count);
	}

	private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color)
	{
		GameObject gameObject = new GameObject("dotConnection", typeof(Image));
		gameObject.transform.SetParent(graphContainer, false);
		gameObject.GetComponent<Image>().color = color;
		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
		Vector2 dir = (dotPositionB - dotPositionA).normalized;
		float distance = Vector2.Distance(dotPositionA, dotPositionB);
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);
		rectTransform.sizeDelta = new Vector2(distance, 3f);
		rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
		rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
		dots.Add(gameObject);
	}

	private void UpdateData(int rm, int sus, int infected, int day)
	{
		
		texts[0].text =  rm + "% removed";
		texts[1].text =  sus + "% susceptible";
		texts[2].text =  infected + "% infected";

		if(infected > highest)
		{
			highest = infected;
			hightestDay = day;
		}

		texts[3].text = "Highest infection amount: " +  highest + "%";
		texts[4].text = "Highest infection day: Day " + hightestDay;
		texts[5].text = "Estimated Dead: " + infected / 100.0f * population;
	}

	//WORKS-ISH, but is laggy
	//private void CreateAreaConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color)
	//{
	//	GameObject barFill = new GameObject("barFill", typeof(Image));
	//	GameObject triangleFill = new GameObject("triFill", typeof(Image));
	//	barFill.transform.SetParent(graphContainer, false);
	//	barFill.GetComponent<Image>().color = color;
	//	triangleFill.transform.SetParent(graphContainer, false);
	//	triangleFill.GetComponent<Image>().color = color;
	//	float yLevel = dotPositionA.y;
	//	if (dotPositionA.y > dotPositionB.y)
	//	{
	//		yLevel = dotPositionB.y;
	//		triangleFill.GetComponent<Image>().sprite = downSprite;
	//	}
	//	else
	//	{
	//		triangleFill.GetComponent<Image>().sprite = upSprite;
	//	}
	//	float width = dotPositionB.x - dotPositionA.x;
		
	//	RectTransform rectTransform = barFill.GetComponent<RectTransform>();
	//	rectTransform.pivot = new Vector2(0.5f, 1);
	//	rectTransform.anchorMin = new Vector2(0, 0);
	//	rectTransform.anchorMax = new Vector2(0, 0);
	//	rectTransform.sizeDelta = new Vector2(width, yLevel);
	//	rectTransform.anchoredPosition = new Vector2(dotPositionA.x + width / 2.0f, yLevel);

	//	rectTransform = triangleFill.GetComponent<RectTransform>();
	//	rectTransform.pivot = new Vector2(0.5f, 1);
	//	rectTransform.anchorMin = new Vector2(0, 0);
	//	rectTransform.anchorMax = new Vector2(0, 0);
	//	rectTransform.sizeDelta = new Vector2(width, Math.Abs(dotPositionB.y - dotPositionA.y));
	//	rectTransform.anchoredPosition = new Vector2(dotPositionA.x + width / 2.0f, dotPositionA.y);

	//	dots.Add(barFill);
	//}
}
