using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class Enhance
{
    public static System.Random random = new System.Random(); // can be marked as static in the gamemanager ! to acces from everywhere 
    public static int EnhanceSuccessRatio(IEnhanceable enhanceable, Enhancement enhancement)
    => ((int)enhanceable.GetQuality() - (int)enhancement.GetQuality(), enhancement.GetLevel()) switch
    {
        (-5, _) => 100,
        (-4, _) => 100,
        (-3, _) => 100,
        (-2, _) => 100,
        (-1, _) => 100,
        (0, _) => 100,
        (1, 4) => 33,
        (1, 7) => 25,
        (1, 9) => 20,
        (2, 4) => 15,
        (2, 7) => 12,
        (2, 9) => 10,
        (3, 4) => 8,
        (3, 7) => 6,
        (3, 9) => 5,
        (4, 4) => 3,
        (4, 7) => 2,
        (4, 9) => 1,
        (5, 4) => 0,
        (5, 7) => 0,
        (5, 9) => 0,
        _ => 0,
    };

    public static bool EnhanceAttempt(IEnhanceable enhanceable, Enhancement enhancement)
    {
        if (Inventory.Instance.RemoveFromInventory(enhancement, amount: 1))
        {
            int rnd = random.Next(0, 100);

            if (rnd <= EnhanceSuccessRatio(enhanceable, enhancement))
            {
                Debug.Log("successful Enhancement !!");
                return true;
            }
        }
        Debug.Log("unsuccesful Enhancement =(");
        return false;
    }

    public static int GetMaxValueIncrease(IEnhanceable enhanceable, Enhancement enhancement)
    {
        var enhanceableBaseValue = enhanceable.GetProductRecipe().recipeSpecs.productValue;
        var enhancementValue = enhanceable.GetQuality() == enhancement.GetQuality() 
                                    ? enhancement.GetValue()
                                    : Mathf.CeilToInt(enhancement.GetProductRecipe().GetValue() * Quality.ValueModifierPerQuality(enhanceable.GetQuality()));
        int totalValueIncrease = enhanceableBaseValue <= enhancementValue ? enhanceableBaseValue : enhancementValue;

        return totalValueIncrease;

    }


    public static IEnumerable<Recipes_SO.MealStatBonus> GetRelevantEnhancementBonuses(IEnhanceable enhanceable_IN, Enhancement enhancement_IN)
    {
        var itemStats = enhanceable_IN.GetProductRecipe().recipeSpecs.mealStatBonuses;
        //var itemStats = enhanceable_IN.GetStatBonuses().ToArray();
        //var enhancementStats = enhancement_IN.GetProductRecipe().recipeSpecs.mealStatBonuses;
        var enhancementStats = enhancement_IN.GetStatBonuses().ToArray();

        if (enhancementStats is not null && itemStats is not null)
        {
            for (int i = 0; i < itemStats.Length; i++)
            {
                for (int j = 0; j < enhancementStats.Length; j++)
                {
                    if (itemStats[i].statType == enhancementStats[j].statType)
                        //yield return enhancementStats[i];

                        yield return Enum.Equals(enhanceable_IN.GetQuality(), enhancement_IN.GetQuality())
                                                    ? enhancementStats[j]
                                                    : new Recipes_SO.MealStatBonus()
                                                    {
                                                        statType = enhancementStats[j].statType,
                                                        statBonus = Mathf.CeilToInt(
                                                            enhancement_IN.GetProductRecipe().recipeSpecs.mealStatBonuses
                                                                        .Where(sb => Enum.Equals(sb.statType, enhancementStats[j].statType))
                                                                        .First().statBonus * Quality.StatModifierPerQuality(enhanceable_IN.GetQuality()))
                                                    };
                }
            }
        }
    }

    public static Recipes_SO.MealStatBonus GetStatBonusByStatType(Enhancement enhancement_IN, Recipes_SO.MealStatType statType_IN)
    {
        //var statBonunsesArray = enhancement_IN.GetProductRecipe().recipeSpecs.mealStatBonuses;

        foreach (var statBonus in enhancement_IN.GetStatBonuses())
        {
            if (Enum.Equals(statBonus.statType, statType_IN))
                return statBonus;
        }
        throw new NotImplementedException();
        /*for (int i = 0; i < statBonunsesArray.Length; i++)
        {
            if (statBonunsesArray[i].statType == statType_IN) return statBonunsesArray[i];
        }*/

    }


    public static async Task<bool> IsOverEnhanceConfirmedAsync(IEnhanceable enhanceable_IN, Enhancement enhancement_IN, int amountToEnhance_IN)
    {
        if ((int)enhancement_IN.GetQuality() > (int)enhanceable_IN.GetQuality())
        {
            var enhanceableQuality = enhanceable_IN.GetQuality();
            var enhancementQuality = enhancement_IN.GetQuality();

            var tcs = new TaskCompletionSource<bool>();

            PopupPanel_Confirmation_LoadData panelLoadData = new(
                mainLoadInfo: enhancement_IN,
                panelHeader: null,
                tcs_IN: tcs,
                bluePrintsToLoad: new List<(GameObject blueprintToLoad, int amountToLoad)> { (enhancement_IN, amountToEnhance_IN) },
                extraDescription_IN: NativeHelper.BuildString_Append(
                    enhancementQuality != Quality.Level.Normal ? MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(enhancementQuality)) + MethodHelper.GiveRichTextString_Size(125) : String.Empty,
                    enhancementQuality.ToString(), " ", enhancement_IN.GetName(),
                    enhancementQuality != Quality.Level.Normal ? MethodHelper.GiveRichTextString_ClosingTagOf("color") + MethodHelper.GiveRichTextString_ClosingTagOf("size") : String.Empty,
                    " is above the quality of ",
                    enhanceableQuality != Quality.Level.Normal ? MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(enhanceableQuality)) + MethodHelper.GiveRichTextString_Size(125) : String.Empty,
                    enhanceableQuality.ToString(), " ", ((SortableBluePrint)enhanceable_IN).GetName(),
                    enhanceableQuality != Quality.Level.Normal ? MethodHelper.GiveRichTextString_ClosingTagOf("color") + MethodHelper.GiveRichTextString_ClosingTagOf("size") : String.Empty,
                    Environment.NewLine,
                    "Are you sure do you want to spend your enhancement?"));

            var invokablePanel = PanelManager.InvokablePanels[typeof(ConfirmationPopupPanel)];

            //Also CAN GET RID OF THE SUCKY CAST BELOW !!
            //Also can get rid of the invokablepanel Varibales Totally!
            PanelManager.ActivateAndLoad(
                invokablePanel_IN: invokablePanel,
                panelLoadAction_IN: () => ((ConfirmationPopupPanel)invokablePanel.MainPanel).LoadPanel(panelLoadData));

            await tcs.Task;
            if (tcs.Task.Result == false) return false;
        }
        return true;
    }

    public static async Task<bool> IsDestroyEnhancementConfirmed(IEnhanceable enhanceable_IN, Enhancement enhancement_IN)
    {
        if (enhanceable_IN.CanEnhanceWith(enhancement_IN.GetEnhancementType()) == false)
        {
            var enhancementQuality = enhancement_IN.GetQuality();
            var tcs = new TaskCompletionSource<bool>();


            var panelLoadData = new PopupPanel_Confirmation_LoadData(
                mainLoadInfo: (SortableBluePrint)enhanceable_IN,
                panelHeader: null,
                tcs_IN: tcs,
                bluePrintsToLoad: new List<(GameObject enhancement, int amountToLoad)> { (enhancement_IN, 1) },
                extraDescription_IN: NativeHelper.BuildString_Append( 
                    "Are you sure to destroy",
                    enhancementQuality != Quality.Level.Normal
                        ? MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(enhancementQuality)) + MethodHelper.GiveRichTextString_Size(125)
                        : String.Empty,
                    enhancementQuality.ToString(), " ", enhancement_IN.GetName()));

            var invokablePanel = PanelManager.InvokablePanels[typeof(ConfirmationPopupPanel)];

            //Also CAN GET RID OF THE SUCKY CAST BELOW !!
            //Also can get rid of the invokablepanel Varibales Totally!
            PanelManager.ActivateAndLoad(
                invokablePanel_IN: invokablePanel, 
                panelLoadAction_IN: () => ((ConfirmationPopupPanel)invokablePanel.MainPanel).LoadPanel(panelLoadData)); 

            await tcs.Task;
            if (tcs.Task.Result == false) return false;
        }
        return true;
    }
}
