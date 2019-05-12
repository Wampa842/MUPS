using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SkyColorPicker : MonoBehaviour
{
    public Slider HueSlider;
    public Slider SatSlider;
    public Slider ValSlider;
    public InputField IntensityField;

    public Image HueHandle;
    public Image SatHandle;
    public Image ValHandle;
    public Image PreviewImage;
    public Material SatValPreview;

    public Color Color;
    public float Intensity = 1.0f;
    public UnityAction<Color, float> OkAction;
    public UnityAction<Color, float> CancelAction;

    public ModalWindow Window;

    public void UpdateIntensity()
    {
        float.TryParse(IntensityField.text, out Intensity);
    }

    public void UpdateColor()
    {
        Color = Color.HSVToRGB(HueSlider.value, SatSlider.value, ValSlider.value);
        PreviewImage.color = Color;
        SatValPreview.SetFloat("_Hue", HueSlider.value);
        HueHandle.color = Color.HSVToRGB(HueSlider.value, 1, 1);
        SatHandle.color = Color.HSVToRGB(HueSlider.value, SatSlider.value, 1);
        float v = ValSlider.value;
        ValHandle.color = new Color(v, v, v, 1);
    }

    public void Show(Color color, float intensity, UnityAction<Color, float> okAction, UnityAction<Color, float> cancelAction)
    {
        Color = color;
        Intensity = intensity;
        OkAction = okAction;
        CancelAction = cancelAction;

        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        HueSlider.value = h;
        SatSlider.value = s;
        ValSlider.value = v;
        IntensityField.text = intensity.ToString("0.###");

        Window.Show();
    }

    public void Close(bool apply)
    {
        Debug.Log("text" + IntensityField.text);
        if(apply && OkAction != null)
        {
            OkAction.Invoke(Color, Intensity);
        }
        else if(CancelAction != null)
        {
            CancelAction.Invoke(Color, Intensity);
        }

        OkAction = null;
        CancelAction = null;

        Window.Hide();
    }
}
