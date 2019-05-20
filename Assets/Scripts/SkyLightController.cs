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

        public void UpdatePreviews()
        {
            SkylightPreview.color = Skylight.color;
            AmbientPreview.color = RenderSettings.ambientLight;
        }

        public void SelectSkyColor()
        {
            ModalController.Instance.SkyColorPicker.Show();
        }

        public void SelectAmbientColor()
        {
            ModalController.Instance.SkyColorPicker.Show();
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

        public void UpdateLightShadows(int val)
        {
            LightShadows mode;
            switch (val)
            {
                case 0:
                    mode = LightShadows.Soft;
                    break;
                case 1:
                    mode = LightShadows.Hard;
                    break;
                default:
                    mode = LightShadows.None;
                    break;
            }

            foreach (SceneLight light in Resources.FindObjectsOfTypeAll<SceneLight>())
            {
                light.Light.shadows = mode;
            }
        }

        private void Start()
        {
            AzimuthSlider.value = -45;
            ElevationSlider.value = 45;

            Skylight.color = Color.white;
            Skylight.intensity = 1;
            RenderSettings.ambientLight = new Color(0.3f, 0.3f, 0.3f, 1);
            UpdatePreviews();
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