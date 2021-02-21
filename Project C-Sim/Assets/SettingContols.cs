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
		slider.value = int.Parse(inputField.text);
	}

	public void UpdateInput()
	{
		inputField.text = ((int)slider.value).ToString();
	}

}
