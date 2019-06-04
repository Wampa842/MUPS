using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MUPS;

public class LightColorPicker : MonoBehaviour
{
    public Slider HueSlider;
    public Slider SatSlider;
    public Slider ValSlider;
    public InputField IntensityField;
    public InputField HtmlField;
    public Dropdown LightTypeSelect;
    public Slider AngleSlider;
    public InputField RangeField;
    public Slider ShadowStrengthSlider;
    public InputField LightName;

    public Image HueHandle;
    public Image SatHandle;
    public Image ValHandle;

    public ModalWindow Window;

    private SceneLight _light;
    private Color _origColor;
    private float _origIntensity;
    private float _origAngle;
    private float _origRange;
    private LightType _origType;
    private float _origShadowStrength;

    private bool _preventHtmlUpdate = false;

    public void UpdateLight()
    {
        float h = HueSlider.value;
        float s = SatSlider.value;
        float v = ValSlider.value;
        _light.Light.color = Color.HSVToRGB(h, s, v);
        _light.Light.spotAngle = AngleSlider.value;
        if (float.TryParse(IntensityField.text, out float intensity))
            _light.Light.intensity = intensity;
        if (float.TryParse(RangeField.text, out float range))
            _light.Light.range = range;
        _light.Light.shadowStrength = ShadowStrengthSlider.value;

        _preventHtmlUpdate = true;
        HtmlField.text = _light.Light.color.ToHtml();

        switch (LightTypeSelect.value)
        {
            case 0:
                _light.Light.type = LightType.Point;
                break;
            case 1:
                _light.Light.type = LightType.Spot;
                break;
            case 2:
                _light.Light.type = LightType.Directional;
                break;
        }

        UpdatePreview(h, s, v);
    }

    public void UpdateLight(string html)
    {
        if (_preventHtmlUpdate)
        {
            _preventHtmlUpdate = false;
            return;
        }
        if (!Utility.ParseHtmlColor(html, out Color color))
            return;
        if (!(float.TryParse(IntensityField.text, out float intensity) && float.TryParse(RangeField.text, out float range)))
            return;

        Color.RGBToHSV(color, out float h, out float s, out float v);
        HueSlider.value = h;
        SatSlider.value = s;
        ValSlider.value = v;

        _light.Light.intensity = intensity;
        _light.Light.spotAngle = range;

        switch (LightTypeSelect.value)
        {
            case 0:
                _light.Light.type = LightType.Point;
                break;
            case 1:
                _light.Light.type = LightType.Spot;
                break;
            case 2:
                _light.Light.type = LightType.Directional;
                break;
        }

        UpdatePreview(h, s, v);
    }

    public void RevertLight()
    {
        _light.Light.color = _origColor;
        _light.Light.intensity = _origIntensity;
        _light.Light.spotAngle = _origAngle;
        _light.Light.range = _origRange;
        _light.Light.type = _origType;
        _light.Light.shadowStrength = _origShadowStrength;
    }

    public void UpdateLightType(int type)
    {
        AngleSlider.gameObject.SetActive(type == 1);
        RangeField.gameObject.SetActive(type != 2);
        UpdateLight();
    }

    public void UpdatePreview(float h, float s, float v)
    {
        //HueHandle.color = Color.HSVToRGB(HueSlider.value, 1, 1);
        //SatHandle.color = Color.HSVToRGB(HueSlider.value, SatSlider.value, 1);
        //ValHandle.color = Color.HSVToRGB(1, 0, ValSlider.value);
        HueHandle.color = Color.HSVToRGB(h, 1, 1);
        SatHandle.color = Color.HSVToRGB(h, s, 1);
        ValHandle.color = Color.HSVToRGB(0, 0, v);
    }

    public void Show(SceneLight light)
    {
        _light = light;
        _origColor = light.Light.color;
        _origIntensity = light.Light.intensity;
        _origAngle = light.Light.spotAngle;
        _origRange = light.Light.range;
        _origType = light.Light.type;
        _origShadowStrength = light.Light.shadowStrength;

        LightName.text = _light.DisplayName;
        Color.RGBToHSV(light.Light.color, out float h, out float s, out float v);
        HueSlider.value = h;
        SatSlider.value = s;
        ValSlider.value = v;
        IntensityField.text = light.Light.intensity.ToString("f3");
        AngleSlider.value = light.Light.spotAngle;
        RangeField.text = light.Light.range.ToString("f3");

        switch (light.Light.type)
        {
            case LightType.Spot:
                LightTypeSelect.value = 1;
                break;
            case LightType.Directional:
                LightTypeSelect.value = 2;
                break;
            default:
                LightTypeSelect.value = 0;
                break;
        }

        UpdateLightType(LightTypeSelect.value);

        _preventHtmlUpdate = true;
        HtmlField.text = _light.Light.color.ToHtml();

        Window.Show();
    }

    public void Close(bool apply)
    {
        if (apply)
        {
            _light.DisplayName = LightName.text;
        }
        else
        {
            RevertLight();
        }

        Window.Hide();
    }
}
