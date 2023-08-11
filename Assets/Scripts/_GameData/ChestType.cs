using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestType 
{
    public static int minUnderlyingValue = (int)Type.Wooden_Chest;

    public enum Type
    {
        None            =  -999,
        Wooden_Chest    =0,
        Frozen_Chest    =1,
        Slimy_Chest     =2,
    }
}
