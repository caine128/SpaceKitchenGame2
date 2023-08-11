using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public interface ISpendable 
{
    public int Amount {  get; protected set;}


    public void SetAmount(int newAmount)
    {
        Amount += newAmount;
    }

    public static string ToScreenFormat(int value)
        => value switch
        {
            < -999 and >= -999999 or > 999 and <= 999999 => $"{value.ToString("0,.#K", CultureInfo.InvariantCulture)}",
            < -999999 or > 999999 => $"{value.ToString("0,,.##M", CultureInfo.InvariantCulture)}",
            _ => $"{value}"
        };
}
