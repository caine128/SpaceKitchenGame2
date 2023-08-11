using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentDisplayIngredient_DeletePopup : ContentDisplay_WithText_GI<IngredientType.Type>, IPlacableRt 
{
    private int requiredAmount;
    public int RecycledAmount => Mathf.CeilToInt((float)requiredAmount / 10) * DeleteItemPopupPanel.Instance.CounterObject.CurrentAmount;
    public IngredientType.Type? ContentType => contentType;

    public RectTransform RT => _rt;
    [SerializeField] private RectTransform _rt;
    [SerializeField] private RectTransform _rtContentInfo;
    [SerializeField] private GUI_LerpMethods_Movement _lerpContentInfo;


    //private void OnDisable()
    //{
    //    RemoveSubscriptions();
    //}

    public sealed override void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        base.Load(info);

        requiredAmount = gameItem.GetProductRecipe().GetRequiredIngredients(info.indexNo_IN, out IngredientType.Type ingredientType, out _);
        contentType = ingredientType;
        SetSubscriptions();

        var existingAmount = ResourcesManager.CheckAmountOfIngredient((IngredientType.Type)contentType, out Ingredient ingredient);
        var currentMaxAmount = ingredient.MaxCap;

        SelectAdressableSpritesToLoad(ResourcesManager.Instance.Resources_SO.ingredients[(int)contentType].spriteRef);
        contentInfo.text = NativeHelper.BuildString_Append(existingAmount.ToString(), " / ", currentMaxAmount.ToString(), " + ", RecycledAmount.ToString());
        SetTextColor(existingAmount >= currentMaxAmount, (IngredientType.Type)contentType);
    }

    public sealed override void AnimateWithRoutine(Vector3? customInitialValue,
                                                  (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation,
                                                  bool isVisible,
                                                  float lerpSpeedmodifier,
                                                  Action followingAction_IN)
    {
        base.AnimateWithRoutine(customInitialValue: customInitialValue, 
                                secondaryInterpolation:secondaryInterpolation,
                                isVisible: isVisible, 
                                lerpSpeedModifier: lerpSpeedmodifier,
                                followingAction_IN: followingAction_IN);

        contentInfo.gameObject.SetActive(false);
        _rtContentInfo.anchoredPosition = new Vector2(_rtContentInfo.rect.width / 2, 0);
        contentInfo.gameObject.SetActive(true);
        _lerpContentInfo.InitialCall(Vector2.zero);

    }



    public void ModifyOnModifierChange()
    {
        UpdateTextField();
    }

    private void SetTextColor(bool isCapReached, IngredientType.Type ingredientType_IN)
    {
        if (contentType == ingredientType_IN) contentInfo.SetTextColor_CapReached(isCapReached);

    }

    private void UpdateTextField()
    {
        contentInfo.text = NativeHelper.BuildString_Append(ResourcesManager.CheckAmountOfIngredient((IngredientType.Type)contentType, out Ingredient ingredient).ToString(), " / ", ingredient.MaxCap.ToString(), " + ", RecycledAmount.ToString());
    }

    //private int CalculateRecycledIngredientAmount()
    //{
    //    return Mathf.CeilToInt(requiredAmount / 10);
    //}

    private void SetSubscriptions()
    {
        ResourcesManager.onCapReached += SetTextColor;
        ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][0] += UpdateTextField;
        ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][1] += UpdateTextField;

    }

    public void RemoveSubscriptions()
    {
        if (contentType != null)
        {
            ResourcesManager.onCapReached -= SetTextColor;
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][0] -= UpdateTextField;
            ResourcesManager.ingredientEventMapping[(IngredientType.Type)contentType][1] -= UpdateTextField;
        }
    }

    public override void Unload()
    {
        if (contentType != null)
        {
            //RemoveSubscriptions();
            UnloadAdressableSprite();
            requiredAmount = 0;
            contentType = null;
        }
    }

}
