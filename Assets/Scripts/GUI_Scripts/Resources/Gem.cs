using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class Gem : SortableBluePrint, ISpendable
{
    int ISpendable.Amount
    {
        get => amount;
        set => amount = value;
    }

    [SerializeField] private int amount;

    public Gem(int amount_IN = default)
    {
        this.amount = amount_IN;
    }


    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        throw new System.NotImplementedException();
    }

    public override string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string GetName()
    {
        throw new System.NotImplementedException();
    }
}
