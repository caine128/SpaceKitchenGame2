using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctionType
{
    public enum RecipeInfoPanel
    {
        None = 0,
        CraftButton = 1,
        AscendButton = 2,
    }

    public enum GameItemInfoPanel
    {
        SlotOpenToEnhancement = 1,
        SlotEnhanced = 2,
        SlotRequiresWorker =3,
    }

    public enum PopupPanel
    {
        None = 0,
        Research =1,
        UnlockRecipe_None =2,
        UnlockRecipe_Worker =3,
        UnlockRecipe_Chest =4,
        UnlockReipe_Buy =5,
        Confirm = 6,
        Reject = 7,
        MissingRequirements_RefillByToken = 8,
        MissingRequirements_RefillByGems = 9,
        MissingRequirements_BuyFromMarket = 10,
        MissingRequirements_GoToQuestArea = 11,
        MissingRequirements_CraftProduct = 12,
        MissingRequirements_ActivateIngredientGenerator = 13,
        MissingRequirements_BuyShopUpgrade = 14,
        MissingRequirements_UpgradeShopUpgrade = 26,
        MissingRequirements_UpgradeResourceCabinet = 15,
        MissingRequirements_None=16,
        MissingRequirements_WorkerNotHired = 17,
       
        EnhancementPanel_ChangeSelection = 18,
        EnhancementPanel_Enhance = 19,
        EnhancementPanel_Enhancement= 20,
        EnhancementPanel_DestroyEnhancement= 21,
        EnhancemenPanel_ReplaceEnhancement= 22,
        ProgressPanel_RushByEnergy = 23,
        ProgressPanel_RushByGem = 24,
        ProgressPanel_BackToShop =25,
    }

    public enum HireCharactersPanel
    {
        None=0,
        RecruitWithGold=1,
        RecruitWithGem=2,
        RecruitWithCommanderBadge=3
    }

    public enum ShopUpgradeInfoPanel
    {
        None=0,
        InvestWithGold=1,
        InvestWithGem=2,
        GoToProgressPanel=3,
        Collect=4,
    }

    public enum InformationModalPanel
    {
        None=0,
        Collect_Rarity=1,
        Upgrade_Rarity=2,
    }
}
