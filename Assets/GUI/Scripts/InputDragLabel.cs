using UnityEngine;
using UnityEngine.UI;

class InputDragLabel : MonoBehaviour
{
    public bool HasMinimum = false;
    public bool HasMaximum = false;
    public float Minimum = 0;
    public float Maximum = 0;
    public float Speed = 0.01f;

    public InputField Field;

    private Vector3 _prev;

    public void InitializeDrag()
    {
        _prev = Input.mousePosition;
    }

    public void UpdateValue()
    {
        float value = 0;
        float.TryParse(Field.text, out value);

        value += Speed * (Input.mousePosition - _prev).x;
        _prev = Input.mousePosition;
        if (HasMinimum)
            value = Mathf.Max(Minimum, value);
        if (HasMaximum)
            value = Mathf.Min(Maximum, value);

        Field.text = value.ToString("f4");
    }
}
