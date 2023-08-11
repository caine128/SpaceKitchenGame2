using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Chest : SpecialItem , IEquatable<Chest>
{
    //public readonly ChestType.Type _chestType;
    //public readonly int _chestTier;

    private readonly int indexNo;

    public Chest(SpecialItemType.Type specialItemType_IN, ChestType.Type chestType_IN) : base( specialItemType_IN)
    {
        int normalizedEnumIndex = (int)chestType_IN - ChestType.minUnderlyingValue;
        indexNo = normalizedEnumIndex;
    }

    public override string GetDescription()
    {
        return SpecialItemsManager.Instance.Chests_SO.chest[indexNo].description;
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return SpecialItemsManager.Instance.Chests_SO.chest[indexNo].spriteRef;
        //return SpecialItemsManager.Instance.Chests_SO.chest[indexNo].sprite;
    }

    public override string GetName()
    {
        return SpecialItemsManager.Instance.Chests_SO.chest[indexNo].name;
    }

    public int GetChestTier()
    {
        return indexNo + 1;
    }
    
    public ChestType.Type GetChestType()
    {
        return SpecialItemsManager.Instance.Chests_SO.chest[indexNo].type;
    }

    public object[] PullChestLoot(params float[] pullValues)
    {
        var amountOfObjects = pullValues.Length;
        object[] lootObjects = new object[amountOfObjects];

        return lootObjects;
    }

    public bool Equals(Chest other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        else
        {
            return itemType == other.itemType &&
                   GetChestType() == other.GetChestType();
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Chest);
    }

    public override int GetHashCode()
    {
        int hashCode = (int)itemType * 7;
        hashCode += (int)GetChestType() * 9;

        return hashCode;
    }
}
