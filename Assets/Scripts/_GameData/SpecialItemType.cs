using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialItemType 
{
    public static int minUnderlyingValue = (int)AllType.Chest;
    public enum AllType
    {
        DateMostRecent = Sort.Type.DateMostRecent,    //001
        Quantity = Sort.Type.Quantity,                //003
        Type = Sort.Type.Type,                        //004


        Chest = 101,
        Key = 102,
        ResearchScroll = 103,
        AscensionShard = 104,

        None = -100,
    }
   public enum Type
    {
        Chest = AllType.Chest,
        Key = AllType.Key,
        ResearchScroll = AllType.ResearchScroll,
        AscensionShard = AllType.AscensionShard,

        None = AllType.None,
    }
}
