using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Text_Color_Setter : MonoBehaviour
{
    [SerializeField] private Color maxColor;
    [SerializeField] private Color minColor;
    [SerializeField] private Color defaultColor;

    [SerializeField] private TextMeshProUGUI textMain;
    [SerializeField] private TextMeshProUGUI textToCompare;

    private Color textToColor;

    private void Awake()
    {
        textToColor = GetComponent<TextMeshProUGUI>().color;
    }

    public void SetColor()
    {
        if (int.Parse(textMain.text) >= int.Parse(textToCompare.text)-1) // -1 Because the last stat is not set yet when it's checking for the color  change 
        {
            textMain.color = maxColor;
        }
        else if (int.Parse(textMain.text) <= int.Parse(textToCompare.text) / 7)
        {
            textMain.color = minColor;
        }
        else
        {
            textMain.color = defaultColor;
        }
    }
}
