using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabPanel_RecipesAvailable : TabPanel_Animated<Tab.CharacterInfoTabs>
{
    public override Tab.CharacterInfoTabs TabType => _tabType;
    [SerializeField] private Tab.CharacterInfoTabs _tabType;
    [SerializeField] private RecipesAvailablePanel_Manager _recipesAvailablePanel_Manager;

    public override void LoadInfo()
    {
        _recipesAvailablePanel_Manager.LoadContainers();
    }
    public override void DisplayContainers()
    {
        _recipesAvailablePanel_Manager.DisplayContainers();
    }
    public override void HideContainers()
    {
        for (int i = 0; i < _recipesAvailablePanel_Manager.ContainersList.Count; i++)
        {
            _recipesAvailablePanel_Manager.ContainersList[i].ScaleDirect(isVisible: false);
        }
    }


    public override void UnloadInfo()
    {
        for (int i = 0; i < _recipesAvailablePanel_Manager.ContainersList.Count; i++)
        {
            _recipesAvailablePanel_Manager.ContainersList[i].UnloadContainer();
            if (_recipesAvailablePanel_Manager.ContainersList[i].gameObject.activeSelf != false) 
            {
                _recipesAvailablePanel_Manager.ContainersList[i].gameObject.SetActive(false);
            } 
        }
    }




}
