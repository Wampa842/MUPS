using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MUPS
{
    public class SkyLightController : MonoBehaviour
    {
        public static SkyLightController Instance { get; private set; }

        public Light Skylight;
        public Slider AzimuthSlider;
        public Slider ElevationSlider;
        public Dropdown SunShadowSelect;
        public Dropdown LightShadowSelect;

        public Image SkylightPreview;
        public Image AmbientPreview;

        public void UpdateLightPosition()
        {
            Skylight.transform.rotation = Quaternion.Euler(ElevationSlider.value, -AzimuthSlider.value, 0);
        }

        public void SelectSkyColor()
        {
            ModalController.Instance.SkyColorPicker.Show(Skylight.color, Skylight.intensity, (c, i) => { SetSkylight(c, i); }, (c, i) => { });
        }

        public void SelectAmbientColor()
        {
            Color color = RenderSettings.ambientSkyColor;
            ModalController.Instance.SkyColorPicker.Show(color, 1, (c, i) => { SetAmbientLight(c, i); }, (c, i) => { });
        }

        public void SetSkylight(Color color, float intensity)
        {
            Skylight.color = color;
            Skylight.intensity = intensity;
            SkylightPreview.color = color;
        }

        public void SetAmbientLight(Color color, float intensity)
        {
            RenderSettings.ambientLight = color;
            AmbientPreview.color = color;
        }

        public void UpdateSunShadows(int val)
        {
            switch(val)
            {
                case 0:
                    Skylight.shadows = LightShadows.Soft;
                    break;
                case 1:
                    Skylight.shadows = LightShadows.Hard;
                    break;
                default:
                    Skylight.shadows = LightShadows.None;
                    break;
            }
        }

        public void UpdateLightShadows()
        {

        }

        private void Start()
        {
            AzimuthSlider.value = -45;
            ElevationSlider.value = 45;

            SetSkylight(Color.white, 1);
            SetAmbientLight(new Color(0.3f, 0.3f, 0.3f, 1), 1);
        }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else if(Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }
        }
    }
}