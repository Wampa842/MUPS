using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TabControl : MonoBehaviour
{
    public Button[] Buttons;
    public GameObject[] Panels;

    private Text[] _textElements;

    private void Start()
    {
        Debug.Assert(Buttons.Length == Panels.Length, "The number of buttons and tabs must be equal.");

        _textElements = new Text[Buttons.Length];

        for (int i = 0; i < Buttons.Length; ++i)
        {
            int num = i;    // I don't know why this is necessary, but apparently, it do be like that sometimes.
            Buttons[i].onClick.AddListener(() => { ShowTab(num); });
            _textElements[i] = Buttons[i].GetComponentInChildren<Text>();
        }

        if (Buttons.Length > 0)
            ShowTab(0);
    }

    private void ShowTab(int number)
    {
        for (int i = 0; i < Panels.Length; ++i)
        {
            Panels[i].SetActive(i == number);
            _textElements[i].fontStyle = i == number ? FontStyle.Bold : FontStyle.Normal;
        }
    }
}
