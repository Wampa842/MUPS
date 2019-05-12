using System.Globalization;
using UnityEngine;

class SetCulture : MonoBehaviour
{
    private void Awake()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }
}
