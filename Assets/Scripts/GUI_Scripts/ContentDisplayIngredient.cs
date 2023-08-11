using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ContentDisplayIngredient : ContentDisplay_WithText_PR<IngredientType.Type>, IPlacableRt
{
    public RectTransform RT { get { return _rt; } }
    [SerializeField] private RectTransform _rt;

    int requiredAmount;
    bool isModified;
    bool isAmountEnough;


   /* public override void Load(SortableBluePrint bluePrint_IN, int indexNo_IN)
    {
        base.Load(bluePrint_IN, indexNo_IN);

        if (contentType != null) RemoveSubscriptionStatus();

        requiredAmount = productRecipe.GetRequiredIngredients(indexNO, out IngredientType.Type contentType_IN, out bool isModified_IN);
        contentType = contentType_IN;
        SelectAdressableSpritesToLoad(ResourcesManager.Instance.Resources_SO.ingredients[(int)contentType].spriteRef);

        isAmountEnough = IsExistingAmountEnough();
        SetSubscriptionStatus();
        isModified = isModified_IN;

        contentInfo.SetAsModifiableSpec(requiredAmount.ToString(), isModified, IsExistingAmountEnough());
    }*/

    public sealed override void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        base.Load(info);

        if (contentType != null) RemoveSubscriptionStatus();

        requiredAmount = productRecipe.GetRequiredIngredients(indexNO, out IngredientType.Type contentType_IN, out bool isModified_IN);
        contentType = contentType_IN;
        SelectAdressableSpritesToLoad(ResourcesManager.Instance.Resources_SO.ingredients[(int)contentType].spriteRef);

        isAmountEnough = IsExistingAmountEnough();
        SetSubscriptionStatus();
        isModified = isModified_IN;

        contentInfo.SetAsModifiableSpec(requiredAmount.ToString(), isModified, IsExistingAmountEnough());
    }

    private void SetSubscriptionStatus()
    {
        if (isAmountEnough)
        {
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][1] += UpdateAmountTextColor;
        }
        else
        {
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][0] += UpdateAmountTextColor;
        }
    }

    private void RemoveSubscriptionStatus()
    {
        if (isAmountEnough)
        {
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][1] -= UpdateAmountTextColor;
        }
        else
        {
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][0] -= UpdateAmountTextColor;
        }
    }


    private bool IsExistingAmountEnough()
    {
        if (requiredAmount <= ResourcesManager.CheckAmountOfIngredient((IngredientType.Type)contentType, out _))     // ResourcesManager.ingredientsDict.TryGetValue((IngredientType.Type)contentType, out Ingredient ingredient) && requiredAmount <= ingredient.GetAmount())
        {
            return true;
        }
        else
        {
            return false;
        }

        //return requiredAmount <= ResourcesManager._ingredientsDict[(IngredientType.Type)contentType].GetAmount();
    }

    private void EvaluateSubscriptionStatus(bool isAmountEnough)
    {
        if (isAmountEnough)
        {
            Debug.Log("current amount is enough" );
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][0] -= UpdateAmountTextColor;
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][1] += UpdateAmountTextColor;
        }
        else
        {
            Debug.Log("current amount is NOT enough");
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][1] -= UpdateAmountTextColor;
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][0] += UpdateAmountTextColor;
        }
    }

    private void UpdateAmountTextColor()
    {
        var isCurrentAmountEnough = IsExistingAmountEnough();

        if (isAmountEnough == isCurrentAmountEnough)
        {
            return;
        }

        else if (isCurrentAmountEnough)
        {
            contentInfo.color = isModified ? Color.green : Color.white;
            EvaluateSubscriptionStatus(true);
        }
        else if (!isCurrentAmountEnough)
        {
            contentInfo.color = Color.red;
            EvaluateSubscriptionStatus(false);
        }

        isAmountEnough = isCurrentAmountEnough;

    }

    public override void Unload()
    {      
        if(contentType != null)
        {
            RemoveSubscriptionStatus();
            UnloadAdressableSprite();

            contentType = null;
        }
    }
}
