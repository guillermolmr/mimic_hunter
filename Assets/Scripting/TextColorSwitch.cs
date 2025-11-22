using TMPro;
using UnityEngine;

public class TextColorSwitch : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField]
    Color colorDefault;
    [SerializeField]
    Color colorHover;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }


    public void SetColorDefault()
    {
        text.color = colorDefault;
    }

    public void SetColorHover()
    {
        text.color = colorHover;
    }

}
