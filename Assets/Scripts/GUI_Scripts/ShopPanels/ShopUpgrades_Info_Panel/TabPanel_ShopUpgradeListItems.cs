using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabPanel_ShopUpgradeListItems : TabPanel_Animated<Tab.ShopUpgradeInfoTabs>
{
    public override Tab.ShopUpgradeInfoTabs TabType => _tabType;
    [SerializeField] private Tab.ShopUpgradeInfoTabs _tabType;
    [SerializeField] private ShopUpgradesListPanel_Manager _shopUpgradesListPanel_Manager;

    public sealed override void LoadInfo()
    {
        _shopUpgradesListPanel_Manager.LoadContainers();
    }

    public sealed override void DisplayContainers()
    {
        _shopUpgradesListPanel_Manager.DisplayContainers();
    }

    public sealed override void HideContainers()
    {
        for (int i = 0; i < _shopUpgradesListPanel_Manager.ContainersList.Count; i++)
        {
            _shopUpgradesListPanel_Manager.ContainersList[i].ScaleDirect(isVisible: false);
        }
    }

    public sealed override void UnloadInfo()
    {
        for (int i = 0; i < _shopUpgradesListPanel_Manager.ContainersList.Count; i++)
        {
            _shopUpgradesListPanel_Manager.ContainersList[i].UnloadContainer();
            if (_shopUpgradesListPanel_Manager.ContainersList[i].gameObject.activeSelf != false)
            {
                _shopUpgradesListPanel_Manager.ContainersList[i].gameObject.SetActive(false);
            }
        }
    }
}
