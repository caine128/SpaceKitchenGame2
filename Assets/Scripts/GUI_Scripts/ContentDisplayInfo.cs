using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class ContentDisplayInfo 
{
  
}

public class ContentDisplayInfo_JustSprite : ContentDisplayInfo
{
    public readonly AssetReferenceT<Sprite> spriteRef;

    public ContentDisplayInfo_JustSprite(AssetReferenceT<Sprite> spriteRef_IN)
    {
        spriteRef = spriteRef_IN;
    }
}
public class ContentDisplayInfo_JustSpriteAndText : ContentDisplayInfo_JustSprite
{

    public readonly string textVal;
    public ContentDisplayInfo_JustSpriteAndText(string textVal_IN, AssetReferenceT<Sprite> spriteRef_IN) : base(spriteRef_IN)
    {
        textVal = textVal_IN;
    }
}

public class ContentDisplayInfo_JustSpriteAndDoubleText : ContentDisplayInfo_JustSpriteAndText
{
    public readonly string textValAdditional;

    public ContentDisplayInfo_JustSpriteAndDoubleText(string textValAdditional_IN, string textVal_IN, AssetReferenceT<Sprite> spriteRef_IN) : base(textVal_IN, spriteRef_IN)
    {
        textValAdditional = textValAdditional_IN;
    }
}


public class ContentDisplayInfo_ContentDisplay_WithText: ContentDisplayInfo
{
    public readonly SortableBluePrint bluePrint_IN;
    public readonly int indexNo_IN;

    public ContentDisplayInfo_ContentDisplay_WithText(SortableBluePrint bluePrint_IN, int indexNo_IN)
    {
        this.bluePrint_IN = bluePrint_IN;
        this.indexNo_IN = indexNo_IN;
    }
}

public class ContentDisplayInfo_ConentDisplayFrame : ContentDisplayInfo
{

    public readonly SortableBluePrint bluePrint_IN;
    public readonly int? indexNo_IN;

    public ContentDisplayInfo_ConentDisplayFrame(SortableBluePrint bluePrint_IN, int? indexNo_IN = null)
    {
        this.bluePrint_IN= bluePrint_IN;
        this.indexNo_IN=indexNo_IN;
    }
}


public class ContentDisplayInfo_PopupGeneric : ContentDisplayInfo
{
    public readonly AssetReferenceT<Sprite> spriteRef_IN;
    public readonly string contentTitle_IN;
    public readonly string contentValue_IN;
    public readonly Func<ToolTipInfo> GetTooltipText_IN;
    public readonly bool?  isValueModified_IN;
    //public readonly SortableBluePrint clickableInfoObject_IN;

    public ContentDisplayInfo_PopupGeneric(
        AssetReferenceT<Sprite> spriteRef_IN, string contentTitle, string contentValue , Func<ToolTipInfo> GetTooltipText, bool? isValueModified = null)
    {
        this.spriteRef_IN = spriteRef_IN;
        this.contentTitle_IN = contentTitle;
        this.contentValue_IN = contentValue;
        this.GetTooltipText_IN = GetTooltipText;
        this.isValueModified_IN = isValueModified;
        //this.clickableInfoObject_IN = clickableObjectInfo_IN;
    }
}


public class ContentDisplayInfo_Modal : ContentDisplayInfo
{
    public readonly SortableBluePrint mainBluePrint_IN;
    public readonly AssetReferenceT<Sprite> spriteRef_IN;
    public readonly string contentTextMain_IN;
    public readonly string contentTextSecondary_IN;
    public readonly ContentDisplayModalPanel.DynamicShape dynamicShape;
    public readonly bool isMaskActive;
    public readonly Color bgColor;

    public ContentDisplayInfo_Modal(SortableBluePrint mainBluePrint_IN, AssetReferenceT<Sprite> spriteRef_IN, 
                                    string contentTextMain_IN, string contentTextSecondary_IN, 
                                    ContentDisplayModalPanel.DynamicShape dynamicShape_IN, bool isMaskActive =true, Color bgColor = default)
    {
        this.mainBluePrint_IN = mainBluePrint_IN;
        this.spriteRef_IN = spriteRef_IN;
        this.contentTextMain_IN = contentTextMain_IN;
        this.contentTextSecondary_IN = contentTextSecondary_IN;
        this.dynamicShape = dynamicShape_IN;
        this.isMaskActive = isMaskActive;
        this.bgColor = bgColor;
    }

}
