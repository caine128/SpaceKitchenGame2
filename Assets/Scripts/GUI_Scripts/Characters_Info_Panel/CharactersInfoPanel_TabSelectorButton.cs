using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharactersInfoPanel_TabSelectorButton : TabSelectorButton<Tab.CharacterInfoTabs>
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        CharactersInfoPanel_Manager.Instance.PlaceTabPanel(_tabType, this);
    }
}
