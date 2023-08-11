using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AscensionMarker : MonoBehaviour
{
    private RectTransform _rt;
    private GUI_LerpMethods_Scale _gUI_LerpMethods_Scale;
    private TextMeshProUGUI _content;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _gUI_LerpMethods_Scale = GetComponent<GUI_LerpMethods_Scale>();
        _content = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void DisableMarker()
    {
        if(_gUI_LerpMethods_Scale.RunningCoroutine is not null) _gUI_LerpMethods_Scale.StopRescale();
        _gUI_LerpMethods_Scale.RescaleDirect(finalScale: Vector2.zero, finalValueOperations: null);
    }
    public void SetMarkerPosAndValue(float newPosX, int newValue)
    {
        _content.text = newValue.ToString();
        _rt.anchoredPosition = newPosX != _rt.anchoredPosition.x
                                   ? new Vector2(newPosX, _rt.anchoredPosition.y)
                                   : _rt.anchoredPosition;     
    }
    public void EnableMarker()
    {
        _gUI_LerpMethods_Scale.Rescale(customInitialValue: null, secondaryInterpolation: null, finalScale: Vector2.one, lerpSpeedModifier: 2f);
    }
}
