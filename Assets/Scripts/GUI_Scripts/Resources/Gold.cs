using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class Gold : SortableBluePrint, ISpendable
{
    int ISpendable.Amount
    {
        get => amount;
        set => amount = value;
    }

    [SerializeField] private int amount;


    public Gold(int amount_IN = default)
    {
        amount = amount_IN;
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

    /*public static string ToScreenFormat(int value)
        => value switch
       {
           < -999 and >= -999999 or > 999 and <= 999999 => $"{value.ToString("0,.#K", CultureInfo.InvariantCulture)}",
           < -999999 or > 999999 => $"{value.ToString("0,,.##M", CultureInfo.InvariantCulture)}",
           _ => $"{value}"
       };*/
}


