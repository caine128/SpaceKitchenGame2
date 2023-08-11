using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AscensionShard : SpecialItem , IEquatable<AscensionShard>
{
    public AscensionShard(SpecialItemType.Type specialItemType_IN) : base(specialItemType_IN)
    {

    }


    public override string GetDescription()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.ascensionShardInfo.description;
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.ascensionShardInfo.spriteRef;
        //return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.ascensionShardInfo.sprite;
    }

    public override string GetName()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.ascensionShardInfo.name;
    }

    public bool Equals(AscensionShard other)
    {
        if(other == null || GetType() != other.GetType())
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
        return Equals(obj as AscensionShard);
    }

    public override int GetHashCode()
    {
        return itemType.GetHashCode() * 22;
    }

}
