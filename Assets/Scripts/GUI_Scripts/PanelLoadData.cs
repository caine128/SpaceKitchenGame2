using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SocialPlatforms;

public class PanelLoadData
{
    public readonly SortableBluePrint mainLoadInfo;
    public readonly string panelHeader;
    public readonly TaskCompletionSource<bool> tcs;

    public PanelLoadData(SortableBluePrint mainLoadInfo, string panelHeader, TaskCompletionSource<bool> tcs_IN)
    {
        this.mainLoadInfo = mainLoadInfo;
        this.panelHeader = panelHeader;
        this.tcs = tcs_IN;
    }
}


public class PopupPanel_Enhancement_LoadData : PanelLoadData
{
    public readonly Enhancement enhancement;
    public readonly IReassignablePanel.AssignedState? panelState;
    

    public PopupPanel_Enhancement_LoadData(SortableBluePrint mainLoadInfo, string panelHeader, TaskCompletionSource<bool> tcs_IN,
        Enhancement enhancement_IN, IReassignablePanel.AssignedState? panelState_IN)
        : base(mainLoadInfo, panelHeader, tcs_IN: tcs_IN)
    {
        enhancement = enhancement_IN;
        panelState = panelState_IN;       
    }
}

public abstract class PanelLoadData <T_ListBlueprintType> : PanelLoadData
    where T_ListBlueprintType : SortableBluePrint
{

    public readonly List<(T_ListBlueprintType blueprintToLoad, int amountToLoad)> bluePrintsToLoad;

    public PanelLoadData(SortableBluePrint mainLoadInfo, string panelHeader, TaskCompletionSource<bool> tcs_IN, 
        List<(T_ListBlueprintType blueprintToLoad, int amountToLoad)> bluePrintsToLoad) : base(mainLoadInfo, panelHeader, tcs_IN)
    {

        this.bluePrintsToLoad = bluePrintsToLoad;
    }
}

public class PanelLoadDatas : PanelLoadData<SortableBluePrint>
{
    public PanelLoadDatas(SortableBluePrint mainLoadInfo, string panelHeader, TaskCompletionSource<bool> tcs_IN, 
        List<(SortableBluePrint blueprintToLoad, int amountToLoad)> bluePrintsToLoad) : base(mainLoadInfo, panelHeader, tcs_IN, bluePrintsToLoad)
    {
    }
}

/*public abstract class ProgressPanelLoadDatas : PanelLoadData
{
    public readonly List<SortableBluePrint_ExtractedData<SortableBluePrint>> extractedLoadDatas;
    public ProgressPanelLoadDatas(SortableBluePrint mainLoadInfo, string panelHeader, TaskCompletionSource<bool> tcs_IN
        ,IEnumerable<SortableBluePrint_ExtractedData<T_BluePintType>> blueprintsToLoad) : base(mainLoadInfo, panelHeader, tcs_IN)
    {
        this.extractedLoadDatas = blueprintsToLoad.ToList();
    }
}*/

public class ProgressPanelLoadData : PanelLoadData
{
    public readonly IEnumerable<IRushable> rushableITems;
    public readonly int clickedObjectIndex;
    public ProgressPanelLoadData(SortableBluePrint mainLoadInfo, string panelHeader, TaskCompletionSource<bool> tcs_IN,
        IEnumerable<IRushable> rushableItemsData, int clickedObjectIndex) : base(mainLoadInfo, panelHeader, tcs_IN)
    {
        this.rushableITems = rushableItemsData;
        this.clickedObjectIndex = clickedObjectIndex;
    }
}

/*public class ProgressPanelLoadDatasOfProductRecipee : ProgressPanelLoadDatas<ProductRecipe>
{
    public ProgressPanelLoadDatasOfProductRecipee(SortableBluePrint mainLoadInfo, string panelHeader, TaskCompletionSource<bool> tcs_IN, 
        IEnumerable<SortableBluePrint_ExtractedData<ProductRecipe>> blueprintsToLoad) : base(mainLoadInfo, panelHeader, tcs_IN, blueprintsToLoad)
    {
    }
}*/

public class PopupPanel_Confirmation_LoadData : PanelLoadData<GameObject>
{
    public readonly string extraDescription;  

    public PopupPanel_Confirmation_LoadData(SortableBluePrint mainLoadInfo, string panelHeader, TaskCompletionSource<bool> tcs_IN,List<(GameObject blueprintToLoad, int amountToLoad)> bluePrintsToLoad, 
        string extraDescription_IN) : base(mainLoadInfo, panelHeader, tcs_IN, bluePrintsToLoad)
    {
        this.extraDescription = extraDescription_IN;
    }
}

public abstract class ModalLoadData
{
    public readonly Information_Modal_Panel.ModalState modalState;

    public ModalLoadData(Information_Modal_Panel.ModalState modalState_IN)
    {
        this.modalState = modalState_IN;
    }
}

public class ModalPanel_Enhancement_LoadData : ModalLoadData
{
    public readonly (SortableBluePrint enhanceable,int succesAmount) enhanceableInfo;
    public readonly Enhancement enhancement;
    public readonly IReadOnlyList<(AssetReferenceT<Sprite> spriteRef, string enhancmentBonus)> enhancementBonuses;
    public readonly (string failedEnhanceableName, string failedEnhancementName, int failAmount)? failedAttemptnfo;

