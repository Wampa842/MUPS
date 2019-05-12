using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderInputSync : MonoBehaviour
{
    public Slider slider;
    public InputField inputField;

    public void UpdateSlider()
    {
        float val;
        if(float.TryParse(inputField.text, out val))
        {
            slider.value = val;
        }
    }

    public void UpdateInput()
    {
        inputField.text = slider.value.ToString("0.###");
    }
}
