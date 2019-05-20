using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SkyColorPicker : MonoBehaviour
{
    public Slider SHSlider, SSSlider, SVSlider;
    public Slider AHSlider, ASSlider, AVSlider;
    public Image SHImage, SSImage, SVImage;
    public Image AHImage, ASImage, AVImage;
    public InputField SIField, AIField;
    public Slider SpecularSlider;
    public Light Skylight;

    public ModalWindow Window;

    private Color _origAColor;
    private Color _origSColor;
    private float _origSIntensity;
    private float _origSpecular;

    public void UpdateSliderColors()
    {
        SHImage.color = Color.HSVToRGB(SHSlider.value, 1, 1);
        SSImage.color = Color.HSVToRGB(SHSlider.value, SSSlider.value, 1);
        SVImage.color = Color.HSVToRGB(SHSlider.value, SSSlider.value, SVSlider.value);
        AHImage.color = Color.HSVToRGB(AHSlider.value, 1, 1);
        ASImage.color = Color.HSVToRGB(AHSlider.value, ASSlider.value, 1);
        AVImage.color = Color.HSVToRGB(AHSlider.value, ASSlider.value, AVSlider.value);
    }


    public void UpdateColor()
    {
        UpdateSliderColors();

        if (float.TryParse(SIField.text, out float si))
            Skylight.intensity = si;
        Skylight.color = Color.HSVToRGB(SHSlider.value, SSSlider.value, SVSlider.value);

        if(float.TryParse(AIField.text, out float ai))
            RenderSettings.ambientLight = Color.HSVToRGB(AHSlider.value, ASSlider.value, AVSlider.value) * ai;

        RenderSettings.reflectionIntensity = SpecularSlider.value;
    }

    public void RevertColor()
    {
        RenderSettings.ambientLight = _origAColor;
        Skylight.color = _origSColor;
        Skylight.intensity = _origSIntensity;
        RenderSettings.reflectionIntensity = _origSpecular;
    }

    public void Show()
    {
        _origAColor = RenderSettings.ambientLight;
        _origSColor = Skylight.color;
        _origSIntensity = Skylight.intensity;
        _origSpecular = RenderSettings.reflectionIntensity;

        Color.RGBToHSV(_origSColor, out float sh, out float ss, out float sv);
        SHSlider.value = sh;
        SSSlider.value = ss;
        SVSlider.value = sv;
        SIField.text = _origSIntensity.ToString("f4");

        float ai = Mathf.Max(_origAColor.maxColorComponent * 2, Mathf.Epsilon);
        Color.RGBToHSV(_origAColor / ai, out float ah, out float @as, out float av);
        AHSlider.value = ah;
        ASSlider.value = @as;
        AVSlider.value = av;
        AIField.text = ai.ToString("f4");

        SpecularSlider.value = _origSpecular;

        Window.Show();
        UpdateSliderColors();
    }

    public void Close(bool apply)
    {
        if (apply && !(float.TryParse(SIField.text, out float si) && float.TryParse(AIField.text, out float ai)))
            return;

        if(!apply)
        {
            RevertColor();
        }

        MUPS.SkyLightController.Instance.UpdatePreviews();
        Window.Hide();
    }
}