    public ModalPanel_Enhancement_LoadData((IEnhanceable enheanceable_IN,int succesAmount_IN) enhanceableInfo_IN, 
                                            Enhancement enhancement_IN,
                                            IReadOnlyList<(AssetReferenceT<Sprite> spriteRef, string enhancmentBonus)> enhancementBonuses_IN,
                                            Information_Modal_Panel.ModalState modalState_IN,
                                            (string failedEnhanceableName, string failedEnhancementName, int failAmount)? failedAttemptnfo_IN = null)
                 : base(modalState_IN)
    {
        this.enhanceableInfo.enhanceable = (SortableBluePrint)enhanceableInfo_IN.enheanceable_IN;
        this.enhanceableInfo.succesAmount = enhanceableInfo_IN.succesAmount_IN;
        this.enhancement = enhancement_IN;
        this.enhancementBonuses = enhancementBonuses_IN;
        this.failedAttemptnfo = failedAttemptnfo_IN;
    }

}

public class ModalPanel_DismantleItem_LoadData : ModalLoadData
{
    public readonly SortableBluePrint craftableItem;
    public readonly int dismantleAmount;
    public readonly IEnumerable<(AssetReferenceT<Sprite> spriteRef, int amount)> recycledResources;

    public ModalPanel_DismantleItem_LoadData (ICraftable craftableItem_IN,
                                             int dismantleAmount_IN,
                                             IEnumerable<(AssetReferenceT<Sprite> spriteRef_IN, int amount_IN)> recycledResources_IN,
                                             Information_Modal_Panel.ModalState modalState_IN) 
                : base(modalState_IN)
    {
        this.craftableItem = (SortableBluePrint)craftableItem_IN;
        this.dismantleAmount = dismantleAmount_IN;
        this.recycledResources = recycledResources_IN;
    }
}

public class ModalPanel_DisplayBonuses_LoadData : ModalLoadData
{
    public readonly AssetReferenceT<Sprite> mainSprite;
    public readonly AssetReferenceT<Sprite> secondarySprite;
    public readonly (string bonusExplanationString1, string bonusExplanationString2) bonusExplanationStringTuple;
    public readonly string subheaderString;

    public ModalPanel_DisplayBonuses_LoadData(AssetReferenceT<Sprite> mainSprite_IN,
                                              AssetReferenceT<Sprite> secondarySprite_IN,
                                              (string bonusExplanationString1_IN, string bonusExplanationString2_IN) bonusExplanationStringTuple_IN,
                                              string subheaderString_IN,
                                              Information_Modal_Panel.ModalState modalState_IN)
                : base(modalState_IN)
    {
        mainSprite = mainSprite_IN;
        secondarySprite = secondarySprite_IN;
        bonusExplanationStringTuple = bonusExplanationStringTuple_IN;
        subheaderString = subheaderString_IN;
    }
}

public class ModalPanel_DisplayBonuses_Ascension_LoadData : ModalPanel_DisplayBonuses_LoadData
{
    public readonly AscensionLevel.Type ascensionLevel;

    public ModalPanel_DisplayBonuses_Ascension_LoadData(AscensionLevel.Type ascensionLevel_IN,
                                              AssetReferenceT<Sprite> mainSprite_IN,
                                              AssetReferenceT<Sprite> secondarySprite_IN,
                                              (string bonusExplanationString1_IN, string bonusExplanationString2_IN) bonusExplanationStringTuple_IN,
                                              string subheaderString_IN,
                                              Information_Modal_Panel.ModalState modalState_IN)
                : base(mainSprite_IN, secondarySprite_IN, bonusExplanationStringTuple_IN, subheaderString_IN, modalState_IN)
    {
        ascensionLevel = ascensionLevel_IN;
    }
}

public class ModalPanel_DisplayBonuses_UnlockCharacter : ModalLoadData
{
    public readonly Character character;

    public ModalPanel_DisplayBonuses_UnlockCharacter(Character character_IN, Information_Modal_Panel.ModalState modalState_IN) : base(modalState_IN)
    {
        character = character_IN;
    }
}

public class ModalPanel_DisplayBonuses_HireCharacter : ModalPanel_DisplayBonuses_UnlockCharacter
{
    public readonly IEnumerable<Recipes_SO> unlocks;
    public ModalPanel_DisplayBonuses_HireCharacter(IEnumerable<Recipes_SO> unlocks_IN,Character character_IN, Information_Modal_Panel.ModalState modalState_IN) : base(character_IN, modalState_IN)
    {
        unlocks = unlocks_IN;
    }
}

public class ModalPanel_DisplayBonuses_LevelUpCharacter : ModalPanel_DisplayBonuses_UnlockCharacter
{
    public readonly IEnumerable<SortableBluePrint> unlocks;

    public ModalPanel_DisplayBonuses_LevelUpCharacter(IEnumerable<SortableBluePrint> unlocks_IN, Character character_IN, Information_Modal_Panel.ModalState modalState_IN) 
        : base(character_IN, modalState_IN)
    {
        unlocks = unlocks_IN;
    }
}

public class ModalPanel_DisplayBonuses_ProductRecipeUnlockOrResearch : ModalLoadData
{
    public readonly ProductRecipe productRecipe;
    public ModalPanel_DisplayBonuses_ProductRecipeUnlockOrResearch(ProductRecipe productRecipe, Information_Modal_Panel.ModalState modalState_IN) : base(modalState_IN)
    {
        this.productRecipe = productRecipe;
    }

}

    public class ModalPanel_GameItemRarityUpgrade : ModalLoadData       
    {
        public readonly TaskCompletionSource<bool> tcs;
        public readonly GameObject gameItem;
        public ModalPanel_GameItemRarityUpgrade(GameObject gameItem_IN, Information_Modal_Panel.ModalState modalState_IN, TaskCompletionSource<bool> tcs_IN) : base(modalState_IN)
        {
            gameItem = gameItem_IN;
            tcs = tcs_IN;
        }
    }


