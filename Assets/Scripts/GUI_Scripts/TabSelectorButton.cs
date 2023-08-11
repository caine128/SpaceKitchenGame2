using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class TabSelectorButton<T_TabType> : MonoBehaviour, IPointerDownHandler
    where T_TabType:System.Enum
{
    public T_TabType TabType { get { return _tabType; }}
    [SerializeField] protected T_TabType _tabType;

    [SerializeField] protected Image buttonBG;
    [SerializeField] protected TextMeshProUGUI buttonText;


    public abstract void OnPointerDown(PointerEventData eventData);

    public void SetbuttonColor(Color colorBG, Color colorTxt)
    {
        buttonBG.color = colorBG;
        buttonText.color = colorTxt;
    }


}
