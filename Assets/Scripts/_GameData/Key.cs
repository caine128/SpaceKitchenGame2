using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Key : SpecialItem, IEquatable<Key>
{

    private readonly int indexNo;

    public Key(SpecialItemType.Type specialItemType_IN, ChestType.Type chestType_IN) : base(specialItemType_IN)
    {
        int normalizedEnumIndex = (int)chestType_IN - ChestType.minUnderlyingValue;
        indexNo = normalizedEnumIndex;
    }

    public override string GetDescription()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.keyInfos[indexNo].description;
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.keyInfos[indexNo].spriteRef;
        //return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.keyInfos[indexNo].sprite;
    }

    public override string GetName()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.keyInfos[indexNo].name;
    }

    public ChestType.Type GetKeyType()
    {
        return SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.keyInfos[indexNo].type;
    }

    public bool Equals(Key other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        else
        {
            return itemType == other.itemType &&
                   GetKeyType() == other.GetKeyType();
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Key);
    }

    public override int GetHashCode()
    {
        int hashCode = (int)itemType * 2;
        hashCode += (int)GetKeyType() * 5;

        return hashCode;
    }
}
