/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class WindowGraph : MonoBehaviour {

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
	//public bool showCircles = true;
	public List<int> infectedValues;
	public List<int> suseptableValues;
	private List<GameObject> dots;

	private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
		dots = new List<GameObject>();
		infectedValues = new List<int>() { 5, 80, 56, 45, 30, 22, 17, 15, 13, 10, 10, 10, 7, 7, 3 };
		suseptableValues = new List<int>() { 95, 20, 20, 20, 20, 18, 17, 15, 13, 13, 13, 13, 10, 10, 10};
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

	private void ShowGraph(List<int> iValues, List<int> sValues) {
		foreach(GameObject dot in dots)
		{
			Destroy(dot);
		}

		float graphHeight = graphContainer.sizeDelta.y;
		float graphWidth = graphContainer.sizeDelta.x;
		float yMaximum = 100f;

        GameObject lastDotGameObject = null;
        for (int i = 0; i < iValues.Count; i++) {
            float xPosition =  graphWidth / (iValues.Count - 1) * i;
            float yPosition = (iValues[i] / yMaximum) * graphHeight;
			GameObject circleGameObject = CreateDot(new Vector2(xPosition, yPosition));
			if (lastDotGameObject != null) {
                CreateDotConnection(lastDotGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition, Color.red);
            }
            lastDotGameObject = circleGameObject;
			dots.Add(circleGameObject);
		}
		lastDotGameObject = null;
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
	}

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color) {
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

}
