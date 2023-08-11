using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class IngredientGenerator
{
    private readonly IngredientType.Type _ingredientType;

    private int tickCount = 0;
    private float maxTickCount;

    public int IngredientGeneratorLevel { get; private set; }
    private bool isSubscribed = false;

    public IngredientGenerator(IngredientType.Type ingredientType_IN, int? ingredientGeneratorLevel_In = null, bool isSubscribed_IN = false) //, bool isActive_IN = false)
    {
        _ingredientType = ingredientType_IN;
        IngredientGeneratorLevel = ingredientGeneratorLevel_In ?? 0;

        var tier = ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.baseInfo[(int)_ingredientType].tier;
        maxTickCount = 60 / (ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.tier[tier].specsByLevel[Mathf.Max(IngredientGeneratorLevel - 1, 0)].productionRate) * 5;
        if (isSubscribed_IN) SetSubscriptionStatus(stopGeneration: false, _ingredientType);
        //SetSubscriptionStatus(ResourcesManager.CheckAmountOfIngredient(ingredientType_In: _ingredientType, out Ingredient ingredient) < ingredient.MaxCap ? false : true, _ingredientType);
    }


    //public void Config(int? ingredientGeneratorLevel_In = null, bool isActive_IN = false)  // Get this to awake or upper beginning
    //{
    //    ingredientGeneratorLevel = ingredientGeneratorLevel_In ?? 1;
    //    isActive = isActive_IN;

    //    var tier = ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.baseInfo[(int)_ingredientType].tier;
    //    maxTickCount = 60 / (ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.tier[tier].specsByLevel[ingredientGeneratorLevel - 1].productionRate) * 5;

    //    if (isActive)
    //    {
    //        TimeTickSystem.onTickTriggered += GenerateIngredient;
    //        ResourcesManager.onCapReached += SetSubscriptionStatus;
    //    }
    //}

    public void Activate() // Maybe this cna go to leveling ??? 
    {
        IngredientGeneratorLevel = 1;
        SetSubscriptionStatus(ResourcesManager.CheckAmountOfIngredient(ingredientType_In: _ingredientType, out Ingredient ingredient) < ingredient.MaxCap ? false : true, _ingredientType);
        ResourcesManager.onCapReached += SetSubscriptionStatus;

    }

    public string GetName()
    {
        return ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.baseInfo[(int)_ingredientType].name;
    }

    public void LevelUp()
    {
        var tier = ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.baseInfo[(int)_ingredientType].tier;

        if (IngredientGeneratorLevel == ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.tier[tier].specsByLevel.Length)
        {
            Debug.Log("Maximum Level Reached");        // It shouldn't be necessary because it shouldn't be pressable to ugrade th item 
            return;
        }
        else
        {
            IngredientGeneratorLevel++;
            maxTickCount = 60 / (ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.tier[tier].specsByLevel[IngredientGeneratorLevel - 1].productionRate) * 5;
        }

        //if (isGemPurchase)
        //{
        //var gemRequired = ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.tier[tier].specsByLevel[IngredientGeneratorLevel - 1].upgradeGemCost;
        //if (StatsData.Gem >= gemRequired)
        // {
        //StatsData.SetGem(-gemRequired);
        //   IngredientGeneratorLevel++;
        //   maxTickCount = 60 / (ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.tier[tier].specsByLevel[IngredientGeneratorLevel - 1].productionRate) * 5;
        //    Debug.Log("succesfull upgraded by gem");
        // }
        // else
        // {
        //    Debug.Log("not enough gem");
        // }
        //  }
        // else
        //{
        // var goldRequired = ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.tier[tier].specsByLevel[IngredientGeneratorLevel - 1].upgradeGoldCost;
        //if (StatsData.Gold >= goldRequired)
        // {
        //  StatsData.SetGold(-goldRequired);
        //IngredientGeneratorLevel++;
        //maxTickCount = 60 / (ResourcesManager.Instance.IngredientGenerators_SO.ingredientGenerators.tier[tier].specsByLevel[IngredientGeneratorLevel - 1].productionRate) * 5;
        //  Debug.Log("succesfull upgraded by gold");
        // }
        //  else
        //  {
        //     Debug.Log("not enough gold");
        // }
        // }
    }

    private void GenerateIngredient(int tickCount_IN, bool isRefillCall)
    {
        tickCount += tickCount_IN;

        if (IngredientDisplayPanel_Manager.Instance.isActiveAndEnabled ||
            MissingRequirementsPopupPanel.Instance.isActiveAndEnabled && MissingRequirementsPopupPanel.Instance.bluePrint is Ingredient ingredient && ingredient.IngredientType == _ingredientType)
        {
            var valueInitial = (float)tickCount / maxTickCount;
            var valueFinal = valueInitial + (float)tickCount_IN / maxTickCount;

            if (IngredientDisplayPanel_Manager.Instance.isActiveAndEnabled) IngredientDisplayPanel_Manager.Instance.SetDisplayContainerBarFill(_ingredientType, valueInitial, valueFinal);
            if (MissingRequirementsPopupPanel.Instance.isActiveAndEnabled && MissingRequirementsPopupPanel.Instance.bluePrint is Ingredient ingredientB && ingredientB.IngredientType == _ingredientType) MissingRequirementsPopupPanel.Instance.UpdateProgressBar(valueInitial, valueFinal);
        }

        if (tickCount >= maxTickCount)
        {
            ResourcesManager.Instance.AddIngredient(ingredientType_IN: _ingredientType, amount_IN: 1, bypassMaxCap:false);
            tickCount = 0;
        }
    }



    public void SetSubscriptionStatus(bool stopGeneration, IngredientType.Type ingredientType_IN)
    {
        //if (IngredientGeneratorLevel < 1) return;
        //else
        //{
        //    if (stopGeneration && _ingredientType == ingredientType_IN)
        //    {
        //        TimeTickSystem.onTickTriggered -= GenerateIngredient;
        //    }
        //    else if (stopGeneration != true && _ingredientType == ingredientType_IN)
        //    {
        //        TimeTickSystem.onTickTriggered += GenerateIngredient;
        //    }
        //}

        switch (IngredientGeneratorLevel, isSubscribed, ingredientType_IN == _ingredientType, stopGeneration)
        {

            case ( < 1, _, _, _):         
            case ( > 0, true, true, true):
                TimeTickSystem.onTickTriggered -= GenerateIngredient;
                isSubscribed = false;
                break;
            case ( > 0, false, true, false):
                TimeTickSystem.onTickTriggered += GenerateIngredient;
                isSubscribed = true;
                break;
            case (_, _, false, _):
            case ( > 0, false, true, true):
            case ( > 0, true, true, false):
            default:
                break;
        }
    }
}
