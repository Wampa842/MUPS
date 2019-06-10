using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
class TogglePanel : MonoBehaviour
{
    public Button ToggleButton;
    public GameObject Content;
    public bool Visible;

    public float WidthClosed, WidthOpen;

    public void ToggleVisibility()
    {
        Visible = !Visible;
        SetVisibility(Visible);
    }

    public void SetVisibility(bool visible)
    {
        Content.SetActive(visible);
        ((RectTransform)transform).sizeDelta = new Vector2(Visible ? WidthOpen : WidthClosed, ((RectTransform)transform).sizeDelta.y);
        LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform.parent);
    }

    private void OnValidate()
    {
        SetVisibility(Visible);
    }

    private void Start()
    {
        ToggleButton.onClick.AddListener(ToggleVisibility);
        LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform.parent);
    }
}