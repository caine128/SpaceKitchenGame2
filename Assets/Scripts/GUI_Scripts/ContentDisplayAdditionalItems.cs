using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ContentDisplayAdditionalItems : ContentDisplay_WithText_PR<GameItemType.Type>, IPlacableRt
{
    public RectTransform RT { get { return _rt; } }
    [SerializeField] private RectTransform _rt;

    int requiredAmount;
    bool isModified;
    bool isAmountEnough;



    /*public override void Load(SortableBluePrint bluePrint_IN, int indexNo_IN)
    {
        if (contentType != null) RemoveSubscriptionStatus();

        base.Load(bluePrint_IN, indexNo_IN);
        
        
        //if (contentType != null) RemoveSubscriptionStatus();

        requiredAmount = productRecipe.GetRequiredAdditionalItems(indexNO, out GameItemType.Type? contentType_IN, out bool? isModified_IN);
        contentType = (GameItemType.Type)contentType_IN;

        SelectAdressableSpritesToLoad(contentType == GameItemType.Type.ExtraComponents 
            ? ResourcesManager.Instance.Resources_SO.extraComponentsList[ExtraComponentsType.GetNormalizedEnumIndex(productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredExtraComponent.extraComponentType)].spriteRef
            : productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredProduct.requiredProduct_Name.receipeImageRef);

        isAmountEnough = IsExistingAmountEnough(out int existingAmount);
        SetSubscriptionStatus();
        isModified = (bool)isModified_IN;

        contentInfo.SetAsModifiableSpec(NativeHelper.BuildString_Append(requiredAmount.ToString(), "/", existingAmount.ToString()), isModified, isAmountEnough);
    }*/

    public override sealed void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        if (contentType != null) RemoveSubscriptionStatus();

        base.Load(info);

        requiredAmount = productRecipe.GetRequiredAdditionalItems(indexNO, out GameItemType.Type? contentType_IN, out bool? isModified_IN);
        contentType = (GameItemType.Type)contentType_IN;

        SelectAdressableSpritesToLoad(contentType == GameItemType.Type.ExtraComponents
            ? ResourcesManager.Instance.Resources_SO.extraComponentsList[ExtraComponentsType.GetNormalizedEnumIndex(productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredExtraComponent.extraComponentType)].spriteRef
            : productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredProduct.requiredProduct_Name.receipeImageRef);

        isAmountEnough = IsExistingAmountEnough(out int existingAmount);
        SetSubscriptionStatus();
        isModified = (bool)isModified_IN;

        contentInfo.SetAsModifiableSpec(NativeHelper.BuildString_Append(requiredAmount.ToString(), "/", existingAmount.ToString()), isModified, isAmountEnough);
    }


    private bool IsExistingAmountEnough(out int existingAmount)
    {
        var existingAmountTemp = contentType switch
        {
            GameItemType.Type.ExtraComponents => Inventory.Instance.CheckAmountInInventory(productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredExtraComponent.extraComponentType, out _),
            GameItemType.Type.Product => Inventory.Instance.CheckAmountInInventory_ByNameDict(productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredProduct.requiredProduct_Name.recipeName, out _),
            //Inventory.Instance.CheckAmountInInventory_Name(productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredProduct.requiredProduct_Name.recipeName, (GameItemType.Type)contentType),
            _ => throw new System.NotImplementedException(),
        };
        existingAmount = existingAmountTemp;
        return requiredAmount <= existingAmountTemp;
    }

    private void SetSubscriptionStatus()
    {
        if (isAmountEnough && contentType == GameItemType.Type.ExtraComponents)
        {
            Inventory.additionalItemsEventMapping[productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredExtraComponent.extraComponentType] += UpdateAmountTextcolor;
        }

        else if (isAmountEnough && contentType == GameItemType.Type.Product)
        {
            Inventory.onProductRemoved += VerifyCallback;
        }
    }

    private void RemoveSubscriptionStatus()
    {
        if (isAmountEnough && contentType == GameItemType.Type.ExtraComponents)
        {
            Inventory.additionalItemsEventMapping[productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredExtraComponent.extraComponentType] -= UpdateAmountTextcolor;
        }
        else if (isAmountEnough && contentType == GameItemType.Type.Product)
        {
            Inventory.onProductRemoved -= VerifyCallback;
        }
    }

    //private void EvaluateSubscriptionStatus()
    //{
    //    if (contentType == GameItemType.Type.ExtraComponents)
    //    {
    //        Inventory.additionalItemsEventMapping[productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredExtraComponent.extraComponentType] -= UpdateAmountTextcolor;
    //    }
    //    else if (contentType == GameItemType.Type.Product)
    //    {
    //        Inventory.onProductRemoved -= VerifyCallback;
    //    }
    //}

    private void VerifyCallback(string productName_IN)
    {
        if (productName_IN != productRecipe.recipeSpecs.requiredAdditionalItems[indexNO].requiredProduct.requiredProduct_Name.recipeName)
        {
            return;
        }

        UpdateAmountTextcolor();
    }

    private void UpdateAmountTextcolor()
    {

        var isCurrentAmountEnough = IsExistingAmountEnough(out int existingAmount);
        contentInfo.SetAsModifiableSpec(NativeHelper.BuildString_Append(requiredAmount.ToString(), "/", existingAmount.ToString()), isModified, isAmountEnough);

        if (isAmountEnough == isCurrentAmountEnough)
        {
            return;
        }

        else if (isCurrentAmountEnough)
        {
            contentInfo.color = isModified ? Color.green : Color.white;
        }
        else if (!isCurrentAmountEnough)
        {
            contentInfo.color = Color.red;
            RemoveSubscriptionStatus();
        }
        isAmountEnough = isCurrentAmountEnough;
    }



    public override void Unload() // LATER TO IMPLEMENT 
    {
        if (contentType != null)
        {
            RemoveSubscriptionStatus();
            UnloadAdressableSprite();

            contentType = null;
        }
    }
}
