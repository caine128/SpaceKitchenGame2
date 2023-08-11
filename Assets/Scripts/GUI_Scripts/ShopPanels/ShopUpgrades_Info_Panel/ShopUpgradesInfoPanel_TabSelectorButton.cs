
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgradesInfoPanel_TabSelectorButton : TabSelectorButton<Tab.ShopUpgradeInfoTabs>
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        ShopUpgradesInfoPanel_Manager.Instance.PlaceTabPanel(_tabType, this);
    }
}
