using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabPanel_CraftUpgrade : TabPanel_Animated<Tab.RecipeInfoTabs>
{
    [SerializeField] private ContentDisplayCraftUpgrades[] craftUpgradesContentDisplays;

    public override Tab.RecipeInfoTabs TabType { get { return _tabType; } }
    [SerializeField] private Tab.RecipeInfoTabs _tabType;

    private void Awake()
    {
        co = new IEnumerator[1];
    }

    public override void LoadInfo()
    {
        craftUpgradesContentDisplays.LoadContainers(RecipeInfoPanel_Manager.Instance.SelectedRecipe, craftUpgradesContentDisplays.Length, hideAtInit: true);
    }
    public override void HideContainers()
    {
        craftUpgradesContentDisplays.HideContainers();
    }

    public override void DisplayContainers()
    {
        craftUpgradesContentDisplays.SortContainers(customInitialValues:null,
                                                    secondaryInterpolations:null,
                                                    amountToSort_IN: craftUpgradesContentDisplays.Length, 
                                                    enumeratorIndex: 0,
                                                    parentPanel_IN: this,
                                                    lerpSpeedModifiers: null);
        //SortContainers(craftUpgradesContentDisplays, craftUpgradesContentDisplays.Length, enumeratorIndex: 0);
    }

    public override void UnloadInfo()
    {
        GUI_CentralPlacement.DeactivateUnusedContainers(0, craftUpgradesContentDisplays);
        base.UnloadInfo();
    }


}
