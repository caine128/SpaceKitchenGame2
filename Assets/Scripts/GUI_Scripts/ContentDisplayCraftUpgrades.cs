
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ContentDisplayCraftUpgrades : ContentDisplay_WithText_PR<Recipes_SO.CraftUpgradeType>
{
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private Image filledImageBG;
    //[SerializeField] private Image bottomIcon;      // TO BE IMPLEMENTED !!!
    //[SerializeField] private AssetReferenceAtlasedSprite[] bottomSpriteRefs;
    [SerializeField] private TextMeshProUGUI bottomText;

    public override sealed void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        base.Load(info);

        contentType = productRecipe.recipeSpecs.craftingUpgrades[indexNO].craftUpgradeType;

        AssetReferenceT<Sprite> spriteReference_IN = null;

        switch (contentType)
        {
            case Recipes_SO.CraftUpgradeType.CraftTimeReduction:
                var percentString =
                contentInfo.text = contentType.Value.AppendCraftingUpgradeBonusToSTring(productRecipe.recipeSpecs.craftingUpgrades[indexNO].craftTimeReductionModifier);
                //contentInfo.text = $"{(1- productRecipe.recipeSpecs.craftingUpgrades[indexNO].craftTimeReductionModifier)*100}%";
                //upgradeDescription.text = "Cook Faster";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;

            case Recipes_SO.CraftUpgradeType.IngredientReduction:
                contentInfo.text = contentType.Value.AppendCraftingUpgradeBonusToSTring(productRecipe.recipeSpecs.craftingUpgrades[indexNO].ingredientReduction.reductionAmount);
                //contentInfo.text = $"- {productRecipe.recipeSpecs.craftingUpgrades[indexNO].ingredientReduction.reductionAmount}";
                //upgradeDescription.text = "Ingredient Economy";
                spriteReference_IN = ResourcesManager.Instance.Resources_SO.ingredients[(int)productRecipe.recipeSpecs.craftingUpgrades[indexNO].ingredientReduction.ingredient].spriteRef;
                break;

            case Recipes_SO.CraftUpgradeType.ExtraComponentReduction:
                contentInfo.text = contentType.Value.AppendCraftingUpgradeBonusToSTring(productRecipe.recipeSpecs.craftingUpgrades[indexNO].extraComponentReduction.reductionAmount);
                //contentInfo.text = $"- {productRecipe.recipeSpecs.craftingUpgrades[indexNO].extraComponentReduction.reductionAmount}";
                //upgradeDescription.text = "Component Econumy";
                spriteReference_IN = ResourcesManager.Instance.Resources_SO.extraComponentsList[ExtraComponentsType.GetNormalizedEnumIndex(productRecipe.recipeSpecs.craftingUpgrades[indexNO].extraComponentReduction.extraComponent)].spriteRef;
                break;

            case Recipes_SO.CraftUpgradeType.ValueIncrease:
                contentInfo.text = contentType.Value.AppendCraftingUpgradeBonusToSTring(productRecipe.recipeSpecs.craftingUpgrades[indexNO].valueIncreaseModifier);
                // contentInfo.text = $"x {productRecipe.recipeSpecs.craftingUpgrades[indexNO].valueIncreaseModifier}";
                //upgradeDescription.text = "Value Increase";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;

            case Recipes_SO.CraftUpgradeType.QualityChanceIncrease:
                contentInfo.text = contentType.Value.AppendCraftingUpgradeBonusToSTring(productRecipe.recipeSpecs.craftingUpgrades[indexNO].qualityChanceIncreaseModifier);
                contentInfo.text = $"x {productRecipe.recipeSpecs.craftingUpgrades[indexNO].qualityChanceIncreaseModifier}";
                //upgradeDescription.text = "Quality Chance";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;

            case Recipes_SO.CraftUpgradeType.UnlockRecipe:
                contentInfo.text = string.Empty;
                //upgradeDescription.text = "Unlock Recipe";
                spriteReference_IN = productRecipe.recipeSpecs.craftingUpgrades[indexNO].unlockRecipe.receipeImageRef; // LATER TO FIX THIS IS DONE FOR UNCONTINUED DISHE UNLOCKS NOW ITS QUALITY MODOFIER
                break;
        }

        upgradeDescription.text = contentType.Value.GetNameOfTheCraftingUpgradeType();

        var masteryLevel = (int)productRecipe.masteryLevel;
        AssetReferenceT<Sprite> additionalSpriteReference = indexNO < masteryLevel ? ImageManager.SelectSprite("MasteredIcon") : ImageManager.SelectSprite("NotMasteredIcon");

        SelectAdressableSpritesToLoad(spriteReference_IN, additionalSpriteReference);
        SetupBackgroundFill(masteryLevel);
    }

   /* public override void Load(SortableBluePrint bluePrint_IN, int indexNo_IN)
    {
        base.Load(bluePrint_IN, indexNo_IN);

        contentType = productRecipe.recipeSpecs.craftingUpgrades[indexNO].craftUpgradeType;
        
        
        AssetReferenceAtlasedSprite spriteReference_IN = null;

        switch (contentType)
        {
            case Recipes_SO.CraftUpgradeType.CraftTimeReduction:
                contentInfo.text = productRecipe.recipeSpecs.craftingUpgrades[indexNO].craftTimeReductionModifier.ToString();
                upgradeDescription.text = "Cook Faster";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.CraftUpgradeType.IngredientReduction:
                contentInfo.text = productRecipe.recipeSpecs.craftingUpgrades[indexNO].ingredientReduction.reductionAmount.ToString();
                upgradeDescription.text = "Ingredient Economy";
                spriteReference_IN = ResourcesManager.Instance.Resources_SO.ingredients[(int)productRecipe.recipeSpecs.craftingUpgrades[indexNO].ingredientReduction.ingredient].spriteRef;
                break;
            case Recipes_SO.CraftUpgradeType.ExtraComponentReduction:
                contentInfo.text = productRecipe.recipeSpecs.craftingUpgrades[indexNO].extraComponentReduction.reductionAmount.ToString();
                upgradeDescription.text = "Component Econumy";
                spriteReference_IN = ResourcesManager.Instance.Resources_SO.extraComponentsList[ExtraComponentsType.GetNormalizedEnumIndex(productRecipe.recipeSpecs.craftingUpgrades[indexNO].extraComponentReduction.extraComponent)].spriteRef;
                break;
            case Recipes_SO.CraftUpgradeType.ValueIncrease:
                contentInfo.text = productRecipe.recipeSpecs.craftingUpgrades[indexNO].valueIncreaseModifier.ToString();
                upgradeDescription.text = "Value Increase";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.CraftUpgradeType.QualityChanceIncrease:
                contentInfo.text = productRecipe.recipeSpecs.craftingUpgrades[indexNO].qualityChanceIncreaseModifier.ToString();
                upgradeDescription.text = "Quality Chance";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.CraftUpgradeType.UnlockRecipe:
                contentInfo.text = string.Empty;
                upgradeDescription.text = "Unlock Recipe";
                spriteReference_IN = productRecipe.recipeSpecs.craftingUpgrades[indexNO].unlockRecipe.receipeImageRef; // LATER TO FIX THIS IS DONE FOR UNCONTINUED DISHE UNLOCKS NOW ITS QUALITY MODOFIER
                break;
        }

        var masteryLevel = (int)productRecipe.masteryLevel;
        AssetReferenceAtlasedSprite additionalSpriteReference = indexNO < masteryLevel ? ImageManager.SelectSprite("MasteredIcon") : ImageManager.SelectSprite("NotMasteredIcon"); 

        SelectAdressableSpritesToLoad(spriteReference_IN, additionalSpriteReference);
        SetupBackgroundFill(masteryLevel);
    }*/

    private void SetupBackgroundFill(int masteryLevel_IN)
    {
        if (indexNO < masteryLevel_IN)
        {
            filledImageBG.fillAmount = 1;
            bottomText.text = (0).ToString();
        }
        else if (indexNO == masteryLevel_IN)
        {
            var amountCraftedLocal = productRecipe.amountCraftedLocal;
            var amountForNextLevel = productRecipe.recipeSpecs.craftingUpgrades[masteryLevel_IN].craftsNeeded;

            filledImageBG.fillAmount = CalculateFillAmount.CalculateFill(amountCraftedLocal, amountForNextLevel);
            bottomText.text = (amountForNextLevel - amountCraftedLocal).ToString();
        }
        else
        {
            filledImageBG.fillAmount = 0;
            bottomText.text = productRecipe.recipeSpecs.craftingUpgrades[indexNO].craftsNeeded.ToString();
        }

    }


    public override void Unload()   // LATER TO IMPLEMENT 
    {
        if (contentType != null)
        {
            UnloadAdressableSprite();

            contentType = null;
        }
    }
}
