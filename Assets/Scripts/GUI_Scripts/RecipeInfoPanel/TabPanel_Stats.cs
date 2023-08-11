using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TabPanel_Stats : TabPanel<Tab.RecipeInfoTabs>
{
    [SerializeField] private TextMeshProUGUI craftedAmount;
    [SerializeField] private TextMeshProUGUI soldAmount;
    [SerializeField] private TextMeshProUGUI unlockedTime;
    [SerializeField] private TextMeshProUGUI lastCraftedTime;
    [SerializeField] private TextMeshProUGUI ascensionLevelDegree;
    [SerializeField] private TextMeshProUGUI masteryLevelDegree;

    private StringBuilder sb = new StringBuilder();
    private readonly string[] headers = new string[] { "Crafted : ", "Sold : ", "Unlocked : ", "Last Crafted : ", "Acension : ", "Mastery : " };

    public override Tab.RecipeInfoTabs TabType { get { return _tabType; } }
    [SerializeField] private Tab.RecipeInfoTabs _tabType;

    public override void LoadInfo()
    {
        craftedAmount.text = BuildString(headers[0],RecipeInfoPanel_Manager.Instance.SelectedRecipe.amountCraftedGlobal.ToString());
        soldAmount.text = BuildString(headers[1],RecipeInfoPanel_Manager.Instance.SelectedRecipe.goldGenerated.ToString());
        unlockedTime.text = BuildString(headers[2], RecipeInfoPanel_Manager.Instance.SelectedRecipe.dateUnlocked.ToString("dd/MM/yyyy"));
        lastCraftedTime.text = BuildString(headers[3],RecipeInfoPanel_Manager.Instance.SelectedRecipe.DateLastCrafted == DateTime.MinValue ? "N/A" : RecipeInfoPanel_Manager.Instance.SelectedRecipe.DateLastCrafted.ToString("T"));
        ascensionLevelDegree.text = BuildString(headers[4], RecipeInfoPanel_Manager.Instance.SelectedRecipe.ascensionLevel.ToString());
        masteryLevelDegree.text = BuildString(headers[5], RecipeInfoPanel_Manager.Instance.SelectedRecipe.masteryLevel.ToString());


        /*Debug.LogWarning(RecipeInfoPanel_Manager.Instance.selectedRecipe.GetMultiCraftChanceModifier() + "multicraftchance");
        Debug.LogWarning("qualitychancemodifiers");
        foreach (var item in RecipeInfoPanel_Manager.Instance.selectedRecipe.GetQualityModifers())
        {
            Debug.LogWarning(item);
        }  */     
    }

    private string BuildString(string strName, string strValue)
    {
        sb.Clear();
        sb.Append(strName);
        sb.Append(strValue);

        return sb.ToString();
    }

    public override void UnloadInfo()
    {
        craftedAmount.text = soldAmount.text = unlockedTime.text = lastCraftedTime.text = ascensionLevelDegree.text = masteryLevelDegree.text = string.Empty;
    }


}
