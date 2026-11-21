using UnityEngine;
using UnityEngine.UI;

public class AreaHud : MonoBehaviour
{
    [SerializeField] private Text areaText;

    public void SetArea(string name)
    {
        if (areaText != null)
            areaText.text = name;
    }
}