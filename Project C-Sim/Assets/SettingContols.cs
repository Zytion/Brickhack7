using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingContols : MonoBehaviour
{
	private TMP_InputField inputField;
	private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
		inputField = gameObject.GetComponentInChildren<TMP_InputField>();
		slider = gameObject.GetComponentInChildren<Slider>();
	}

	public void UpdateSlider()
	{
		if(slider.wholeNumbers)
		{
			slider.value = int.Parse(inputField.text);
		}
		else
		{
			slider.value = float.Parse(inputField.text);
		}
	}

	public void UpdateInput()
	{
		if (slider.wholeNumbers)
		{
			inputField.text = ((int)slider.value).ToString();
		}
		else
		{
			inputField.text = (Mathf.Round(slider.value * 100f) / 100f).ToString();
		}
	}

}
