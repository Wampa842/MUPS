using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModalController : MonoBehaviour
{
    public SkyColorPicker SkyColorPicker;

    public bool AnyOpen { get { return Resources.FindObjectsOfTypeAll<ModalWindow>().Any(w => w.Visible); } }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ColorPicker.Show();
        //}
    }

    public static ModalController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(Instance);
            Instance = this;
        }
    }
}
