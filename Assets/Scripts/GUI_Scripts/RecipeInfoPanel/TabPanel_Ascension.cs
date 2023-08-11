using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabPanel_Ascension : TabPanel_Animated<Tab.RecipeInfoTabs>
{
    [SerializeField] private ContentDisplayAscensionUpgrades[] ascensionUpgradesContentDisplays;
    [SerializeField] private AscensionLadderSelector_Button AscensionLadderSelector_Button;

    public override Tab.RecipeInfoTabs TabType { get { return _tabType; } }
    [SerializeField] private Tab.RecipeInfoTabs _tabType;

    private void Awake()
    {
        co = new IEnumerator[1];
    }

    public override void LoadInfo()
    {
        var selectedRecipe = RecipeInfoPanel_Manager.Instance.SelectedRecipe;

        AscensionLadderSelector_Button.AssignCriteria(selectedRecipe.recipeSpecs.productType);
        ascensionUpgradesContentDisplays.LoadContainers(selectedRecipe, ascensionUpgradesContentDisplays.Length, hideAtInit: true);

    }
    public override void HideContainers()
    {
        ascensionUpgradesContentDisplays.HideContainers();
    }

    public override void DisplayContainers()
    {
        ascensionUpgradesContentDisplays.SortContainers(customInitialValues:null,
                                                        secondaryInterpolations: null,
                                                        amountToSort_IN: ascensionUpgradesContentDisplays.Length, 
                                                        enumeratorIndex: 0,
                                                        parentPanel_IN: this,
                                                        lerpSpeedModifiers: null);     
    }

    public override void UnloadInfo()
    {
        for (int i = 0; i < ascensionUpgradesContentDisplays.Length; i++)
        {
            ascensionUpgradesContentDisplays[i].Unload();
        }
    }


}
