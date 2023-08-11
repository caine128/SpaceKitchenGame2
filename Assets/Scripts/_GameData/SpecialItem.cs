using System;
using UnityEngine;

public abstract class SpecialItem : GameObject , IToolTipDisplayable
{
    protected readonly SpecialItemType.Type itemType;   
    
    public SpecialItem(SpecialItemType.Type itemType_IN, DateTime? dateLastCrafted_IN = null)            
    {
        this.itemType = itemType_IN;
        DateLastCrafted = dateLastCrafted_IN ?? DateTime.Now;
    }

    public SpecialItemType.Type GetSpecialItemType()
    {
        return itemType;
    }

    public ToolTipInfo GetToolTipText()
        => new(bodytextAsColumns: new string[1] { GetDescription() }, header: GetName(),footer:null);
    /*{
        return new string[]
                            {
                                NativeHelper.BuildString_Append(GetName(),
                                                                Environment.NewLine,
                                                                GetDescription())
                            };
    }*/


    // Pay ATTENTION TO THIS !?!?!?!? NEED TO REMOVE 
    //public abstract bool Equals(SpecialItem other);
    //{
    //    if (other == null || GetType() != other.GetType())
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        return itemType == other.GetSpecialItemType();
    //    }
    //}

    /*
    public override bool Equals(object obj)
    {
        return Equals(obj as SpecialItem);
    }
    public override int GetHashCode()
    {
        return (int)itemType * 23;
    }
    */
}
