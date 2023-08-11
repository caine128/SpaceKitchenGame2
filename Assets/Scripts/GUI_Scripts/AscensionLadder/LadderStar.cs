using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LadderStar : MonoBehaviour
{
    public RectTransform RT => _rt;
    private RectTransform _rt;
    public Vector2 InitialSize => _initialsize;
    private Vector2 _initialsize;
    private GUI_TintScale _gUI_TintScale;
    private TextMeshProUGUI _valueText;
    public int Value { get; private set; }
    private Image _starImage;
    private Color _initialColor;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _valueText = GetComponentInChildren<TextMeshProUGUI>();
        _initialsize = _rt.sizeDelta;
        _starImage = GetComponent<Image>();
        _initialColor = _starImage.color;
        _gUI_TintScale = GetComponent<GUI_TintScale>();
    }


    public void LoadStarContainer(int value_IN, bool isBig, int currentAscensionState)
    {
        Value = value_IN;
        _valueText.text = Value.ToString();
        _rt.sizeDelta = isBig ? _initialsize * 1.5f : _initialsize;
        _starImage.color = SetColor(currentAscensionState);
    }

    public void NudgeStarContainer()
    {
        _starImage.color = Color.white;
        _gUI_TintScale.TintSize();
    }

    private Color SetColor(int currentAscensionState)
        => (Value <= currentAscensionState) switch
        {
            true => Color.white,
            false  => _initialColor,          
        };
}
