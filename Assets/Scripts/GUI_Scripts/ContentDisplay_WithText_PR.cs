using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;


//[RequireComponent(typeof(GUI_TintScale))]
[RequireComponent(typeof(GUI_LerpMethods_Scale))]
public abstract class ContentDisplay : MonoBehaviour , IGUI_Animatable , ILoadable<ContentDisplayInfo>
{
    //[SerializeField] protected Adressable_SpriteDisplay[] imageContainers;
    [SerializeField] protected AdressableImage[] adressableImageContainers;
    private GUI_LerpMethods_Scale gUI_LerpMethods_Scale;


    protected virtual void Awake()
    {
        gUI_LerpMethods_Scale = GetComponent<GUI_LerpMethods_Scale>();
    }

    protected void SelectAdressableSpritesToLoad(params AssetReferenceT<Sprite>[] newSpriteRefs_IN)
    {
        for (int i = 0; i < newSpriteRefs_IN.Length; i++)
        {
            adressableImageContainers[i].LoadSprite(newSpriteRefs_IN[i]);           
        }
    }

    protected void UnloadAdressableSprite()
    {
        for (int i = 0; i < adressableImageContainers.Length; i++)
        {
            adressableImageContainers[i].UnloadSprite();
        }
    }

    public virtual void AnimateWithRoutine(Vector3? customInitialValue, 
                                          (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation, 
                                          bool isVisible,
                                          float lerpSpeedModifier,
                                          Action followingAction_IN)
    {
        Vector3 scale = isVisible == true ? Vector3.one : Vector3.zero;
        gUI_LerpMethods_Scale.Rescale(customInitialValue:customInitialValue,
                                      secondaryInterpolation: secondaryInterpolation,
                                      finalScale: scale, 
                                      lerpSpeedModifier:lerpSpeedModifier,
                                      followingAction_IN: followingAction_IN);
    }

    public  void StopAnimateWithRoutine()
    {
        if (gUI_LerpMethods_Scale.RunningCoroutine is not null) gUI_LerpMethods_Scale.StopRescale();
    }

    public void ScaleDirect(bool isVisible, (Func<RectTransform, bool> finalValueChecker, Action<RectTransform> finalValueSetter)? finalValueOperations)
    {
        if (!this.gameObject.activeSelf) gameObject.SetActive(true);
        gUI_LerpMethods_Scale.RescaleDirect(finalScale: isVisible == true 
                                                ? Vector3.one 
                                                : Vector3.zero,
                                            finalValueOperations: finalValueOperations);
    }

    public abstract void Unload();

    public abstract void Load(ContentDisplayInfo info);    // Later to Arrange all !!!! Very Imporant 

}


public abstract class ContentDisplay_WithText : ContentDisplay , ILoadable<ContentDisplayInfo_ContentDisplay_WithText>
{
    protected int indexNO;
    [SerializeField] protected TextMeshProUGUI contentInfo;


    // this is the new method 
    public abstract void Load(ContentDisplayInfo_ContentDisplay_WithText info);
    public override void Load(ContentDisplayInfo info)
    {
        Load((ContentDisplayInfo_ContentDisplay_WithText)info);
    }


    public override void Unload()
    {
        if (contentInfo.text != null) contentInfo.text = null;
    }

}



public abstract class ContentDisplay_WithText_PR : ContentDisplay_WithText
{
    protected ProductRecipe productRecipe;

    /*public override void Load(SortableBluePrint bluePrint_IN, int indexNo_IN)
    {
        productRecipe = (ProductRecipe)bluePrint_IN;
        indexNO = indexNo_IN;
    }*/


    // this is the new method 
    public override void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        productRecipe = (ProductRecipe)info.bluePrint_IN;
        indexNO = info.indexNo_IN;
    }
}



public abstract class ContentDisplay_WithText_GI<T_DisplayType> : ContentDisplay_WithText
    where T_DisplayType : struct,System.Enum
{
    protected ICraftable gameItem;
    protected T_DisplayType? contentType = null;

    /*public override void Load(SortableBluePrint bluePrint_IN, int indexNo_IN)
    {
        Debug.Log(bluePrint_IN);
        gameItem = (ICraftable)bluePrint_IN;
        indexNO = indexNo_IN;
       // contentType = gameItem.GetProductRecipe().GetRequiredIngredients(indexNo_IN, )
    }*/



    // this is the new method 
    public override void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        gameItem = (ICraftable)info.bluePrint_IN;
        indexNO = info.indexNo_IN;
    }
}


public abstract class ContentDisplay_WithText_PR<T_DisplayType> : ContentDisplay_WithText_PR
    where T_DisplayType: struct, System.Enum
{
    protected T_DisplayType? contentType = null;  
}

