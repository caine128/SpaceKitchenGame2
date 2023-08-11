using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResearchScroll : SpecialItem, IEquatable<ResearchScroll>
{
    public ResearchScroll(SpecialItemType.Type specialItemType_IN) : base(specialItemType_IN)
    {

    }
    public override string GetDescription()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.researchScrollInfo.description;
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.researchScrollInfo.spriteRef;
        //return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.researchScrollInfo.sprite;
    }

    public override string GetName()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.researchScrollInfo.name;
    }

    public bool Equals(ResearchScroll other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        else
        {
            return itemType == other.itemType;
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ResearchScroll);
    }

    public override int GetHashCode()
    {
        return itemType.GetHashCode() * 23;
    }

}
