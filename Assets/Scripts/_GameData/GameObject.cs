using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GameObject : SortableBluePrint , ICollectible  , IDatable
{
    protected int AmountOwned =0;                 //later to make autopropoertys
    public DateTime DateLastCrafted { get; protected set; }

    public virtual void SetAmount(int amount)
    {
        AmountOwned += amount;
        if(amount>0) SetLastCraftedTime();     /// Items are considered created when the amount is raised, therefore we inhibit datetime fro mbeing updated in case of amount reduction
    }
    public virtual int GetAmount()
    {
        return AmountOwned;
    }

    public DateTime GetLastCraftedDate()
    {
        return DateLastCrafted;
    }

    public void SetLastCraftedTime()
    {
        DateLastCrafted = DateTime.Now;
    }
}
