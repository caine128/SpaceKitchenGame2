using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraComponentsType 
{
   // public static int minUnderlyingValue = (int)AllType.Iron_Pine_Cone;
    public enum AllType
    {
        DateMostRecent = Sort.Type.DateMostRecent,    //001
        Quantity = Sort.Type.Quantity,                //003
        Type = Sort.Type.Type,                        //004

        Iron_Pine_Cone = 101,
        Elven_Wood = 102,
        Glow_Shroom = 103,
        Silver_Dust = 104,
        Webbed_Wing = 105,
        Precious_Gem = 106,
        Living_Root = 107,
        Rustwyrm_Scale = 108,
        Deep_Pearl = 109,
        Sigil_Of_Spark = 110,

        None = -100,

    }

    public enum Type
    {
        Iron_Pine_Cone = AllType.Iron_Pine_Cone,
        Elven_Wood = AllType.Elven_Wood,
        Glow_Shroom = AllType.Glow_Shroom,
        Silver_Dust = AllType.Silver_Dust,
        Webbed_Wing = AllType.Webbed_Wing,
        Precious_Gem = AllType.Precious_Gem,
        Living_Root = AllType.Living_Root,
        Rustwyrm_Scale = AllType.Rustwyrm_Scale,
        Deep_Pearl = AllType.Deep_Pearl,
        Sigil_Of_Spark = AllType.Sigil_Of_Spark,

        None = AllType.None,
    }


    public static int GetNormalizedEnumIndex(Type extraComponentType)
    {
        return ((int)extraComponentType - (int)AllType.Iron_Pine_Cone);
    }
}
