using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownButtons : MonoBehaviour
{
    public bool CloseOnActivate = true;
    public GameObject Panel;
    public Button Button;

    private void Start()
    {
        Button.onClick.AddListener(TogglePanel);
        foreach(Button button in Panel.GetComponentsInChildren<Button>())
        {
            button.onClick.AddListener(() =>
            {
                if(CloseOnActivate)
                {
                    ClosePanel();
                }
            });
        }
    }

    public void ClosePanel()
    {
        Panel.SetActive(false);
    }

    public void TogglePanel()
    {
        Panel.SetActive(!Panel.activeSelf);
    }
}