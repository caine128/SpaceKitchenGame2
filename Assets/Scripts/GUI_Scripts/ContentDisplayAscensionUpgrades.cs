
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class ContentDisplayAscensionUpgrades : ContentDisplay_WithText_PR<Recipes_SO.AscensionUpgradeType>
{
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private Image filledImageBG;

   /* public override void Load(SortableBluePrint bluePrint_IN, int indexNo_IN)
    {
        base.Load(bluePrint_IN, indexNo_IN);

        contentType = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].ascensionUpgradeType;

        AssetReferenceAtlasedSprite spriteReference_IN = null;

        switch (contentType)
        {
            case Recipes_SO.AscensionUpgradeType.CraftTimeReduction:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].craftTimeReductionModifier.ToString();
                upgradeDescription.text = "Cook Faster";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.IngredientReduction:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].ingredientReduction.reductionAmount.ToString();
                upgradeDescription.text = "Ingredient Economy";
                spriteReference_IN = ResourcesManager.Instance.Resources_SO.ingredients[(int)productRecipe.recipeSpecs.ascensionUpgrades[indexNO].ingredientReduction.ingredient].spriteRef;
                break;
            case Recipes_SO.AscensionUpgradeType.ExtraComponentReduction:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].extraComponentReduction.reductionAmount.ToString();
                upgradeDescription.text = "Component Economy";
                spriteReference_IN = ResourcesManager.Instance.Resources_SO.extraComponentsList[ExtraComponentsType.GetNormalizedEnumIndex(productRecipe.recipeSpecs.ascensionUpgrades[indexNO].extraComponentReduction.extraComponent)].spriteRef;
                break;
            case Recipes_SO.AscensionUpgradeType.QualityChanceIncrease:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].qualityChanceIncreaseModifier.ToString();
                upgradeDescription.text = "Quality Chance";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.MultiCraftChance:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].multicraftChanceModifier.ToString();
                upgradeDescription.text = "Multicraft Chance";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.RequiredProductReduction:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].requiredProductReduction.reductionAmount.ToString();
                upgradeDescription.text = "Component Economy";
                spriteReference_IN = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].requiredProductReduction.requiredProduct_Name.receipeImageRef;
                break;
        }

        AssetReferenceAtlasedSprite[] additionalSpriteReferences = new AssetReferenceAtlasedSprite[imageContainers.Length-1];

        for (int i = 0; i < additionalSpriteReferences.Length; i++)
        {
            additionalSpriteReferences[i] = indexNO >= i ? ImageManager.SelectSprite("StarIconRed") : ImageManager.SelectSprite("StarIconYellow");
        }

        SelectAdressableSpritesToLoad(spriteReference_IN, additionalSpriteReferences[0], additionalSpriteReferences[1], additionalSpriteReferences[2]);
        SetupBackgroundFill();
    }*/

    public override sealed void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        base.Load(info);

        contentType = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].ascensionUpgradeType;

        AssetReferenceT<Sprite> spriteReference_IN = null;

        switch (contentType)
        {
            case Recipes_SO.AscensionUpgradeType.CraftTimeReduction:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].craftTimeReductionModifier.ToString();
                upgradeDescription.text = "Cook Faster";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.IngredientReduction:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].ingredientReduction.reductionAmount.ToString();
                upgradeDescription.text = "Ingredient Economy";
                spriteReference_IN = ResourcesManager.Instance.Resources_SO.ingredients[(int)productRecipe.recipeSpecs.ascensionUpgrades[indexNO].ingredientReduction.ingredient].spriteRef;
                break;
            case Recipes_SO.AscensionUpgradeType.ExtraComponentReduction:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].extraComponentReduction.reductionAmount.ToString();
                upgradeDescription.text = "Component Economy";
                spriteReference_IN = ResourcesManager.Instance.Resources_SO.extraComponentsList[ExtraComponentsType.GetNormalizedEnumIndex(productRecipe.recipeSpecs.ascensionUpgrades[indexNO].extraComponentReduction.extraComponent)].spriteRef;
                break;
            case Recipes_SO.AscensionUpgradeType.QualityChanceIncrease:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].qualityChanceIncreaseModifier.ToString();
                upgradeDescription.text = "Quality Chance";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.MultiCraftChance:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].multicraftChanceModifier.ToString();
                upgradeDescription.text = "Multicraft Chance";
                spriteReference_IN = ImageManager.SelectSprite(contentType.ToString());
                break;
            case Recipes_SO.AscensionUpgradeType.RequiredProductReduction:
                contentInfo.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].requiredProductReduction.reductionAmount.ToString();
                upgradeDescription.text = "Component Economy";
                spriteReference_IN = productRecipe.recipeSpecs.ascensionUpgrades[indexNO].requiredProductReduction.requiredProduct_Name.receipeImageRef;
                break;
        }

        AssetReferenceT<Sprite>[] additionalSpriteReferences = new AssetReferenceT<Sprite>[adressableImageContainers.Length - 1];

        for (int i = 0; i < additionalSpriteReferences.Length; i++)
        {
            additionalSpriteReferences[i] = indexNO >= i ? ImageManager.SelectSprite("StarIconRed") : ImageManager.SelectSprite("StarIconYellow");
        }

        SelectAdressableSpritesToLoad(spriteReference_IN, additionalSpriteReferences[0], additionalSpriteReferences[1], additionalSpriteReferences[2]);
        SetupBackgroundFill();

    }




    private void SetupBackgroundFill()
    {
        var ascensionLevel = (int)productRecipe.ascensionLevel;

        if (ascensionLevel <= indexNO && filledImageBG.fillAmount != 0)
        {
            filledImageBG.fillAmount = 0;
        }
        else if (ascensionLevel > indexNO && filledImageBG.fillAmount != 1)
        {
            filledImageBG.fillAmount = 1;
        }
    }

    public override void Unload() // LATER TO IMPLEMENT 
    {
        if (contentType != null)
        {
            UnloadAdressableSprite();
            contentType = null;
        }
    }


}
