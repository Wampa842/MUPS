using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ModalWindow : MonoBehaviour
{
    public GameObject Background;
    public bool Visible = false;
    public Button OkButton;
    public Button CancelButton;

    public void Show()
    {
        gameObject.SetActive(true);
        Background.SetActive(true);
        Visible = true;
    }

    public void Hide()
    {
        Visible = false;
        gameObject.SetActive(Visible);
        Background.SetActive(ModalController.Instance.AnyOpen);
    }

    private void Update()
    {
        if((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && OkButton != null)
        {
            OkButton.onClick.Invoke();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && CancelButton != null)
        {
            CancelButton.onClick.Invoke();
        }
    }
}
